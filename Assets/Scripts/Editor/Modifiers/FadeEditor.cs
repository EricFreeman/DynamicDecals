using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LlockhamIndustries.Decals
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Fade))]
    public class FadeEditor : ModifierEditor
    {
        SerializedProperty type;
        SerializedProperty wrapMode;

        SerializedProperty fade;
        SerializedProperty fadeLength;

        protected override void OnEnable()
        {
            base.OnEnable();

            type = serializedObject.FindProperty("type");
            wrapMode = serializedObject.FindProperty("wrapMode");

            fade = serializedObject.FindProperty("fade");
            fadeLength = serializedObject.FindProperty("fadeLength");
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(type);
            EditorGUILayout.PropertyField(wrapMode);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(fade);
            EditorGUILayout.PropertyField(fadeLength);

            EditorGUILayout.Space();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}