using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LlockhamIndustries.Decals
{
    [CustomEditor(typeof(CursorPositioner))]
    public class CursorPositionerEditor : PositionerEditor
    {
        SerializedProperty projectionCamera;

        public override void OnEnable()
        {
            base.OnEnable();

            projectionCamera = serializedObject.FindProperty("projectionCamera");
        }

        public override void OnInspectorGUI()
        {
            //Update object
            serializedObject.Update();

            projectionGUI();
            CastGUI();
            CameraGUI();


            //Apply modified properties
            serializedObject.ApplyModifiedProperties();
        }

        private void CameraGUI()
        {
            EditorGUILayout.LabelField(new GUIContent("Camera", "The Camera to project from, if null will use main"));
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(projectionCamera, new GUIContent("", "The Camera to project from, if null will use main"));
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
    }
}