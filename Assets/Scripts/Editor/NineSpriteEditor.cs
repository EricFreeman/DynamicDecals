using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LlockhamIndustries.Decals
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(NineSprite))]
    public class NineSpriteEditor : Editor
    {
        SerializedProperty sprite;
        SerializedProperty borderPixelSize;
        SerializedProperty borderWorldSize;

        private void OnEnable()
        {
            sprite = serializedObject.FindProperty("sprite");
            borderPixelSize = serializedObject.FindProperty("borderPixelSize");
            borderWorldSize = serializedObject.FindProperty("borderWorldSize");
        }

        public override void OnInspectorGUI()
        {
            //Update object
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(sprite);
            EditorGUILayout.Space();
            EditorGUILayout.Slider(borderPixelSize, 0, 0.5f);
            EditorGUILayout.Slider(borderWorldSize, 0.1f, 5);
            EditorGUILayout.Space();

            //Apply modified properties
            serializedObject.ApplyModifiedProperties();

            //Update
            for (int i = 0; i < serializedObject.targetObjects.Length; i++)
            {
                ((NineSprite)serializedObject.targetObjects[i]).UpdateNineSprite();
            }
        }
    }
}