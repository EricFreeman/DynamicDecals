using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace LlockhamIndustries.Decals
{
    [CustomEditor(typeof(SceneLayers))]
    public class SceneLayersEditor : Editor
    {
        private SerializedProperty layers;

        private void OnEnable()
        {
            layers = serializedObject.FindProperty("layers");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Space();

            Rect rect = GUILayoutUtility.GetRect(0, layers.arraySize * 20 + 40);
            MaskSettings(rect, layers);

            EditorGUILayout.Space();
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void MaskSettings(Rect Area, SerializedProperty Layers)
        {
            GUI.BeginGroup(Area);

            //Header
            EditorGUI.DrawRect(new Rect(0, 0, Area.width, 24), LlockhamEditorUtility.HeaderColor);
            EditorGUI.LabelField(new Rect(8, 4, Area.width - 32, 16), "Masking", EditorStyles.boldLabel);

            //Reset
            Rect Reset = new Rect(Area.width - 20, 6, 12, 12);
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Reset.Contains(Event.current.mousePosition))
            {
                ResetLayers(Layers);
                Event.current.Use();
            }
            GUI.DrawTexture(Reset, LlockhamEditorUtility.Reset);

            //Draw Background
            EditorGUI.DrawRect(new Rect(0, 24, Area.width, Area.height - 24), LlockhamEditorUtility.MidgroundColor);

            //Generate layer options
            for (int i = 0; i < Layers.arraySize; i++)
            {
                SerializedProperty layer = Layers.GetArrayElementAtIndex(i);
                SerializedProperty name = layer.FindPropertyRelative("name");
                SerializedProperty layers = layer.FindPropertyRelative("layers");

                Rect nameRect = new Rect(4, 32 + (i * 20), Area.width - 160, 16);
                Rect layerRect = new Rect(Area.width - 140, 32 + (i * 20), 120, 16);

                EditorGUI.PropertyField(nameRect, name, GUIContent.none);
                EditorGUI.PropertyField(layerRect, layers, GUIContent.none);
            }

            GUI.EndGroup();
        }
        private void ResetLayers(SerializedProperty Layers)
        {
            Layers.arraySize = 4;
            for (int i = 0; i < Layers.arraySize; i++)
            {
                SerializedProperty layer = Layers.GetArrayElementAtIndex(i);
                SerializedProperty name = layer.FindPropertyRelative("name");
                SerializedProperty layers = layer.FindPropertyRelative("layers");

                name.stringValue = "Layer " + (i + 1);
                layers.intValue = 0;
            }

            Layers.serializedObject.ApplyModifiedProperties();
        }
    }
}