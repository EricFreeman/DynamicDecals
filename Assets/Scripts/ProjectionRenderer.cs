using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LlockhamIndustries.Decals
{
    /**
    * This core component of the system. Attach this to empty gameObjects to create decals or omni-decals.
    */
    [ExecuteInEditMode]
    public class ProjectionRenderer : MonoBehaviour
    {
        //Properties
        /**
        * The instanced properties unique to this renderer. Each projection type will have different properties available to change. You should rarely need to get and set them as a group though.
        * This is usually only used to copy all the instanced properties from one projection to another. Remember to call UpdateProperties() once your done modifiying a renderers properties.
        */
        public ProjectionProperty[] Properties
        {
            get { return properties; }
            set
            {
                if (value != null) properties = (ProjectionProperty[])value.Clone();
                else properties = null;

                MarkProperties();
            }
        }
        /**
        * Resets the instanced properties to match the default values of the selected projection. If your making further immediate changes leave UpdateImmediately as false and remember to call UpdateProperties() once your done.
        * @param UpdateImmediately calls UpdateProperties() for you.
        */
        public void ResetProperties(bool UpdateImmediately = false)
        {
            //Reset tiling / offset
            tiling = Vector2.one;
            offset = Vector2.zero;

            //Reset masking 
            maskMethod = MaskMethod.DrawOnEverythingExcept;
            masks = new bool[4];

            //Reset properties
            if (projection != null) properties = (ProjectionProperty[])projection.Properties.Clone();
            else properties = null;

            MarkProperties(UpdateImmediately);
        }
        /**
        * Applies any changes made to the instanced properties of the renderer. Tiling, Offset, Masking & Projection specific properties all count as instanced properties.
        * This should be called once after your done making all your changes to these properties, not once per property.
        */
        public void UpdateProperties()
        {
            if (meshRenderer != null)
            {
                //Update our property block
                if (marked && Projection != null)
                {
                    UpdateRendererBlock(Properties, Projection.Properties);
                    meshRenderer.SetPropertyBlock(block);

                    //No longer needs to be updated
                    marked = false;
                }
            }
        }

        public void MarkProperties(bool UpdateImmediately = false)
        {
            marked = true;
            if (UpdateImmediately && meshRenderer != null) UpdateProperties();
        }

        /**
        * Modifies the float value (Such as emission strength) of a renderers projection specific properties.
        * Remember to call UpdateProperties() once your done modifiying a renderers properties.
        * @param PropertyIndex The index of the property being modified. If your not sure of the index look at the UI of your projectionRenderer, the instanced properties appear here in order. In all cases 0 is the albedo color and it's alpha is used for transparency.
        * @param Float The value you wish to set the property too.
        */
        public void SetFloat(int PropertyIndex, float Float)
        {
            if ((properties[PropertyIndex].type == PropertyType.Float || properties[PropertyIndex].type == PropertyType.Combo) && properties[PropertyIndex].value != Float)
            {
                //Adjust property
                properties[PropertyIndex].value = Float;
                properties[PropertyIndex].enabled = true;

                //Mark renderers to update
                MarkProperties();
            }
        }
        /**
        * Modifies the color value (Such as emission color) of a renderers projection specific properties.
        * Remember to call UpdateProperties() once your done modifiying a renderers properties.
        * @param PropertyIndex The index of the property being modified. If your not sure of the index look at the UI of your projectionRenderer, the instanced properties appear here in order. In all cases 0 is the albedo color and it's alpha is used for transparency.
        * @param Color The value you wish to set the property too.
        */
        public void SetColor(int PropertyIndex, Color Color)
        {
            if ((properties[PropertyIndex].type == PropertyType.Color || properties[PropertyIndex].type == PropertyType.Combo) && properties[PropertyIndex].color != Color)
            {
                //Adjust property
                properties[PropertyIndex].color = Color;
                properties[PropertyIndex].enabled = true;

                //Mark renderers to update
                MarkProperties();
            }
        }

        //Tiling & Offset
        /**
        * The tiling of your projection. This can be used to sample a specific part of your projection (below 1), or have your projection tile (above 1).
        * Remember to call UpdateProperties() once your done modifiying a renderers properties.
        */
        public Vector2 Tiling
        {
            get { return tiling; }
            set
            {
                if (tiling != value)
                {
                    //Adjust property
                    tiling = value;

                    //Mark renderers to update
                    MarkProperties();
                }
            }
        }
        /**
        * The offset of your projection. This allows you to offset where you begin sampling your projection.
        * Remember to call UpdateProperties() once your done modifiying a renderers properties.
        */
        public Vector2 Offset
        {
            get { return offset; }
            set
            {
                if (offset != value)
                {
                    //Adjust property
                    offset = value;

                    //Mark renderers to update
                    MarkProperties();
                }
            }
        }

        //Masking
        /**
        * Defines which masking method we should apply to this projection. Either "DrawOnEverythingExcept" or "OnlyDrawOn".
        * Draw On Everything Except - will draw on all surface except those in the selected mask layers.
        * Only Draw On - will only draw on surfaces that are part  of the selected mask layers.
        */
        public MaskMethod MaskMethod
        {
            get { return maskMethod; }
            set
            {
                if (maskMethod != value)
                {
                    //Adjust property
                    maskMethod = value;
                    
                    //Mark renderers to update
                    MarkProperties();
                }
            }
        }
        /**
        * Defines whether this projection is affected by the first masking layer. 
        * To add surfaces to this mask layer add a Mask component to a renderable gameObject and toggle on the appropriate mask layer.
        */
        public bool MaskLayer1
        {
            get { return masks[0]; }
            set
            {
                if (masks[0] != value)
                {
                    //Adjust property
                    masks[0] = value;
                    
                    //Mark renderers to update
                    MarkProperties();
                }
            }
        }
        /**
        * Defines whether this projection is affected by the second masking layer.
        * To add surfaces to this mask layer add a Mask component to a renderable gameObject and toggle on the appropriate mask layer.
        */
        public bool MaskLayer2
        {
            get { return masks[1]; }
            set
            {
                if (masks[1] != value)
                {
                    //Adjust property
                    masks[1] = value;
                    
                    //Mark renderers to update
                    MarkProperties();
                }
            }
        }
        /**
        * Defines whether this projection is affected by the third masking layer.
        * To add surfaces to this mask layer add a Mask component to a renderable gameObject and toggle on the appropriate mask layer.
        */
        public bool MaskLayer3
        {
            get { return masks[2]; }
            set
            {
                if (masks[2] != value)
                {
                    //Adjust property
                    masks[2] = value;

                    //Mark renderers to update
                    MarkProperties();
                }
            }
        }
        /**
        * Defines whether this projection is affected by the fourth masking layer.
        * To add surfaces to this mask layer add a Mask component to a renderable gameObject and toggle on the appropriate mask layer.
        */
        public bool MaskLayer4
        {
            get { return masks[3]; }
            set
            {
                if (masks[3] != value)
                {
                    //Adjust property
                    masks[3] = value;

                    //Mark renderers to update
                    MarkProperties();
                }
            }
        }

        //Projection
        /**
        * The projection your renderer will render.
        * Changing the projection will reset all of your projections per-instance properties to the default value of the new projection.
        */
        public Projection Projection
        {
            get
            {
                if (gameObject.activeInHierarchy && active != null) return active;
                else return projection;
            }
            set
            {
                if (projection != value)
                {
                    projection = value;
                    ChangeProjection();
                }
            }
        }
        public void ChangeProjection()
        {
            if (active != projection)
            {
                //Deregister old projection
                if (gameObject.activeInHierarchy && enabled && active != null) Deregister();

                //Set new projection
                active = projection;

                //Register new projection
                if (gameObject.activeInHierarchy && enabled && active != null) Register();

                //Reset tiling / offset
                tiling = Vector2.one;
                offset = Vector2.zero;

                //Get new properties
                if (active != null) Properties = active.Properties;
                else Properties = null;
            }
            
            //Assign Projection to renderer
            UpdateProjection();
        }
        public void UpdateProjection()
        {
            if (meshRenderer != null)
            {
                if (Projection != null && Projection.Valid)
                {
                    //Enable renderer
                    meshRenderer.gameObject.SetActive(true);

                    //Apply materials
                    meshRenderer.sharedMaterial = Projection.Mat;

                    //Reset properties
                    UpdateRendererBlock(Properties, Projection.Properties);
                    meshRenderer.SetPropertyBlock(block);

                    //No longer needs to be updated
                    marked = false;
                }
                else
                {
                    //Disable renderer
                    meshRenderer.gameObject.SetActive(false);

                    //Remove materials
                    meshRenderer.sharedMaterial = null;

                    //Reset properties
                    if (block != null) block.Clear();
                    meshRenderer.SetPropertyBlock(block);
                }
            }
        }

        private bool Register()
        {
            if (this != null)
            {
                #if UNITY_EDITOR
                PrefabType prefabType = PrefabUtility.GetPrefabType(gameObject);
                if (prefabType == PrefabType.ModelPrefab || prefabType == PrefabType.Prefab) return false;
                #endif

                return DynamicDecals.System.Register(this);
            }
            return false;
        }
        private void Deregister()
        {
            if (this != null)
            {
                #if UNITY_EDITOR
                PrefabType prefabType = PrefabUtility.GetPrefabType(gameObject);
                if (prefabType == PrefabType.ModelPrefab || prefabType == PrefabType.Prefab) return;
                #endif

                DynamicDecals.System.Deregister(this);
            }
        }

        //Renderer
        public MeshRenderer Renderer
        {
            get { return meshRenderer; }
        }

        private MaterialPropertyBlock block;
        private bool marked = true;

        public void InitializeRenderer(bool Active)
        {
            //Check if we already have a forward renderer
            if (meshRenderer == null)
            {
                foreach (Transform child in transform)
                {
                    if (child.name == "Renderer")
                    {
                        meshRenderer = child.GetComponent<MeshRenderer>();
                        break;
                    }
                }
            }

            //If none could be found, create one
            if (meshRenderer == null)
            {
                //Create renderer
                GameObject go = new GameObject("Renderer");
                go.transform.SetParent(transform, false);
                go.layer = gameObject.layer;

                //Hide renderer
                go.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor | HideFlags.HideInHierarchy;

                //Setup mesh filter
                MeshFilter filter = go.AddComponent<MeshFilter>();
                filter.sharedMesh = DynamicDecals.System.Cube;

                //Setup mesh renderer
                meshRenderer = go.AddComponent<MeshRenderer>();
                meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.Camera;
                meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;

                
                //Hide editor outline
                #if UNITY_EDITOR
                EditorUtility.SetSelectedRenderState(meshRenderer, EditorSelectedRenderState.Hidden);
                #endif
            }

            //Make sure renderer is enabled
            meshRenderer.gameObject.SetActive(Active);
        }
        public void TerminateRenderer()
        {
            meshRenderer.gameObject.SetActive(false);
        }

        private void UpdateRendererBlock(ProjectionProperty[] Local, ProjectionProperty[] Global)
        {
            //Initialize / Clear block
            if (block == null) block = new MaterialPropertyBlock();
            else block.Clear();

            //Update tiling / offset
            block.SetVector(_TilingOffset, new Vector4(Tiling.x, Tiling.y, Offset.x, Offset.y));

            //Masking
            switch (maskMethod)
            {
                case MaskMethod.DrawOnEverythingExcept:
                    block.SetFloat(_MaskBase, 1);

                    Color except = Color.clear;
                    except.r = (masks[0]) ? 0 : 0.5f;
                    except.g = (masks[1]) ? 0 : 0.5f;
                    except.b = (masks[2]) ? 0 : 0.5f;
                    except.a = (masks[3]) ? 0 : 0.5f;

                    block.SetVector(_MaskLayers, except);
                    break;

                case MaskMethod.OnlyDrawOn:
                    block.SetFloat(_MaskBase, 0);

                    Color only = Color.clear;
                    only.r = (masks[0]) ? 1 : 0.5f;
                    only.g = (masks[1]) ? 1 : 0.5f;
                    only.b = (masks[2]) ? 1 : 0.5f;
                    only.a = (masks[3]) ? 1 : 0.5f;

                    block.SetVector(_MaskLayers, only);
                    break;
            }

            //Update and apply our material property block
            for (int i = 0; i < Local.Length; i++)
            {
                if (Local[i].type == PropertyType.Float)
                {
                    float value = (Local[i].enabled) ? Local[i].value : Global[i].value;
                    block.SetFloat(Global[i].nameID, value);
                }
                if (Local[i].type == PropertyType.Color)
                {
                    Color color = (Local[i].enabled) ? Local[i].color : Global[i].color;
                    block.SetColor(Global[i].nameID, color);
                }
                if (Local[i].type == PropertyType.Combo)
                {
                    Color color = (Local[i].enabled) ? Local[i].color * Local[i].value : Global[i].color * Global[i].value;
                    block.SetColor(Global[i].nameID, color);
                }
            }
        }

        //Enable / Disable
        private void OnEnable()
        {
            //Grab our IDs
            _TilingOffset = Shader.PropertyToID("_TilingOffset");
            _MaskBase = Shader.PropertyToID("_MaskBase");
            _MaskLayers = Shader.PropertyToID("_MaskLayers");

            Initialize();
        }
        private void OnDisable()
        {
            Terminate();
        }

        private void Initialize()
        {
            if (projection != null)
            {
                //Set new projection
                active = projection;

                //Register 
                bool registeredState = Register();

                //Initialize renderer
                InitializeRenderer(registeredState);
            }
            else InitializeRenderer(false);

            //Assign Projection to renderer
            UpdateProjection();
        }
        private void Terminate()
        {
            //Deregister projection
            if (projection != null) Deregister();

            //Terminate renderer
            TerminateRenderer();
        }

        //Sub-Order
        public ProjectionData Data
        {
            get { return data; }
            set { data = value; }
        }

        /**
        * Each projection is rendered before or after other projections. Each renderer has a sub-order within it's projection.
        * MoveToTop moves this renderer in front of all other renderers rendering the same projection.
        * Projection must be active (enabled) for this to have any effect.
        */
        public void MoveToTop()
        {
            if (data != null)
            {
                data.MoveToTop(this);
            }
        }
        /**
        * Each projection is rendered before or after other projections. Each renderer has a sub-order within it's projection.
        * MoveToBottom moves this renderer behind all other renderers rendering the same projection.
        * Projection must be active (enabled) for this to have any effect.
        */
        public void MoveToBottom()
        {
            if (data != null)
            {
                data.MoveToBottom(this);
            }
        }

        //Pooling
        /**
        * The projection pool your renderer belongs too. Will return null if the renderer is not currently in any pool.
        */
        public ProjectionPool Pool
        {
            get
            {
                if (poolItem != null) return poolItem.Pool;
                else return null;
            }
        }
        public PoolItem PoolItem
        {
            get { return poolItem; }
            set { poolItem = value; }
        }

        /**
        * Checks to see how much a point is intersecting with the projection bounds.
        * The closer the point is to the center of the projection bounds, the higher the returned value will be.
        * A perfectly intersecting point (ie. At the centre of the projection bounds) will return 1, while a non-intersecting point will return 0.
        * We can use this method to cheaply determine how much overlap is occuring between projections, or to see if some other object would be projected onto and act accordingly.
        * @param Point defines the point (in world space) to check.
        */
        public float CheckIntersecting(Vector3 Point)
        {
            Vector3 localPoint = transform.InverseTransformPoint(Point);
            return Mathf.Clamp01(2 * (0.5f - Mathf.Max(Mathf.Max(Mathf.Abs(localPoint.x), Mathf.Abs(localPoint.y)), Mathf.Abs(localPoint.z))));
            
        }

        //Destroy
        /**
        * Use this instead of GameObject.Destroy().
        * If the renderer is part of a pool returns it back to the pool so it can be used again. Otherwise Destroys the object.
        */
        public void Destroy()
        {
            if (poolItem != null) poolItem.Return();
            else Destroy(gameObject);
        }

        #region Backing Fields
        [SerializeField]
        private Projection projection;
        private Projection active;

        [SerializeField]
        private ProjectionProperty[] properties;

        [SerializeField]
        private Vector2 tiling;
        [SerializeField]
        private Vector2 offset;

        [SerializeField]
        protected MaskMethod maskMethod;
        [SerializeField]
        protected bool[] masks = new bool[4];

        //Renderers
        private MeshRenderer meshRenderer;

        //Sub-Order
        private ProjectionData data;

        //Pooling
        private PoolItem poolItem;
        
        //Property Ids
        public int _TilingOffset;
        public int _MaskBase;
        public int _MaskLayers;
        #endregion
        #region Editor
        #if UNITY_EDITOR
        //Duplicate check
        [SerializeField]
        private int instanceID = 0;
        private void Awake()
        {
            if (!Application.isPlaying)
            {
                if (instanceID == 0)
                {
                    instanceID = GetInstanceID();
                }
                else if (instanceID != GetInstanceID())
                {
                    //Duplicate detected
                    instanceID = GetInstanceID();

                    //Clone properties
                    properties = (ProjectionProperty[])properties.Clone();
                }
            }
        }

        //Gizmo type
        protected virtual SelectionGizmo Gizmo
        {
            get
            {
                if (projection != null)
                {
                    switch (projection.ProjectionType)
                    {
                        case ProjectionType.Decal:
                            return SelectionGizmo.Cube;
                        case ProjectionType.OmniDecal:
                            return SelectionGizmo.Sphere;
                    }
                }
                return SelectionGizmo.Cube;
            }
        }
        protected enum SelectionGizmo { Cube, Sphere };

        //Gizmo rendering
        public void OnDrawGizmos()
        {
            DrawGizmo(false);
        }
        public void OnDrawGizmosSelected()
        {
            DrawGizmo(true);
        }
        private void DrawGizmo(bool Selected)
        {
            if (isActiveAndEnabled)
            {
                //Decalare color and matrix
                Color color = new Color(0.8f, 0.8f, 0.8f, 1.0f);
                Gizmos.matrix = transform.localToWorldMatrix;

                //Draw selection gizmo
                switch (Gizmo)
                {
                    case SelectionGizmo.Cube:
                        if (!Selected)
                        {
                            color.a = 0.0f;
                            Gizmos.color = color;
                            Gizmos.DrawCube(Vector3.zero, Vector3.one);
                        }
                        
                        color.a = Selected ? 0.5f : 0.05f;
                        Gizmos.color = color;
                        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                        Gizmos.DrawLine(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, -0.5f));
                        Gizmos.DrawLine(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(-0.5f, 0.5f, -0.5f));
                        break;
                    case SelectionGizmo.Sphere:
                        if (!Selected)
                        {
                            color.a = 0.0f;
                            Gizmos.color = color;
                            Gizmos.DrawSphere(Vector3.zero, 1);
                        }

                        color.a = Selected ? 0.5f : 0.05f;
                        Gizmos.color = color;
                        Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
                        break;
                }
            }
        }
        #endif
        #endregion
    }

    [System.Serializable]
    public struct ProjectionProperty
    {
        public string name;
        public int nameID;
        public PropertyType type;
        
        public Color color;
        public float value;
        public bool enabled;

        //Constructors
        public ProjectionProperty(string Name, int ID, Color Color)
        {
            name = Name;
            nameID = ID;

            type = PropertyType.Color;
            color = Color;
            value = 0;

            enabled = false;
        }
        public ProjectionProperty(string Name, int ID, float Value)
        {
            name = Name;
            nameID = ID;

            type = PropertyType.Float;
            color = Color.white;
            value = Value;

            enabled = false;
        }
        public ProjectionProperty(string Name, int ID, Color Color, float Value)
        {
            name = Name;
            nameID = ID;

            type = PropertyType.Combo;
            color = Color;
            value = Value;

            enabled = false;
        }
    }
    public enum PropertyType { Color, Float, Combo }
}