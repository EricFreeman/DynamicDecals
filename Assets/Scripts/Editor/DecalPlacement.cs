using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using LlockhamIndustries.ExtensionMethods;

namespace LlockhamIndustries.Decals
{
    public class DecalPlacement : EditorWindow
    {
        [MenuItem("Window/Decals/Placement")]
        private static void Init()
        {
            DecalPlacement window = (DecalPlacement)GetWindow(typeof(DecalPlacement));
            window.titleContent = new GUIContent("Decals");
            window.minSize = new Vector2(300, 180);
            window.Show();
        }

        //ScrollView
        private Vector2 scrollPosition;

        //Projections
        public ProjectionRenderer[] prints = new ProjectionRenderer[1];
        public LayerMask[] printLayers;
        public string[] printTags;

        //Selection method
        public PrintSelection printMethod;

        //Parents
        public PrintParent parent;
        public Transform defaultParent;

        //Size
        public float min = 0.5f;
        public float max = 1.5f;

        private bool drawing;
        private float timeElapsed;

        private Object[] empty = new Object[0];

        private void OnEnable()
        {
            //Register undo/redo callback
            Undo.undoRedoPerformed += UndoRedo;

            //Register onSceneGUI
            SceneView.onSceneGUIDelegate += OnSceneGUI;
        }
        private void OnDisable()
        {
            //De-register undo/redo callback
            Undo.undoRedoPerformed -= UndoRedo;

            //De-register onSceneGUI
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
        }

        private void OnGUI()
        {
            //Grab our settings
            DynamicDecalSettings settings = DynamicDecals.System.Settings;

            //Calculate required rect height
            float settingsHeight = (prints.Length * 18) + 180;
            float totalHeight = LlockhamEditorUtility.TabHeight * (settings.pools.Length + 4) + settingsHeight;

            //Begin change check & scrollView
            EditorGUI.BeginChangeCheck();
            Rect scrollRect = new Rect(0, 0, position.width - 20, totalHeight);
            scrollPosition = GUI.BeginScrollView(new Rect(10, 10, position.width - 20, position.height - 20), scrollPosition, scrollRect, GUIStyle.none, GUIStyle.none);

            //General settings
            Settings(new Rect(0, 0, scrollRect.width, settingsHeight));

            //Drawing toggle
            DrawToggle(new Rect((position.width - 200) / 2, settingsHeight + 20, 200, 20), "Toggle Draw");

            //End change check & scrollView
            GUI.EndScrollView();
            if (EditorGUI.EndChangeCheck())
            {
                
            }
        }
        private void UndoRedo()
        {
            //Grab our settings
            DynamicDecalSettings settings = DynamicDecals.System.Settings;

            //Recalculate passes
            settings.CalculatePasses();

            //Update renderers
            DynamicDecals.System.UpdateRenderers();

            //Repaint the window to show changes immediately
            Repaint();
        }
        private void OnSceneGUI(SceneView sceneview)
        {
            if (drawing && focusedWindow == sceneview)
            {
                Ray ray = sceneview.camera.ScreenPointToRay(Event.current.mousePosition);
                ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 255, QueryTriggerInteraction.Ignore))
                {
                    //Handle
                    Handles.DrawWireDisc(hit.point, hit.normal, 1);

                    //Left click
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        Draw(hit.point, Quaternion.LookRotation(-hit.normal), hit.collider.transform, hit.collider.gameObject.layer);
                        Event.current.Use();
                    }
                }

                //No selection
                Selection.objects = empty;
            }

            //Repaint
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }

        private void Settings(Rect Rect)
        {
            GUILayout.BeginArea(Rect);

            PrintGUI();
            SizeGUI();

            ParentGUI();

            GUILayout.EndArea();
        }

        protected void PrintGUI()
        {
            //Top Space
            EditorGUILayout.Space();

            //Header
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Projections", "The possible Projections to print & the method used to select amongst them."), GUILayout.MaxWidth(120));
            GUILayout.FlexibleSpace();
            int printSize = EditorGUILayout.IntSlider(new GUIContent("", "The number of projections available to print"), prints.Length, 1, 10, GUILayout.MaxWidth(120));
            if (printLayers == null || printLayers.Length != printSize)
            {
                if (printLayers == null) printLayers = new LayerMask[printSize];
                else printLayers = printLayers.Resize(printSize);
            }
            if (printTags == null || printTags.Length != printSize)
            {
                if (printTags == null) printTags = new string[printSize];
                else printTags = printTags.Resize(printSize);
            }
            if (prints == null || prints.Length != printSize)
            {
                if (prints == null) prints = new ProjectionRenderer[printSize];
                else prints = prints.Resize(printSize);
            }
            EditorGUILayout.EndHorizontal();

            //Body
            EditorGUI.indentLevel++;
            //Selection method is only relevant if theres more than 1 print to choose from
            if (prints.Length > 1)
            {
                printMethod = (PrintSelection)EditorGUILayout.EnumPopup(new GUIContent("Selection Method"), printMethod);
                EditorGUILayout.Space();
            }

            //Prints
            for (int i = 0; i < prints.Length; i++)
            {
                if (prints.Length > 1 && printLayers.Length > 1 && printMethod == PrintSelection.Layer)
                {
                    EditorGUILayout.BeginHorizontal();
                    prints[i] = (ProjectionRenderer)EditorGUILayout.ObjectField(new GUIContent("", "Projection to print"), prints[i], typeof(ProjectionRenderer), false);
                    printLayers[i] = EditorGUILayout.LayerField(new GUIContent("", "Layer to print on"), printLayers[i], GUILayout.Width(100));
                    EditorGUILayout.EndHorizontal();
                }
                else if (prints.Length > 1 && printLayers.Length > 1 && printMethod == PrintSelection.Tag)
                {
                    EditorGUILayout.BeginHorizontal();
                    prints[i] = (ProjectionRenderer)EditorGUILayout.ObjectField(new GUIContent("", "Projection to print"), prints[i], typeof(ProjectionRenderer), false);
                    if (i == 0)
                    {
                        EditorGUILayout.LabelField(new GUIContent("Default", "Tag to print on"), GUILayout.Width(100));
                    }
                    else
                    {
                        printTags[i] = EditorGUILayout.TagField(new GUIContent("", "Tag to print on"), printTags[i], GUILayout.Width(100));
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    prints[i] = (ProjectionRenderer)EditorGUILayout.ObjectField(new GUIContent("", "Projection to print"), prints[i], typeof(ProjectionRenderer), false);
                }
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
        protected void ParentGUI()
        {
            EditorGUILayout.LabelField(new GUIContent("Parent", "The transform to attach the prints to"));
            EditorGUI.indentLevel++;
            parent = (PrintParent)EditorGUILayout.EnumPopup(new GUIContent("", "The transform to attach the prints to"), parent);
            if (parent == PrintParent.Default) defaultParent = (Transform)EditorGUILayout.ObjectField(GUIContent.none, defaultParent, typeof(Transform), true);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
        protected void SizeGUI()
        {
            EditorGUILayout.LabelField(new GUIContent("Size", "Size modifier"));
            EditorGUI.indentLevel++;
            min = EditorGUILayout.Slider(new GUIContent("Min", "The minimum size"), min, 0, 4);
            max = EditorGUILayout.Slider(new GUIContent("Max", "The maximum size"), max, 0, 4);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
        protected void DrawToggle(Rect Rect, string Title)
        {
            if (GUI.Button(Rect, Title)) drawing = !drawing;
        }

        private void Draw(Vector3 Position, Quaternion Rotation, Transform Surface, int Layer = 0)
        {
            //Projection Check
            if (prints == null || prints.Length < 1)
            {
                Debug.LogError("No Projections to print. Please set at least one projection to print.");
                return;
            }

            //Print using print method
            switch (printMethod)
            {
                case PrintSelection.Layer:
                    if (printLayers == null || printLayers.Length == 0)
                    {
                        DrawProjection(prints[0], Position, Rotation, Surface);
                    }
                    else
                    {
                        //If the layer mask contains the hit layer, print the decal associated with it.
                        for (int i = 0; i < printLayers.Length; i++)
                        {
                            if (printLayers[i] == (printLayers[i] | (1 << Layer))) DrawProjection(prints[i], Position, Rotation, Surface);
                        }
                    }
                    break;
                case PrintSelection.Tag:
                    if (printLayers == null || printLayers.Length == 0)
                    {
                        DrawProjection(prints[0], Position, Rotation, Surface);
                    }
                    else
                    {
                        bool printed = false;
                        //If the surface is of the tag, print the decal associated with it.
                        for (int i = 1; i < printTags.Length; i++)
                        {
                            if (printTags[i] == Surface.tag)
                            {
                                DrawProjection(prints[i], Position, Rotation, Surface);
                                printed = true;
                            }
                        }

                        //If the surface has no relevant tag, print the default print.
                        if (!printed)
                        {
                            DrawProjection(prints[0], Position, Rotation, Surface);
                        }
                    }
                    break;
                case PrintSelection.Random:
                    //Generate an int between one and the prints length
                    int index = Random.Range(0, prints.Length);
                    //Print the projection at that index
                    DrawProjection(prints[index], Position, Rotation, Surface);
                    break;
                case PrintSelection.All:
                    //Print each projection once
                    foreach (ProjectionRenderer projection in prints)
                    {
                        DrawProjection(projection, Position, Rotation, Surface);
                    }
                    break;
            }
        }
        private void DrawProjection(ProjectionRenderer Projection, Vector3 Position, Quaternion Rotation, Transform Surface)
        {
            if (Projection != null)
            {
                //Instantiate projection
                ProjectionRenderer proj = Instantiate(Projection);
                proj.name = Projection.name;

                Undo.RegisterCreatedObjectUndo(proj.gameObject, "Place Projection");

                //Get randomized scale
                float scaleMod = Random.Range(min, max);

                //Set Transform Data
                proj.transform.position = Position;
                proj.transform.rotation = Rotation;
                proj.transform.localScale *= scaleMod;

                //Set Parent
                if (parent == PrintParent.Surface)
                {
                    //Create a sub parent
                    //Fixes Non-Uniform scaling and is generally cleaner
                    Transform subParent = null;
                    foreach (Transform child in Surface)
                    {
                        if (child.name == "Projections") subParent = child;
                    }
                    if (subParent == null)
                    {
                        subParent = new GameObject("Projections").transform;
                        subParent.SetParent(Surface);
                    }
                    proj.transform.SetParent(subParent);
                }
                else if (defaultParent != null)
                {
                    proj.transform.SetParent(defaultParent);
                }
            }
        }
    }
}