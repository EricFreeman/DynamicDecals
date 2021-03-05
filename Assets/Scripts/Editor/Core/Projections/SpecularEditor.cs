using UnityEngine;
using UnityEditor;
using System.Collections;

namespace LlockhamIndustries.Decals
{
    [CustomEditor(typeof(Specular))]
    public class SpecularEditor : ProjectionEditor
    {
        //Property groups
        protected SerializedProperty albedo;
        protected SerializedProperty specular;
        protected SerializedProperty normal;
        protected SerializedProperty emissive;

        //Property group drawers
        protected AlbedoDrawer albedoDrawer;
        protected SpecularDrawer specularDrawer;
        protected NormalDrawer normalDrawer;
        protected EmissiveDrawer emissiveDrawer;

        public override void OnEnable()
        {
            base.OnEnable();

            if (target != null)
            {
                //Grab our properties
                albedo = serializedObject.FindProperty("albedo");
                specular = serializedObject.FindProperty("specular");
                normal = serializedObject.FindProperty("normal");
                emissive = serializedObject.FindProperty("emissive");

                //Initialize our property drawers
                if (propertyGroups == null || propertyGroups.Length != 4) propertyGroups = new PropertyGroupDrawer[4];
                if (propertyGroups[0] == null) propertyGroups[0] = new AlbedoTextureDrawer(new GUIContent("Albedo"), albedo, transparencyType, cutoff, this);
                if (propertyGroups[1] == null) propertyGroups[1] = new SpecularTextureDrawer(new GUIContent("Gloss"), specular, this);
                if (propertyGroups[2] == null) propertyGroups[2] = new NormalTextureDrawer(new GUIContent("Normal"), normal, this);
                if (propertyGroups[3] == null) propertyGroups[3] = new EmissiveTextureDrawer(new GUIContent("Emissive"), emissive, this);

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