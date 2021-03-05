using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LlockhamIndustries.Decals
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RayCollisionPrinter))]
    public class RayCollisionPrinterEditor : PrinterEditor
    {
        SerializedProperty condition;
        SerializedProperty conditionTime;

        SerializedProperty layers;

        SerializedProperty method;
        SerializedProperty castCenter;
        SerializedProperty castDimensions;
        SerializedProperty castLength;
        SerializedProperty positionOffset;
        SerializedProperty rotationOffset;
        SerializedProperty hitTriggers;

        public override void OnEnable()
        {
            base.OnEnable();

            condition = serializedObject.FindProperty("condition");
            conditionTime = serializedObject.FindProperty("conditionTime");

            layers = serializedObject.FindProperty("layers");

            method = serializedObject.FindProperty("method");
            castCenter = serializedObject.FindProperty("castCenter");
            castDimensions = serializedObject.FindProperty("castDimensions");
            castLength = serializedObject.FindProperty("castLength");
            positionOffset = serializedObject.FindProperty("positionOffset");
            rotationOffset = serializedObject.FindProperty("rotationOffset");
            hitTriggers = serializedObject.FindProperty("hitTriggers");
        }

        public override void OnInspectorGUI()
        {
            //Update object
            serializedObject.Update();

            PrintGUI();
            BehaviourGUI();
            PoolGUI();
            ParentGUI();
            OverlapGUI();
            FrequencyGUI();

            ConditionGUI();
            LayersGUI();
            CastGUI();

            //Apply modified properties
            serializedObject.ApplyModifiedProperties();
        }

        private void ConditionGUI()
        {
            EditorGUILayout.LabelField(new GUIContent("Condition", "When and how the decal is printed."));
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(condition, new GUIContent("", "When and how the decal is printed."));
            if (condition.enumValueIndex == 1)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(conditionTime, new GUIContent("Delay", "How long after entry to delay printing the decal."));
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
        private void LayersGUI()
        {
            if (prints.arraySize > 1 && printMethod.enumValueIndex == 2)
            {
                int finalLayers = 0;
                foreach (SerializedProperty layermask in printLayers)
                {
                    finalLayers = (finalLayers | layermask.intValue);
                }
                layers.intValue = finalLayers;
            }
            else
            {
                EditorGUILayout.LabelField(new GUIContent("Layers", "Which layers to cast against"));
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(layers, new GUIContent("", "Which layers to cast against"));
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
        }
        private void CastGUI()
        {
            EditorGUILayout.LabelField("Cast Information");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(method);
            EditorGUILayout.PropertyField(castCenter);
            if (method.enumValueIndex != 0)
            {
                EditorGUILayout.PropertyField(castDimensions);
            }
            EditorGUILayout.PropertyField(castLength);
            EditorGUILayout.PropertyField(positionOffset);
            EditorGUILayout.PropertyField(rotationOffset);
            EditorGUILayout.PropertyField(hitTriggers);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
    }
}