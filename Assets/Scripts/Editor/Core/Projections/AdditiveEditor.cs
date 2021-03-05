using UnityEngine;
using UnityEditor;
using System.Collections;

namespace LlockhamIndustries.Decals
{
    [CustomEditor(typeof(Additive))]
    public class AdditiveEditor : UnlitEditor
    {
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