using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LlockhamIndustries.Decals
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Modifier))]
    public abstract class ModifierEditor : Editor
    {
        SerializedProperty frequency;

        protected virtual void OnEnable()
        {
            frequency = serializedObject.FindProperty("frequency");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.Space();
            if (!frequency.hasMultipleDifferentValues)
            {
                //Grab possible enum values
                Array values = Enum.GetValues(typeof(Frequency));

                int index = (int)(Frequency)EditorGUILayout.EnumPopup(new GUIContent("Update Rate", "How often the modifier updates."), (Frequency)values.GetValue(frequency.enumValueIndex));
                if (index != frequency.enumValueIndex)
                {
                    for (int i = 0; i < serializedObject.targetObjects.Length; i++)
                    {
                        ((Modifier)serializedObject.targetObjects[i]).Frequency = (Frequency)values.GetValue(index);
                    }
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Update Rate");
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField(" - ");
                EditorGUILayout.EndHorizontal();
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}