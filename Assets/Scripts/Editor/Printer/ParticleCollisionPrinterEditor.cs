using UnityEngine;
using UnityEditor;
using System.Collections;

namespace LlockhamIndustries.Decals
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ParticleCollisionPrinter))]
    public class ParticleCollisionPrinterEditor : PrinterEditor
    {
        SerializedProperty rotationSource;
        SerializedProperty ratio;

        public override void OnEnable()
        {
            base.OnEnable();

            rotationSource = serializedObject.FindProperty("rotationSource");
            ratio = serializedObject.FindProperty("ratio");
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
            RatioGUI();
            RotationSourceGUI();

            //Apply modified properties
            serializedObject.ApplyModifiedProperties();
        }

        private void RatioGUI()
        {
            EditorGUILayout.LabelField(new GUIContent("Ratio", "The percentage of particles that print projections. At 0, no particles will print, at 1, all will."));
            EditorGUI.indentLevel++;
            EditorGUILayout.Slider(ratio, 0, 1, new GUIContent("", "The percentage of particles that print projections. At 0, no particles will print, at 1, all will."));
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
        private void RotationSourceGUI()
        {
            EditorGUILayout.LabelField(new GUIContent("Rotation Source", "What should determine how the printed decal is orientated"));
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(rotationSource, new GUIContent("", "What should determine how the printed decal is orientated"));
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
    }
}
