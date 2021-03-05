using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LlockhamIndustries.Misc
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(StarGenerator))]
    public class StarGeneratorEditor : Editor
    {
        SerializedProperty seed;
        SerializedProperty count;
        SerializedProperty radius;
        SerializedProperty octaves;
        SerializedProperty size;

        private void OnEnable()
        {
            seed = serializedObject.FindProperty("seed");
            count = serializedObject.FindProperty("count");
            radius = serializedObject.FindProperty("radius");
            octaves = serializedObject.FindProperty("octaves");
            size = serializedObject.FindProperty("size");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(seed);
            EditorGUILayout.PropertyField(count);
            EditorGUILayout.PropertyField(radius);
            EditorGUILayout.IntSlider(octaves, 1, 8);
            EditorGUILayout.PropertyField(size);
            EditorGUILayout.Space();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            if (GUILayout.Button("Generate"))
            {
                for (int i = 0; i < serializedObject.targetObjects.Length; i++)
                {
                    StarGenerator generator = serializedObject.targetObjects[i] as StarGenerator;
                    if (generator != null) generator.GenerateQuadStars();
                }
            }
        }
    }
}