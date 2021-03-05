using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Rendering;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using LlockhamIndustries.ExtensionMethods;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LlockhamIndustries.Decals
{
    /**
    * \mainpage Welcome to the documentation
    * To keep things from being too overwhelming I've only documented what's necessary to script with the system.
    * For more advanced users, everything in the system is commented and built to be expanded on. Dig through the code to your hearts content.
    * If you have any questions or get stuck at any stage I'm always available at Support@LlockhamIndustries.com.
    */

    /**
    * The core class of the system, responsible for the majority of the systems functionality. 
    * For scripting purposes, it's almost entirely a black box, you should rarely need to access or modify anything within it.
    * It's well stuctured and commented all the same though, so if your interested, open it up and have a look around.
    */
    [ExecuteInEditMode]
    public class DynamicDecals : MonoBehaviour
    {
        //MultiScene Editor Singleton
        public static bool Initialized
        {
            get { return system != null; }
        }
        public static DynamicDecals System
        {
            get
            {
                if (system == null)
                {
                    //Create hidden host game object
                    GameObject go = new GameObject("Dynamic Decals");
                    go.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild | HideFlags.HideInHierarchy | HideFlags.HideInInspector;

                    //Setup system
                    go.AddComponent<DynamicDecals>();
                }
                return system;
            }
        }
        private static DynamicDecals system;

        private void Start()
        {
            //Don't destroy on scene switch
            if (Application.isPlaying) DontDestroyOnLoad(gameObject);
        }
        private void OnEnable()
        {
            //Singleton
            if (system == null)
            {
                system = this;
            }
            else if (system != this)
            {
                if (Application.isPlaying) Destroy(gameObject);
                else DestroyImmediate(gameObject, true);
                return;
            }

            Initialize();
        }
        private void OnDisable()
        {
            //Terminate the system
            Terminate();
        }

        #if UNITY_EDITOR
        private void OnApplicationQuit()
        {
            //Reset the system when transitioning back to edit mode
            Terminate();
            Initialize();
        }
        #endif

        //Settings
        public DynamicDecalSettings Settings
        {
            get
            {
                //Try load our settings
                if (settings == null) settings = Resources.Load<DynamicDecalSettings>("Settings");

                //If not found create them
                if (settings == null) settings = ScriptableObject.CreateInstance<DynamicDecalSettings>();
                return settings;
            }
        }
        private DynamicDecalSettings settings;
        public static void ApplySettings()
        {
            //In editor settings subject to change
            system.settings = Resources.Load<DynamicDecalSettings>("Settings");
        }

        private bool FireInCulling
        {
            get
            {
                return !(UnityEngine.XR.XRSettings.enabled || Settings.Replacement == ShaderReplacementType.VR);
            }
        }

        #region Rendering
        //Rendering Path
        public SystemPath SystemPath
        {
            get { return renderingPath; }
        }
        public SystemPath renderingPath;

        //Shader Replacement
        public bool ShaderReplacement
        {
            get { return Projections.Count > 0 && shaderReplacement; }
            set { shaderReplacement = value; }
        }
        private bool shaderReplacement = true;

        internal RenderTextureFormat depthFormat;
        internal RenderTextureFormat normalFormat;
        internal RenderTextureFormat maskFormat;

        //Instanced
        public bool Instanced
        {
            get { return SystemInfo.supportsInstancing; }
        }

        //Methods
        private void UpdateSystemPath()
        {
            //Get our primary camera
            Camera target = null;
            if (Camera.main != null) target = Camera.main;
            else if (Camera.current != null) target = Camera.current;

            if (target != null)
            {
                //Determine our rendering method
                if (target.actualRenderingPath == RenderingPath.Forward || target.actualRenderingPath == RenderingPath.DeferredShading)
                {
                    SystemPath newPath = SystemPath.Forward;
                    if (target.actualRenderingPath == RenderingPath.DeferredShading) newPath = SystemPath.Deferred;

                    if (renderingPath != newPath)
                    {
                        renderingPath = newPath;
                        UpdateRenderers();
                    }
                }
                else Debug.LogWarning("Current Rendering Path not supported! Please use either Forward or Deferred");
            }
        }
        public void RestoreDepthTextureModes()
        {
            //Iterate over every camera and restore it to it's original depth texture mode
            for (int i = 0; i < cameraData.Count; i++)
            {
                Camera camera = cameraData.ElementAt(i).Key;
                if (camera != null) cameraData.ElementAt(i).Value.RestoreDepthTextureMode(camera);
            }
        }
        #endregion
        #region Projections
        //Projections
        private List<ProjectionData> Projections;
        private ProjectionData GetProjectionData(Projection Projection)
        {
            for (int i = 0; i < Projections.Count; i++)
            {
                if (Projections[i].projection == Projection)
                {
                    return Projections[i];
                }
            }
            return null;
        }
        private void UpdateProjectionData()
        {
            for (int i = 0; i < Projections.Count; i++)
            {
                Projections[i].Update();
            }
        }

        //Registration
        public bool Register(ProjectionRenderer Instance)
        {
            if (Instance != null)
            {
                //Determine our projection
                Projection projection = Instance.Projection;

                //Check if our projection has been registered
                ProjectionData data = GetProjectionData(projection);
                if (data != null)
                {
                    data.Add(Instance);
                    return isActiveAndEnabled;
                }
                else
                {
                    //Create and register our projection data
                    data = new ProjectionData(projection);
                    data.Add(Instance);

                    //If we are lower priority than a projection in the list, insert ourself before them
                    for (int i = 0; i < Projections.Count; i++)
                    {
                        if (projection.Priority < Projections[i].projection.Priority)
                        {
                            Projections.Insert(i, data);
                            return true;
                        }
                    }

                    //If we are higher priority than everything in the list, just add ourself
                    Projections.Add(data);
                    return isActiveAndEnabled;
                }
            }
            return false;
        }
        public void Deregister(ProjectionRenderer Instance)
        {
            if (Instance != null)
            {
                //Determine our projection
                Projection projection = Instance.Projection;

                //Check if our projection has been registered
                for (int i = 0; i < Projections.Count; i++)
                {
                    if (Projections[i].projection == projection)
                    {
                        //Remove instance from projection
                        Projections[i].Remove(Instance);

                        //Remove empty projections
                        if (Projections[i].instances.Count == 0)
                        {
                            Projections.RemoveAt(i);
                        }
                        return;
                    }
                }
            }
        }
        public void Reorder(Projection Projection)
        {
            ProjectionData data = GetProjectionData(Projection);
            if (data != null)
            {
                //Remove ourself from the list
                Projections.Remove(data);

                //Insert ourself back in the correct position
                for (int i = 0; i < Projections.Count; i++)
                {
                    if (Projection.Priority < Projections[i].projection.Priority)
                    {
                        Projections.Insert(i, data);
                        return;
                    }
                }

                //If no correct position found add ourself to the end
                Projections.Add(data);

                //Reorder renderers
                OrderRenderers();
            }
        }

        //Order renderers
        public void OrderRenderers()
        {
            if (renderersMarked && Projections != null)
            {
                int i = 1;
                foreach (ProjectionData projection in Projections)
                {
                    projection.AssertOrder(ref i);
                }
            }
        }
        public void MarkRenderers()
        {
            renderersMarked = true;
        }
        private bool renderersMarked;

        //Update all renderers
        public void UpdateRenderers()
        {
            if (Projections != null)
            {
                for (int i = 0; i < Projections.Count; i++)
                {
                    Projections[i].UpdateRenderers();
                }
            }
        }
        public void UpdateRenderers(Projection Projection)
        {
            if (Projections != null)
            {
                for (int i = 0; i < Projections.Count; i++)
                {
                    if (Projections[i].projection == Projection)
                    {
                        Projections[i].UpdateRenderers();
                        return;
                    }
                }
            }
        }

        //Debug
        public int ProjectionCount
        {
            get { return Projections.Count; }
        }
        public int RendererCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < Projections.Count; i++) count += Projections[i].instances.Count;
                return count;
            }
        }
        #endregion
        #region Meshes & Shaders
        public Mesh Cube
        {
            get
            {
                if (cube == null)
                {
                    cube = Resources.Load<Mesh>("Decal");
                    cube.name = "Projection";
                }
                return cube;
            }
        }

        //Triple Pass / Classic
        public Shader DepthShader
        {
            get
            {
                if (depthShader == null)
                {
                    depthShader = Shader.Find("Projection/Internal/Depth");
                }
                return depthShader;
            }

        }
        public Shader NormalShader
        {
            get
            {
                if (normalShader == null)
                {
                    normalShader = Shader.Find("Projection/Internal/Normal");
                }
                return normalShader;
            }
        }
        public Shader MaskShader
        {
            get
            {
                if (maskShader == null)
                {
                    maskShader = Shader.Find("Projection/Internal/Mask");
                }
                return maskShader;
            }
        }
        
        //Double Pass
        public Shader NormalMaskShader
        {
            get
            {
                if (normalMaskShader == null)
                {
                    normalMaskShader = Shader.Find("Projection/Internal/NormalMask");
                }
                return normalMaskShader;
            }
        }

        //Single Pass
        public Shader DepthNormalMaskShader
        {
            get
            {
                if (depthNormalMaskShader == null)
                {
                    depthNormalMaskShader = Shader.Find("Projection/Internal/DepthNormalMask");
                }
                return depthNormalMaskShader;
            }
        }

        //Packed
        public Shader DepthNormalMaskShader_Packed
        {
            get
            {
                if (depthNormalMaskShader_Packed == null)
                {
                    depthNormalMaskShader_Packed = Shader.Find("Projection/Internal/DepthNormalMask_Packed");
                }
                return depthNormalMaskShader_Packed;
            }
        }

        public Material StereoBlitLeft
        {
            get
            {
                if (stereoBlitLeft == null)
                {
                    stereoBlitLeft = new Material(Shader.Find("Projection/Internal/StereoBlitLeft"));
                }
                return stereoBlitLeft;
            }
        }
        public Material StereoBlitRight
        {
            get
            {
                if (stereoBlitRight == null)
                {
                    stereoBlitRight = new Material(Shader.Find("Projection/Internal/StereoBlitRight"));
                }
                return stereoBlitRight;
            }
        }
        public Material StereoDepthBlitLeft
        {
            get
            {
                if (stereoDepthBlitLeft == null)
                {
                    stereoDepthBlitLeft = new Material(Shader.Find("Projection/Internal/StereoDepthBlitLeft"));
                }
                return stereoDepthBlitLeft;
            }
        }
        public Material StereoDepthBlitRight
        {
            get
            {
                if (stereoDepthBlitRight == null)
                {
                    stereoDepthBlitRight = new Material(Shader.Find("Projection/Internal/StereoDepthBlitRight"));
                }
                return stereoDepthBlitRight;
            }
        }

        //Backing Fields
        private Mesh cube;

        private Shader depthShader;
        private Shader normalShader;
        private Shader maskShader;

        private Shader depthNormalShader;
        private Shader normalMaskShader;

        private Shader depthNormalMaskShader;
        private Shader depthNormalMaskShader_Packed;

        private Material stereoBlitLeft;
        private Material stereoBlitRight;
        private Material stereoDepthBlitLeft;
        private Material stereoDepthBlitRight;
        #endregion
        #region Masking
        private void SetupMaskedMaterials()
        {
            foreach (Material material in Settings.Materials) material.renderQueue = 2999;
        }
        #endregion
        #region Cameras
        //Scene camera data
        internal Dictionary<Camera, CameraData> cameraData = new Dictionary<Camera, CameraData>();
        internal CameraData GetData(Camera Camera)
        {
            //Declare our Camera Data
            CameraData data = null;

            //Check if this camera already has camera data
            if (!cameraData.TryGetValue(Camera, out data))
            {
                //Generate data
                data = new CameraData();

                //Store data
                cameraData[Camera] = data;
            }

            //Initialize if required
            if (data != null)
            {
                if (!data.initialized && Camera.GetComponent<ProjectionBlocker>() == null) data.Initialize(Camera, this);
                else if (data.initialized && Camera.GetComponent<ProjectionBlocker>() != null) data.Terminate(Camera);
            }

            //Return our updated Camera Data
            return data;
        }

        //Shader replacement camera
        public Camera CustomCamera
        {
            get
            {
                if (customCamera == null)
                {
                    GameObject cameraObject = new GameObject("Custom Camera");
                    customCamera = cameraObject.AddComponent<Camera>();
                    cameraObject.AddComponent<ProjectionBlocker>();
                    cameraObject.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild | HideFlags.HideInHierarchy | HideFlags.HideInInspector;
                    cameraObject.SetActive(false);

                    if (Application.isPlaying) GameObject.DontDestroyOnLoad(cameraObject);
                }
                return customCamera;
            }
        }
        private Camera customCamera;
        #endregion
        #region Pools
        private Dictionary<int, ProjectionPool> Pools;
        internal ProjectionPool PoolFromInstance(PoolInstance Instance)
        {
            //Make sure we are initialized
            if (Pools == null) Pools = new Dictionary<int, ProjectionPool>();

            ProjectionPool pool;
            if (!Pools.TryGetValue(Instance.id, out pool))
            {
                pool = new ProjectionPool(Instance);
                Pools.Add(Instance.id, pool);
            }
            return pool;
        }

        public ProjectionPool DefaultPool
        {
            get { return PoolFromInstance(Settings.pools[0]); }
        }

        /**
         * Returns a pool with the specified name, if it exists. If it doesn't, returns the default pool.
         * @param Title The title of the pool to be returned.
         */
        public ProjectionPool GetPool(string Title)
        {
            //Check Settings for an ID
            for (int i = 0; i < Settings.pools.Length; i++)
            {
                if (settings.pools[i].title == Title)
                {
                    return PoolFromInstance(settings.pools[i]);
                }
            }
            //No valid pool set up, log a Warning and return the default pool
            Debug.LogWarning("No valid pool with the title : " + Title + " found. Returning default pool");
            return PoolFromInstance(settings.pools[0]);
        }
        /**
         * Returns a pool with the specified ID, if it exists. If it doesn't, returns the default pool.
         * @param ID The ID of the pool to be returned.
         */
        public ProjectionPool GetPool(int ID)
        {
            //Check Settings for an ID
            for (int i = 0; i < Settings.pools.Length; i++)
            {
                if (settings.pools[i].id == ID)
                {
                    return PoolFromInstance(settings.pools[i]);
                }
            }
            //No valid pool set up, log a Warning and return the default pool
            Debug.LogWarning("No valid pool with the ID : " + ID + " found. Returning default pool");
            return PoolFromInstance(settings.pools[0]);
        }
        #endregion

        //Initialize / Terminate
        private void Initialize()
        {
            #if UNITY_EDITOR
            Settings.CalculateVR();

            SceneView.onSceneGUIDelegate += OnSceneGUI;
            Undo.undoRedoPerformed += UndoRedo;
            #endif

            //Determine texture formats
            depthFormat = RenderTextureFormat.Depth;
            normalFormat = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB2101010) ? RenderTextureFormat.ARGB2101010 : RenderTextureFormat.ARGB32;
            maskFormat = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB32) ? RenderTextureFormat.ARGB32 : RenderTextureFormat.ARGB32;

            //Register our projection events to all cameras
            Camera.onPreCull += SuperLateUpdate;
            Camera.onPreRender += PreRender;

            //Initialize projections
            if (Projections == null) Projections = new List<ProjectionData>();
            else
            {
                for(int i = 0; i < Projections.Count; i++)
                {
                    Projections[i].EnableRenderers();
                }
            }

            //Masked materials
            SetupMaskedMaterials();
        }
        private void Terminate()
        {
            #if UNITY_EDITOR
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            Undo.undoRedoPerformed -= UndoRedo;
            #endif

            //Deregister our projection events
            Camera.onPreCull -= SuperLateUpdate;
            Camera.onPreRender -= PreRender;

            //Iterate over our camera data
            foreach (var cb in cameraData)
            {
                //Terminate camera data
                cb.Value.Terminate(cb.Key);
            }

            //Clear camera Data
            cameraData.Clear();

            //Disable our projections
            if (Projections != null)
            {
                for (int i = 0; i < Projections.Count; i++)
                {
                    Projections[i].DisableRenderers();
                }
            }
        }

        //Primary Methods
        private void LateUpdate()
        {
            //Check our system path
            UpdateSystemPath();

            //Update projections
            UpdateProjectionData();

            //Order renderers
            OrderRenderers();

            //Uncomment below to log debug information while in a development build
            //DebugInDevelopmentBuild();
        }
        private void SuperLateUpdate(Camera Camera)
        {
            if (FireInCulling)
            {
                //Grab our camera data
                CameraData data = GetData(Camera);

                //Only run on initialized cameras
                if (data.initialized && (Camera.cameraType == CameraType.SceneView || Camera.cameraType == CameraType.Preview || Camera.isActiveAndEnabled))
                {
                    //Update and perform shader replacement
                    data.Update(Camera, this);
                }
            }
        }
        private void PreRender(Camera Camera)
        {
            //Grab our camera data
            CameraData data = GetData(Camera);

            //Only run on initialized cameras
            if (data.initialized && (Camera.cameraType == CameraType.SceneView || Camera.cameraType == CameraType.Preview || Camera.isActiveAndEnabled))
            {
                if (!FireInCulling)
                {
                    //Update and perform shader replacement
                    data.Update(Camera, this);
                }

                //Shader replacement
                data.AssignGlobalProperties(Camera);
            }
        }

        //Debug
        public static string DebugLog
        {
            get
            {
                string debug = "Debug Information (Copy and Paste) \r\n";

                //General settings
                debug += "\r\nGeneral\r\n";
                debug += "OS : " + SystemInfo.operatingSystem + "\r\n";
                debug += "Graphics device : " + SystemInfo.graphicsDeviceName + "\r\n";
                debug += "Graphics API : " + SystemInfo.graphicsDeviceType + "\r\n";

                //Camera & shader replacement settings
                Camera cam = Camera.main;
                if (cam != null)
                {
                    debug += "\r\nCamera\r\n";
                    debug += "Rendering path : " + cam.actualRenderingPath + "\r\n";
                    debug += "Is orthographic : " + cam.orthographic + "\r\n";

                    debug += "\r\nShader Replacement\r\n";
                    debug += "Method : " + System.GetData(cam).replacement.ToString() + "\r\n";
                }
                else
                {
                    debug += "\r\nMain camera not found\r\nPlease tag your main camera\r\n";
                }

                //Virtual reality settings
                if (UnityEngine.XR.XRSettings.enabled)
                {
                    debug += "\r\nVirtualReality : " + UnityEngine.XR.XRSettings.isDeviceActive + "\r\n";
                    debug += "VR API : " + UnityEngine.XR.XRSettings.loadedDeviceName + "\r\n";
                    debug += "VR device : " + UnityEngine.XR.XRDevice.model + "\r\n";

                    #if UNITY_EDITOR
                    debug += "Stereo rendering path : " + (System.Settings.SinglePassVR? "SinglePass" : "MultiPass") + "\r\n";
                    #endif
                }

                return debug;
            }
        }
        public static void DebugInDevelopmentBuild()
        {
            if (Debug.isDebugBuild) Debug.Log(DebugLog);
        }

        //Editor Scene Placement
        #if UNITY_EDITOR
        private List<GameObject> dragables = new List<GameObject>();
        private void OnSceneGUI(SceneView sceneView)
        {
            //Drag Update
            if (Event.current.type == EventType.DragUpdated)
            {
                //Check for projection renderers among selection
                if (dragables.Count == 0)
                {
                    foreach (UnityEngine.Object o in DragAndDrop.objectReferences)
                    {
                        GameObject go = o as GameObject;
                        if (go != null)
                        {
                            if (go.GetComponent<ProjectionRenderer>() != null)
                            {
                                //Create our dragable
                                GameObject dragable = PrefabUtility.InstantiatePrefab(go) as GameObject;
                                dragable.name = go.name;
                                dragable.hideFlags = HideFlags.HideInHierarchy;

                                //Register to list
                                dragables.Add(dragable);
                            }
                        }
                    }
                }

                //Position dragables
                if (dragables.Count > 0)
                {
                    RaycastHit hit;
                    Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                    {
                        foreach (GameObject go in dragables)
                        {
                            go.transform.position = hit.point;
                            go.transform.rotation = Quaternion.LookRotation(-hit.normal);
                        }
                    }
                    else
                    {
                        foreach (GameObject go in dragables)
                        {
                            go.transform.position = Vector3.zero;
                            go.transform.rotation = Quaternion.LookRotation(-Vector3.up);
                        }
                    }

                    //Set mode
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    //Use event
                    Event.current.Use();
                }
            }

            //Drag Perform
            if (Event.current.type == EventType.DragPerform && dragables.Count > 0)
            {
                //Show dragables
                foreach (GameObject go in dragables)
                {
                    //Show objects in heirarchy
                    go.hideFlags = HideFlags.None;

                    //Register Undo
                    Undo.RegisterCreatedObjectUndo(go, "Instantiate Prefab");
                }

                //No longer require dragables
                dragables.Clear();

                //Use event
                Event.current.Use();
            }

            //Drag Exit
            if (Event.current.type == EventType.DragExited && dragables.Count > 0)
            {
                //Destroy all dragables
                foreach (GameObject go in dragables) DestroyImmediate(go);
                dragables.Clear();

                //Use event
                Event.current.Use();
            }
        }
        private void UndoRedo()
        {
            dragables.Clear();
        }
        #endif
    }

    //System path
    public enum SystemPath { Forward, Deferred };

    //Projection management
    public class ProjectionData
    {
        //Projection
        public Projection projection;
        public void Update()
        {
            projection.Update();
        }

        //Instances
        public List<ProjectionRenderer> instances;

        public void Add(ProjectionRenderer Instance)
        {
            //If the instance isn't already in the list, add it
            if (!instances.Contains(Instance))
            {
                //Add our instance
                instances.Add(Instance);

                //Notify instance of data change
                Instance.Data = this;

                //Mark renderers to change
                DynamicDecals.System.MarkRenderers();
            }

        }
        public void Remove(ProjectionRenderer Instance)
        {
            //Attempt to remove the instance from the list
            instances.Remove(Instance);

            //Notify instance of data change
            if (Instance.Data == this) Instance.Data = null;
        }

        public void MoveToTop(ProjectionRenderer Instance)
        {
            //Remove the instance from the list
            instances.Remove(Instance);

            //Add instance to top of list
            instances.Add(Instance);

            //Mark renderers to change
            DynamicDecals.System.MarkRenderers();
        }
        public void MoveToBottom(ProjectionRenderer Instance)
        {
            //Remove the instance from the list
            instances.Remove(Instance);

            //Add instance to top of list
            instances.Insert(0, Instance);

            //Mark renderers to change
            DynamicDecals.System.MarkRenderers();
        }

        //Constructor
        public ProjectionData(Projection Projection)
        {
            //Initialize Projection
            projection = Projection;
            instances = new List<ProjectionRenderer>();
        }

        //Order
        public void AssertOrder(ref int Order)
        {
            if (projection.Instanced)
            {
                foreach (ProjectionRenderer renderer in instances)
                {
                    renderer.Renderer.sortingOrder = Order;
                }
                Order++;
            }
            else
            {
                foreach (ProjectionRenderer renderer in instances)
                {
                    renderer.Renderer.sortingOrder = Order;
                    Order++;
                }
            }
        }

        //Renderers
        public void EnableRenderers()
        {
            for (int j = 0; j < instances.Count; j++)
            {
                instances[j].InitializeRenderer(true);
            }
        }
        public void DisableRenderers()
        {
            for (int j = 0; j < instances.Count; j++)
            {
                instances[j].TerminateRenderer();
            }
        }
        public void UpdateRenderers()
        {
            for (int j = 0; j < instances.Count; j++)
            {
                instances[j].UpdateProjection();
            }
        }
    }

    //Camera management
    internal class CameraData
    {
        //Depth texture mode
        public DepthTextureMode? originalDTM, desiredDTM = null;

        //Shader replacement
        public ShaderReplacement replacement;

        //Render textures
        private RenderTexture depthBuffer;
        private RenderTexture normalBuffer;
        private RenderTexture maskBuffer;

        //Eye textures (Single pass stereo)
        private RenderTexture depthEye;
        private RenderTexture normalEye;
        private RenderTexture maskEye;

        //Core functions
        public bool initialized;
        public void Initialize(Camera Camera, DynamicDecals System)
        {
            initialized = true;
        }
        public void Terminate(Camera Camera)
        {
            //Restore cameras depthTexture mode
            RestoreDepthTextureMode(Camera);

            //Release render textures
            ReleaseTextures();

            //Disable
            initialized = false;
        }
        public void Update(Camera Camera, DynamicDecals System)
        {
            //Update rendering method
            UpdateRenderingMethod(Camera, System);

            //Update render textures
            UpdateRenderTextures(Camera, System);

            //render shader replacement
            UpdateShaderReplacement(Camera, System);
        }

        //PreRender
        public void AssignGlobalProperties(Camera Camera)
        {
            if (replacement != ShaderReplacement.Null)
            {
                switch (replacement)
                {
                    case ShaderReplacement.Classic:
                        //Assign mask buffer
                        maskBuffer.SetGlobalShaderProperty("_MaskBuffer_0");

                        //Tell shaders not to use custom or precision depth/normals
                        Shader.DisableKeyword("_PrecisionDepthNormals");
                        Shader.DisableKeyword("_CustomDepthNormals");
                        Shader.DisableKeyword("_PackedDepthNormals");
                        break;

                    case ShaderReplacement.TriplePass:
                    case ShaderReplacement.DoublePass:
                        //Assign custom buffers
                        if (Camera.actualRenderingPath == RenderingPath.DeferredShading)
                        {
                            //Assign custom depth/normals
                            depthBuffer.SetGlobalShaderProperty("_CustomDepthTexture");
                            normalBuffer.SetGlobalShaderProperty("_CustomNormalTexture");

                            //Tell shaders to use custom depth/normals
                            Shader.DisableKeyword("_PrecisionDepthNormals");
                            Shader.EnableKeyword("_CustomDepthNormals");
                            Shader.DisableKeyword("_PackedDepthNormals");
                        }
                        else
                        {
                            //Assign custom normals
                            normalBuffer.SetGlobalShaderProperty("_CustomNormalTexture");

                            //Tell shaders to use precision depth/normals
                            Shader.EnableKeyword("_PrecisionDepthNormals");
                            Shader.DisableKeyword("_CustomDepthNormals");
                            Shader.DisableKeyword("_PackedDepthNormals");
                        }

                        //Assign mask buffer
                        maskBuffer.SetGlobalShaderProperty("_MaskBuffer_0");
                        break;

                    case ShaderReplacement.SinglePass:
                        //Assign custom depth/normals/mask
                        depthBuffer.SetGlobalShaderProperty("_CustomDepthTexture");
                        normalBuffer.SetGlobalShaderProperty("_CustomNormalTexture");
                        maskBuffer.SetGlobalShaderProperty("_MaskBuffer_0");

                        //Tell shaders to use custom depth/normals
                        Shader.DisableKeyword("_PrecisionDepthNormals");
                        Shader.EnableKeyword("_CustomDepthNormals");
                        Shader.DisableKeyword("_PackedDepthNormals");
                        break;

                    case ShaderReplacement.SingleTarget:
                        //Assign custom depth/normals
                        depthBuffer.SetGlobalShaderProperty("_CustomDepthNormalMaskTexture");

                        //Tell shaders to use packed depth normals
                        Shader.DisableKeyword("_PrecisionDepthNormals");
                        Shader.DisableKeyword("_CustomDepthNormals");
                        Shader.EnableKeyword("_PackedDepthNormals");
                        break;
                }

                
            }
        }

        //Rendering method
        private ShaderReplacement Standard (bool VR)
        {
            //VR requires rendering into seperate buffers
            if (VR) return ShaderReplacement.TriplePass;

            //Singlepass requires rendering to 3 buffers
            if (SystemInfo.supportedRenderTargetCount < 3)
            {
                return ShaderReplacement.DoublePass;
            }

            #if !UNITY_2017_2_OR_NEWER
            //DirectX9 Doesn't seem to like rendering to different renderTexture formats at the same time
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D9)
            {
                return ShaderReplacement.DoublePass;
            }
            #endif

            //Otherwise use singlepass
            return ShaderReplacement.SinglePass;
        }
        private bool VRCamera(Camera Source)
        {
            if (Source.cameraType == CameraType.SceneView || Source.cameraType == CameraType.Preview) return false;
            return (UnityEngine.XR.XRSettings.enabled && Source.stereoTargetEye != StereoTargetEyeMask.None);
        }

        private void UpdateRenderingMethod(Camera Camera, DynamicDecals System)
        {
            //Use standard shader replacement by default
            ShaderReplacement shaderReplacement = Standard(UnityEngine.XR.XRSettings.enabled && Camera.stereoTargetEye != StereoTargetEyeMask.None);

            //Unless otherwise specified
            switch (System.Settings.Replacement)
            {
                case ShaderReplacementType.VR:
                    shaderReplacement = ShaderReplacement.SingleTarget;
                    break;
                case ShaderReplacementType.Mobile:
                    shaderReplacement = ShaderReplacement.Classic;
                    break;
            }

            if (depthBuffer != null) Misc.DebugManager.Log("Depth Format", depthBuffer.format.ToString());
            if (normalBuffer != null) Misc.DebugManager.Log("Normal Format", normalBuffer.format.ToString());
            if (maskBuffer != null) Misc.DebugManager.Log("Mask Format", maskBuffer.format.ToString());
            Misc.DebugManager.Log("Shader Replacement", shaderReplacement.ToString());
            Misc.DebugManager.Log("API", SystemInfo.graphicsDeviceType.ToString());

            //Update our rendering method
            if (replacement != shaderReplacement)
            {
                //Update our rendering method
                replacement = shaderReplacement;

                //Add ourself to the new rendering method
                SwitchRenderingMethod(Camera);

                //Set up our new render textures
                UpdateRenderTextures(Camera, System, true);
            }
        }
        private void SwitchRenderingMethod(Camera Camera)
        {
            switch (replacement)
            {
                case ShaderReplacement.Classic:
                    //Low precision depth & normals
                    desiredDTM = DepthTextureMode.DepthNormals;
                    SetDepthTextureMode(Camera);
                    break;

                case ShaderReplacement.TriplePass:
                case ShaderReplacement.DoublePass:

                    if (Camera.actualRenderingPath == RenderingPath.DeferredShading)
                    {
                        //Use custom depth, Unity's depth renders too late
                        RestoreDepthTextureMode(Camera);
                    }
                    else
                    {
                        //High precision depth
                        desiredDTM = DepthTextureMode.Depth;
                        SetDepthTextureMode(Camera);
                    }
                    
                    break;

                case ShaderReplacement.SinglePass:
                case ShaderReplacement.SingleTarget:
                    //Restore our depth texture mode
                    RestoreDepthTextureMode(Camera);
                    break;
            }
        }        

        //Native shader replacement
        private void SetDepthTextureMode(Camera Camera)
        {
            //If we have a desired value change to it.
            if (desiredDTM.HasValue)
            {
                if (Camera.depthTextureMode != desiredDTM)
                {
                    //If we haven't already, Cache the original depth texture mode, otherwise revert to it.
                    if (!originalDTM.HasValue) originalDTM = Camera.depthTextureMode;
                    else Camera.depthTextureMode = originalDTM.Value;

                    //Add our desired depth texture mode.
                    Camera.depthTextureMode |= desiredDTM.Value;
                }
            }
            //If we have no desired value, switch back to the original value.
            else RestoreDepthTextureMode(Camera);
        }
        public void RestoreDepthTextureMode(Camera Camera)
        {
            //Restore the depth texture mode to the cached
            if (originalDTM.HasValue && Camera != null)
            {
                Camera.depthTextureMode = originalDTM.Value;
            }
        }

        //Custom render textures
        private void UpdateRenderTextures(Camera Camera, DynamicDecals System, bool ForceNewTextures = false)
        {
            //Calculate width and height
            int width = Camera.pixelWidth;
            int height = Camera.pixelHeight;

            //VR cameras use eye texture values
            if (VRCamera(Camera))
            {
                width = (System.Settings.SinglePassVR) ? UnityEngine.XR.XRSettings.eyeTextureWidth * 2 : UnityEngine.XR.XRSettings.eyeTextureWidth;
                height = UnityEngine.XR.XRSettings.eyeTextureHeight;
            }

            //If the size has changed, grab new render textures of the new size
            if (maskBuffer == null || maskBuffer.width != width || maskBuffer.height != height || ForceNewTextures)
            {
                ReleaseTextures();
                GetTextures(Camera, System, width, height);
            }
        }

        private void GetTextures(Camera Camera, DynamicDecals System, int Width, int Height)
        {
            switch (replacement)
            {
                case ShaderReplacement.SingleTarget:
                    //Get single combined buffer for everything
                    depthBuffer = RenderTexture.GetTemporary(Width, Height, 24, RenderTextureFormat.RGFloat);

                    //VR compatible, get eye version as required
                    if (VRCamera(Camera) && System.Settings.SinglePassVR) depthEye = RenderTexture.GetTemporary(UnityEngine.XR.XRSettings.eyeTextureWidth, UnityEngine.XR.XRSettings.eyeTextureHeight, 24, RenderTextureFormat.RGFloat);
                    break;

                case ShaderReplacement.SinglePass:
                    //Get seperate buffers for everything
                    depthBuffer = RenderTexture.GetTemporary(Width, Height, 24, System.depthFormat);
                    normalBuffer = RenderTexture.GetTemporary(Width, Height, 0, System.normalFormat);
                    maskBuffer = RenderTexture.GetTemporary(Width, Height, 0, System.maskFormat);
                    break;

                case ShaderReplacement.DoublePass:
                    //Get seperate buffers for everything
                    if (Camera.actualRenderingPath == RenderingPath.DeferredShading) depthBuffer = RenderTexture.GetTemporary(Width, Height, 24, System.depthFormat);
                    normalBuffer = RenderTexture.GetTemporary(Width, Height, 24, System.normalFormat);
                    maskBuffer = RenderTexture.GetTemporary(Width, Height, 0, System.maskFormat);
                    break;

                case ShaderReplacement.TriplePass:
                    //Get seperate buffers for everything
                    if (Camera.actualRenderingPath == RenderingPath.DeferredShading) depthBuffer = RenderTexture.GetTemporary(Width, Height, 24, System.depthFormat);
                    normalBuffer = RenderTexture.GetTemporary(Width, Height, 24, System.normalFormat);
                    maskBuffer = RenderTexture.GetTemporary(Width, Height, 24, System.maskFormat);

                    //VR compatible, get eye version as required
                    if (VRCamera(Camera) && System.Settings.SinglePassVR)
                    {
                        if (Camera.actualRenderingPath == RenderingPath.DeferredShading) depthEye = RenderTexture.GetTemporary(UnityEngine.XR.XRSettings.eyeTextureWidth, UnityEngine.XR.XRSettings.eyeTextureHeight, 24, System.depthFormat);
                        normalEye = RenderTexture.GetTemporary(UnityEngine.XR.XRSettings.eyeTextureWidth, UnityEngine.XR.XRSettings.eyeTextureHeight, 24, System.normalFormat);
                        maskEye = RenderTexture.GetTemporary(UnityEngine.XR.XRSettings.eyeTextureWidth, UnityEngine.XR.XRSettings.eyeTextureHeight, 24, System.maskFormat);
                    }
                    break;

                case ShaderReplacement.Classic:
                    //Only need mask buffer
                    maskBuffer = RenderTexture.GetTemporary(Width, Height, 24, System.maskFormat);
                    break;
            }
        }
        private void ReleaseTextures()
        {
            //Depth
            if (depthBuffer != null && depthBuffer.IsCreated())
            {
                RenderTexture.ReleaseTemporary(depthBuffer);
                depthBuffer = null;

                if (depthEye != null && depthEye.IsCreated())
                {
                    RenderTexture.ReleaseTemporary(depthEye);
                    depthEye = null;
                }
            }

            //Normal
            if (normalBuffer != null && normalBuffer.IsCreated())
            {
                RenderTexture.ReleaseTemporary(normalBuffer);
                normalBuffer = null;

                if (normalEye != null && normalEye.IsCreated())
                {
                    RenderTexture.ReleaseTemporary(normalEye);
                    normalEye = null;
                }
            }

            //Mask
            if (maskBuffer != null && maskBuffer.IsCreated())
            {
                RenderTexture.ReleaseTemporary(maskBuffer);
                maskBuffer = null;

                if (maskEye != null && maskEye.IsCreated())
                {
                    RenderTexture.ReleaseTemporary(maskEye);
                    maskEye = null;
                }
            }
        }

        //Custom shader replacement
        private void UpdateShaderReplacement(Camera Source, DynamicDecals System)
        {
            if (System.ShaderReplacement)
            {
                //Grab and setup replacement camera
                Camera Renderer = System.CustomCamera;
                SetupReplacementCamera(Source, Renderer);

                //Render into textures
                if (VRCamera(Source) && System.Settings.SinglePassVR)
                {
                    //Left eye
                    if (Source.stereoTargetEye == StereoTargetEyeMask.Both || Source.stereoTargetEye == StereoTargetEyeMask.Left)
                    {
                        //Position
                        if (Source.transform.parent != null)
                        {
                            Renderer.transform.position = Source.transform.parent.TransformPoint(UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.LeftEye));
                        }
                        else
                        {
                            Renderer.transform.position = UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.LeftEye);
                        }
                        
                        //Rotation
                        Renderer.transform.rotation = Source.transform.rotation * UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.LeftEye);

                        //Projection matrix
                        Renderer.projectionMatrix = Source.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);

                        //View matrix
                        Renderer.worldToCameraMatrix = Source.worldToCameraMatrix;

                        //Render
                        RenderToTextures(Source, Renderer, System, depthEye, normalEye, maskEye);

                        //Blit into buffer
                        StereoBlit(Source, System, true);
                    }

                    //Right eye
                    if (Source.stereoTargetEye == StereoTargetEyeMask.Both || Source.stereoTargetEye == StereoTargetEyeMask.Right)
                    {
                        if (Source.transform.parent != null)
                        {
                            //Position
                            Renderer.transform.position = Source.transform.parent.TransformPoint(UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.RightEye));

                            //View matrix
                            Matrix4x4 worldToCamera = Source.worldToCameraMatrix;
                            worldToCamera.m03 -= Source.stereoSeparation * Source.transform.parent.localScale.x;
                            Renderer.worldToCameraMatrix = worldToCamera;
                        }
                        else
                        {
                            //Position
                            Renderer.transform.position = UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.RightEye);

                            //View matrix
                            Matrix4x4 worldToCamera = Source.worldToCameraMatrix;
                            worldToCamera.m03 -= Source.stereoSeparation;
                            Renderer.worldToCameraMatrix = worldToCamera;
                        }

                        //Rotation
                        Renderer.transform.rotation = Source.transform.rotation * UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.RightEye);

                        //Projection matrix
                        Renderer.projectionMatrix = Source.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);

                        //Render
                        RenderToTextures(Source, Renderer, System, depthEye, normalEye, maskEye);

                        //Blit into buffer
                        StereoBlit(Source, System, false);
                    }
                }
                else RenderToTextures(Source, Renderer, System, depthBuffer, normalBuffer, maskBuffer);
            }
        }

        private void RenderToTextures(Camera Source, Camera Renderer, DynamicDecals System, RenderTexture depth, RenderTexture normal, RenderTexture mask)
        {
            switch (replacement)
            {
                case ShaderReplacement.Classic:
                    //Render to mask buffer
                    Renderer.targetTexture = mask;
                    DrawSplitPass(Source, Renderer, System, System.MaskShader, false);
                    break;

                case ShaderReplacement.TriplePass:
                    //Set culling layers
                    Renderer.cullingMask = Source.cullingMask;

                    //Render to depth buffer
                    if (Source.actualRenderingPath == RenderingPath.DeferredShading)
                    {
                        Renderer.targetTexture = depth;
                        DrawRegualarPass(Renderer, System.DepthShader);
                    }

                    //Render to normal buffer
                    Renderer.targetTexture = normal;
                    DrawRegualarPass(Renderer, System.NormalShader);

                    //Render to mask buffer
                    Renderer.targetTexture = mask;
                    DrawSplitPass(Source, Renderer, System, System.MaskShader);
                    break;

                case ShaderReplacement.DoublePass:

                    //Render to depth buffer
                    if (Source.actualRenderingPath == RenderingPath.DeferredShading)
                    {
                        Renderer.cullingMask = Source.cullingMask;
                        Renderer.targetTexture = depth;
                        DrawRegualarPass(Renderer, System.DepthShader);
                    }

                    //Render to normal and mask buffers at once
                    RenderBuffer[] doublePassBuffers = new RenderBuffer[] { normal.colorBuffer, mask.colorBuffer };
                    Renderer.SetTargetBuffers(doublePassBuffers, normal.depthBuffer);
                    DrawSplitPass(Source, Renderer, System, System.NormalMaskShader);
                    break;

                case ShaderReplacement.SinglePass:
                    //Render to depth, normal and mask buffers at once
                    RenderBuffer[] singlePassBuffers = new RenderBuffer[] { mask.colorBuffer, normal.colorBuffer, depth.colorBuffer };
                    Renderer.SetTargetBuffers(singlePassBuffers, depth.depthBuffer);
                    DrawSplitPass(Source, Renderer, System, System.DepthNormalMaskShader);
                    break;

                case ShaderReplacement.SingleTarget:
                    //Render to packed buffer
                    Renderer.targetTexture = depth;
                    DrawSplitPass(Source, Renderer, System, System.DepthNormalMaskShader_Packed);
                    break;
            }

            //Tell camera to stop rendering to render texture/s
            Renderer.targetTexture = null;
        }
        private void DrawRegualarPass(Camera Renderer, Shader ReplacementShader)
        {
            //Clear before drawing
            Renderer.clearFlags = CameraClearFlags.SolidColor;
            Renderer.backgroundColor = Color.clear;

            //Render into temporary render texture
            Renderer.RenderWithShader(ReplacementShader, "RenderType");
        }
        private void DrawSplitPass(Camera Source, Camera Renderer, DynamicDecals System, Shader ReplacementShader, bool RenderInvalid = true)
        {
            //Grab mask passes
            List<ReplacementPass> passes = System.Settings.Passes;

            //Clear on the first pass
            Renderer.clearFlags = CameraClearFlags.SolidColor;
            Renderer.backgroundColor = Color.clear;

            //Render mask passes
            if (System.Settings.UseMaskLayers)
            {
                for (int i = 0; i < passes.Count; i++)
                {
                    if (passes[i].vector != Vector4.zero || RenderInvalid)
                    {
                        //Set culling mask
                        Renderer.cullingMask = passes[i].layers & Source.cullingMask;

                        //Set mask vector
                        Shader.SetGlobalVector("_MaskWrite", passes[i].vector);

                        //Render into temporary render texture/s
                        Renderer.RenderWithShader(ReplacementShader, "RenderType");

                        //Only clear on first render
                        Renderer.clearFlags = CameraClearFlags.Nothing;
                    }
                }
            }
            else if (RenderInvalid)
            {
                //Set culling mask
                Renderer.cullingMask = -1;

                //Set mask vector
                Shader.SetGlobalVector("_MaskWrite", Vector4.zero);

                //Render into temporary render texture/s
                Renderer.RenderWithShader(ReplacementShader, "RenderType");

                //Only clear on first render
                Renderer.clearFlags = CameraClearFlags.Nothing;
            }
        }

        private void StereoBlit(Camera Source, DynamicDecals System, bool Left)
        {
            //Blit from eye into buffer
            switch (replacement)
            {
                case ShaderReplacement.SingleTarget:
                    //Packed
                    Graphics.Blit(depthEye, depthBuffer, (Left) ? System.StereoBlitLeft : System.StereoBlitRight);
                    break;

                case ShaderReplacement.TriplePass:
                    //Depth (deferred only)
                    if (Source.actualRenderingPath == RenderingPath.DeferredShading)
                    {
                        Material depthBlit = (Left) ? System.StereoDepthBlitLeft : System.StereoDepthBlitRight;

                        depthBlit.SetTexture("_DepthTex", depthEye);
                        Graphics.Blit(depthEye, depthBuffer, depthBlit);
                    }
                    //Normal
                    Graphics.Blit(normalEye, normalBuffer, (Left) ? System.StereoBlitLeft : System.StereoBlitRight);
                    //Mask
                    Graphics.Blit(maskEye, maskBuffer, (Left) ? System.StereoBlitLeft : System.StereoBlitRight);
                    break;
            }
        }

        private void SetupReplacementCamera(Camera Source, Camera Target)
        {
            Target.CopyFrom(Source);
            Target.renderingPath = RenderingPath.Forward;
            Target.depthTextureMode = DepthTextureMode.None;
            Target.useOcclusionCulling = false;
            Target.allowMSAA = false;
            Target.allowHDR = false;
            Target.rect = new Rect(0, 0, 1, 1);
        }
        private void SetupReplacementCameraExperimental(Camera Source, Camera Target)
        {
            //The idea behind this was to avoid association with VR cameras by avoiding the CopyFrom(VRCamera)
            //Breaks more than it fixes

            //Physical position
            Target.transform.position = Source.transform.position;
            Target.transform.rotation = Source.transform.rotation;

            //Fov and ranges
            if (!UnityEngine.XR.XRSettings.enabled) Target.fieldOfView = Source.fieldOfView;
            Target.nearClipPlane = Source.nearClipPlane;
            Target.farClipPlane = Source.farClipPlane;
            Target.rect = new Rect(0, 0, 1, 1);

            //Orthographic support
            Target.orthographic = Source.orthographic;
            Target.orthographicSize = Source.orthographicSize;

            //Matrix
            Target.ResetProjectionMatrix();
            Target.ResetWorldToCameraMatrix();

            //Force forward rendering and disable extravagences
            Target.renderingPath = RenderingPath.Forward;
            Target.depthTextureMode = DepthTextureMode.None;
            Target.useOcclusionCulling = false;
            Target.allowMSAA = false;
            Target.allowHDR = false;
            Target.eventMask = 0;
        }
    }
}