using UnityEngine;
using UnityEditor;
using System.Collections;

namespace LlockhamIndustries.Decals
{
    [CustomEditor(typeof(Projection))]
    public abstract class ProjectionEditor : Editor
    {
        //Type
        protected SerializedProperty type;

        //Instanced
        protected SerializedProperty instanced;

        //Force Forward
        protected SerializedProperty forceForward;

        //Priority
        protected SerializedProperty priority;

        //Transparency
        protected SerializedProperty transparencyType;
        protected SerializedProperty cutoff;

        //Masking
        protected SerializedProperty maskMethod;
        protected SerializedProperty masks;

        //Projection Limit
        protected SerializedProperty projectionLimit;

        //Drawers
        protected PropertyGroupDrawer[] propertyGroups;

        public void Mark()
        {
            //Apply changes
            serializedObject.ApplyModifiedProperties();

            //Update projection
            for (int i = 0; i < serializedObject.targetObjects.Length; i++)
            {
                ((Projection)serializedObject.targetObjects[i]).Mark(true);
            }

            //Repaint scene
            SceneView.RepaintAll();
        }
        public void Reorder()
        {
            //Apply changes
            serializedObject.ApplyModifiedProperties();

            //Reorder projection
            foreach (UnityEngine.Object projection in serializedObject.targetObjects)
            {
                DynamicDecals.System.Reorder(projection as Projection);
            }

            //Repaint scene
            SceneView.RepaintAll();
        }
        protected void MarkAllPreviews()
        {
            if (propertyGroups != null)
            {
                for (int i = 0; i < propertyGroups.Length; i++) propertyGroups[i].Mark();
            }
        }

        public virtual void OnEnable()
        {
            //Register to undo redo
            Undo.undoRedoPerformed += OnUndoRedo;

            if (target != null)
            {
                //Grab our properties
                type = serializedObject.FindProperty("type");
                instanced = serializedObject.FindProperty("instanced");
                forceForward = serializedObject.FindProperty("forceForward");
                priority = serializedObject.FindProperty("priority");
                transparencyType = serializedObject.FindProperty("transparencyType");
                cutoff = serializedObject.FindProperty("cutoff");
                maskMethod = serializedObject.FindProperty("maskMethod");
                masks = serializedObject.FindProperty("masks");
                projectionLimit = serializedObject.FindProperty("projectionLimit");
            }
        }
        public virtual void OnDisable()
        {
            //Dergister from undo redo
            Undo.undoRedoPerformed -= OnUndoRedo;

            //Terminate property drawers
            if (propertyGroups != null)
            {
                for (int i = 0; i < propertyGroups.Length; i++) propertyGroups[i].Terminate();
            }
        }

        private void OnUndoRedo()
        {
            //Update previews
            MarkAllPreviews();

            //Require delayed update
            DelayedMark();

            //Repaint editor
            Repaint();
        }
        private void DelayedMark()
        {
            delayedMark = true;
        }
        private bool delayedMark;

        protected void Type()
        {
            Rect Rect = GUILayoutUtility.GetRect(0, 22 + LlockhamEditorUtility.Spacing);
            EditorGUI.DrawRect(new Rect(Rect.x, Rect.y + 2, Rect.width, Rect.height - 6), LlockhamEditorUtility.MidgroundColor);
            GUI.BeginGroup(Rect);

            //Header
            EditorGUI.LabelField(new Rect(0, 4, Rect.width * 0.4f, 16), new GUIContent("Type", "Traditional decals project 2D images in a single direction. Omni decals project 1D gradiants in every direction at once."), EditorStyles.boldLabel);

            //Properties
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(new Rect((Rect.width * 0.4f) + 8, 4, (Rect.width * 0.6f) - 12, 16), type, new GUIContent(""));
            if (EditorGUI.EndChangeCheck()) Mark();

            GUI.EndGroup();
        }
        protected void Priority(int PriorityLimit = 100)
        {
            Rect Rect = GUILayoutUtility.GetRect(0, 22 + LlockhamEditorUtility.Spacing);
            EditorGUI.DrawRect(new Rect(Rect.x, Rect.y + 2, Rect.width, Rect.height - 6), LlockhamEditorUtility.MidgroundColor);
            GUI.BeginGroup(Rect);

            //Header
            EditorGUI.LabelField(new Rect(0, 4, Rect.width * 0.4f, 16), new GUIContent("Priority", "Determines whether this projection will be drawn over others"), EditorStyles.boldLabel);
            
            //Properties
            EditorGUI.BeginChangeCheck();
            priority.intValue = EditorGUI.IntSlider(new Rect((Rect.width * 0.4f) + 8, 4, (Rect.width * 0.6f) - 12, 16), new GUIContent(""), priority.intValue, 0, PriorityLimit);
            if (EditorGUI.EndChangeCheck()) Reorder();

            GUI.EndGroup();
        }
        protected void Transparency(bool Supported = true)
        {
            if (Supported)
            {
                Rect Rect = GUILayoutUtility.GetRect(0, ((transparencyType.enumValueIndex == 0) ? 42 : 22) + LlockhamEditorUtility.Spacing);
                EditorGUI.DrawRect(new Rect(Rect.x, Rect.y + 2, Rect.width, Rect.height - 6), LlockhamEditorUtility.MidgroundColor);
                GUI.BeginGroup(Rect);

                //Header
                EditorGUI.LabelField(new Rect(0, 4, Rect.width * 0.4f, 16), new GUIContent("Transparency", "Determines where on the surface to Draw."), EditorStyles.boldLabel);

                //Properties
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(new Rect((Rect.width * 0.4f) + 8, 4, (Rect.width * 0.6f) - 12, 16), transparencyType, new GUIContent(""));
                if (EditorGUI.EndChangeCheck())
                {
                    //Adjust cutoff
                    if (transparencyType.enumValueIndex == 0)
                    {
                        cutoff.floatValue = 0.2f;
                    }
                    else cutoff.floatValue = 0.01f;

                    //Apply changes
                    Mark();
                    MarkAllPreviews();
                }
                if (transparencyType.enumValueIndex == 0)
                {
                    EditorGUI.BeginChangeCheck();
                    cutoff.floatValue = EditorGUI.Slider(new Rect(4, 20, Rect.width - 8, 16), new GUIContent(""), cutoff.floatValue, 0, 1);
                    if (EditorGUI.EndChangeCheck())
                    {
                        //Apply changes
                        Mark();
                        MarkAllPreviews();
                    }
                }
                GUI.EndGroup();
            }
            else
            {
                if (cutoff.floatValue != 0)
                {
                    //Adjust cutoff
                    cutoff.floatValue = 0;
                    
                    //Apply changes
                    Mark();
                    MarkAllPreviews();
                }
            }
            
        }
        protected void Masking()
        {
            if (DynamicDecals.System.Settings.UseMaskLayers)
            {
                Rect Rect = GUILayoutUtility.GetRect(0, 62 + LlockhamEditorUtility.Spacing);
                EditorGUI.DrawRect(new Rect(Rect.x, Rect.y + 2, Rect.width, Rect.height - 6), LlockhamEditorUtility.MidgroundColor);
                GUI.BeginGroup(Rect);

                //Header
                EditorGUI.LabelField(new Rect(0, 4, 60, 16), new GUIContent("Masking", "Determines which objects the projection can be drawn on."), EditorStyles.boldLabel);

                //Properties
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(new Rect(80, 5, Rect.width - 84, 16), maskMethod, new GUIContent(""));
                for (int i = 0; i < masks.arraySize; i++)
                {
                    Rect rect = new Rect((i == 0 || i == 2) ? 4 : (Rect.width / 2) + 4, (i < 2) ? 22 : 40, (Rect.width / 2) - 16, 16);
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 8, rect.height), new GUIContent(DynamicDecals.System.Settings.Layers[i].name, ""), LlockhamEditorUtility.MiniLabel);
                    masks.GetArrayElementAtIndex(i).boolValue = EditorGUI.Toggle(new Rect(rect.xMax - 8, rect.y, 14, rect.height), new GUIContent(""), masks.GetArrayElementAtIndex(i).boolValue);
                }
                if (EditorGUI.EndChangeCheck()) Mark();
                GUI.EndGroup();
            }
        }
        protected void ProjectionLimit()
        {
            if (type.enumValueIndex == 0)
            {
                Rect Rect = GUILayoutUtility.GetRect(0, 22 + LlockhamEditorUtility.Spacing);
                EditorGUI.DrawRect(new Rect(Rect.x, Rect.y + 2, Rect.width, Rect.height - 6), LlockhamEditorUtility.MidgroundColor);
                GUI.BeginGroup(Rect);

                //Header
                EditorGUI.LabelField(new Rect(0, 4, Rect.width * 0.4f, 16), new GUIContent("Angle Limit", "Determines angle (between the decals forward vector & the surface normal) at which we stop drawing the projection. Prevents projections from drawing in situations in which they would be stretched."), EditorStyles.boldLabel);

                //Properties
                EditorGUI.BeginChangeCheck();
                projectionLimit.floatValue = EditorGUI.Slider(new Rect((Rect.width * 0.4f) + 8, 4, (Rect.width * 0.6f) - 12, 16), new GUIContent(""), projectionLimit.floatValue, 0, 180);
                if (EditorGUI.EndChangeCheck())
                {
                    MarkAllPreviews();
                    Mark();
                }
                GUI.EndGroup();
            }
        }
        protected void ForceForward()
        {
            foreach (UnityEngine.Object obj in serializedObject.targetObjects)
            {
                Projection projection = obj as Projection;

                if (projection.SupportedRendering == RenderingPaths.Both)
                {
                    Rect Rect = GUILayoutUtility.GetRect(0, 22 + LlockhamEditorUtility.Spacing);
                    EditorGUI.DrawRect(new Rect(Rect.x, Rect.y + 2, Rect.width, Rect.height - 6), LlockhamEditorUtility.MidgroundColor);
                    GUI.BeginGroup(Rect);

                    //Header
                    EditorGUI.LabelField(new Rect(0, 4, Rect.width * 0.4f, 16), new GUIContent("Force Forward", "Should this projection be forced to render in a forward render loop?"), EditorStyles.boldLabel);

                    //Properties
                    EditorGUI.BeginChangeCheck();
                    forceForward.boolValue = EditorGUI.Toggle(new Rect((Rect.width * 0.4f) + 8, 4, (Rect.width * 0.6f) - 12, 16), new GUIContent(""), forceForward.boolValue);
                    if (EditorGUI.EndChangeCheck()) Mark();

                    GUI.EndGroup();
                }
                else
                {
                    //Cannot force forward on a forward only or deferred only projection
                    forceForward.boolValue = false;
                }
            }
            
        }
        protected void Instanced()
        {
            Rect Rect = GUILayoutUtility.GetRect(0, 22 + LlockhamEditorUtility.Spacing);
            EditorGUI.DrawRect(new Rect(Rect.x, Rect.y + 2, Rect.width, Rect.height - 6), LlockhamEditorUtility.MidgroundColor);
            GUI.BeginGroup(Rect);

            //Header
            EditorGUI.LabelField(new Rect(0, 4, Rect.width * 0.4f, 16), new GUIContent("Instanced", "Should instancing be enabled on this projection?"), EditorStyles.boldLabel);

            //Properties
            EditorGUI.BeginChangeCheck();
            instanced.boolValue = EditorGUI.Toggle(new Rect((Rect.width * 0.4f) + 8, 4, (Rect.width * 0.6f) - 12, 16), new GUIContent(""), instanced.boolValue);
            if (EditorGUI.EndChangeCheck()) Mark();

            GUI.EndGroup();
        }

        protected void ExecuteDelayedMark()
        {
            //Delayed Mark
            if (delayedMark)
            {
                //Update projection & properties
                for (int i = 0; i < serializedObject.targetObjects.Length; i++)
                {
                    ((Projection)serializedObject.targetObjects[i]).Mark(true);
                }

                //Repaint scene
                SceneView.RepaintAll();

                //No longer require undo
                delayedMark = false;
            }
        }
    }
}