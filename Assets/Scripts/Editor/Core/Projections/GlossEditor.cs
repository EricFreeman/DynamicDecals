using UnityEngine;
using UnityEditor;
using System.Collections;

namespace LlockhamIndustries.Decals
{
    [CustomEditor(typeof(Gloss))]
    public class GlossEditor : ProjectionEditor
    {
        //Property groups
        protected SerializedProperty gloss;
        protected SerializedProperty glossType;

        public override void OnEnable()
        {
            base.OnEnable();

            if (target != null)
            {
                //Grab our properties
                gloss = serializedObject.FindProperty("gloss");
                glossType = serializedObject.FindProperty("glossType");

                //Initialize our property drawers
                if (propertyGroups == null || propertyGroups.Length != 1) propertyGroups = new PropertyGroupDrawer[1];
                if (propertyGroups[0] == null) propertyGroups[0] = new GlossTextureDrawer(new GUIContent("Gloss"), gloss, this);

                //Initialize property groups
                if (propertyGroups != null) for (int i = 0; i < propertyGroups.Length; i++) propertyGroups[i].Initialize();
            }
        }
        public override void OnDisable()
        {
            base.OnDisable();
        }

        public override void OnInspectorGUI()
        {
            //Update
            serializedObject.Update();

            Type();
            Priority(40);
            Transparency(false);
            GlossType();

            //Draw property groups
            if (propertyGroups != null)
            {
                for (int i = 0; i < propertyGroups.Length; i++) propertyGroups[i].OnGUILayout();
            }

            //Masking();
            ProjectionLimit();
            ForceForward();
            Instanced();

            //Delayed Mark
            ExecuteDelayedMark();
        }

        private void GlossType()
        {
            Rect Rect = GUILayoutUtility.GetRect(0, 22 + LlockhamEditorUtility.Spacing);
            EditorGUI.DrawRect(new Rect(Rect.x, Rect.y + 2, Rect.width, Rect.height - 6), LlockhamEditorUtility.MidgroundColor);
            GUI.BeginGroup(Rect);

            //Header
            EditorGUI.LabelField(new Rect(0, 4, Rect.width * 0.4f, 16), new GUIContent("Gloss Type", "Defines how the gloss modifcation affects the surface. Shine will have the decal shine the surface it's applied too. Great for making surfaces appear wet. Dull will have the decal dull the surface its applied too. Great for making surfaces appear worn or weathered."), EditorStyles.boldLabel);

            //Properties
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(new Rect((Rect.width * 0.4f) + 8, 4, (Rect.width * 0.6f) - 12, 16), glossType, new GUIContent(""));
            if (EditorGUI.EndChangeCheck())
            {
                //Mark changes to be applied
                Mark();
            }
                

            GUI.EndGroup();
        }
    }
}