using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LlockhamIndustries.Decals
{
    [CustomEditor(typeof(RayPositioner))]
    public class RayPositionerEditor : PositionerEditor
    {
        SerializedProperty rayTransform;
        SerializedProperty positionOffset;
        SerializedProperty rotationOffset;
        SerializedProperty castLength;

        public override void OnEnable()
        {
            base.OnEnable();

            rayTransform = serializedObject.FindProperty("rayTransform");
            positionOffset = serializedObject.FindProperty("positionOffset");
            rotationOffset = serializedObject.FindProperty("rotationOffset");
            castLength = serializedObject.FindProperty("castLength");
        }

        public override void OnInspectorGUI()
        {
            //Update object
            serializedObject.Update();

            projectionGUI();
            CastGUI();
            RayGUI();

            //Apply modified properties
            serializedObject.ApplyModifiedProperties();
        }

        private void RayGUI()
        {
            EditorGUILayout.LabelField(new GUIContent("Ray", "The ray to project from"));
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(rayTransform, new GUIContent("Transform", "The transform to project the ray from. If null will use this"));
            EditorGUILayout.PropertyField(positionOffset, new GUIContent("Position Offset", "A position offset applied to the transform to determines the ray starting position"));
            EditorGUILayout.PropertyField(rotationOffset, new GUIContent("Rotation Offset", "A rotation offset applied to the transforms forward direction to determine the rays direction"));
            EditorGUILayout.PropertyField(castLength, new GUIContent("Cast Length", "How far to cast the ray"));
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
    }
}