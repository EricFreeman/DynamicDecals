using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LlockhamIndustries.Misc
{
    [CustomEditor(typeof(LaserSpawner))]
    public class DemoLSEditor : Editor
    {
        SerializedProperty laser;
        SerializedProperty laserCount;
        SerializedProperty spawnRate;

        void OnEnable()
        {
            laser = serializedObject.FindProperty("laser");
            laserCount = serializedObject.FindProperty("laserCount");
            spawnRate = serializedObject.FindProperty("spawnRate");
        }

        public override void OnInspectorGUI()
        {
            //Update object
            serializedObject.Update();

            EditorGUILayout.PropertyField(laser);
            laserCount.intValue = EditorGUILayout.IntSlider("Laser Count", laserCount.intValue, 0, 10000);
            EditorGUILayout.PropertyField(spawnRate);

            //Apply modified properties
            serializedObject.ApplyModifiedProperties();

            //Update laser count
            ((LaserSpawner)target).LaserCountChange();
        }
    }
}

