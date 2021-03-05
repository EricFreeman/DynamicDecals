using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LlockhamIndustries.Decals
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Cull))]
    public class CullEditor : ModifierEditor
    {
        SerializedProperty cullTime;
        SerializedProperty wrapMode;

        SerializedProperty fade;
        SerializedProperty fadeLength;

        protected override void OnEnable()
        {
            base.OnEnable();

            cullTime = serializedObject.FindProperty("cullTime");
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(cullTime);

            EditorGUILayout.Space();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}