using UnityEngine;
using UnityEditor;
using System.Collections;

namespace LlockhamIndustries.Decals
{
    [CustomEditor(typeof(Forward), true)]
    public class UnlitEditor : ProjectionEditor
    {
        //Property groups
        protected SerializedProperty albedo;

        //Property group drawers
        protected AlbedoDrawer albedoDrawer;

        public override void OnEnable()
        {
            base.OnEnable();

            if (target != null)
            {
                //Grab our properties
                albedo = serializedObject.FindProperty("albedo");

                //Initialize our property drawers
                if (propertyGroups == null || propertyGroups.Length != 1) propertyGroups = new PropertyGroupDrawer[1];
                if (propertyGroups[0] == null) propertyGroups[0] = new AlbedoTextureDrawer(new GUIContent("Albedo"), albedo, transparencyType, cutoff, this);

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
            Transparency();

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
    }
}