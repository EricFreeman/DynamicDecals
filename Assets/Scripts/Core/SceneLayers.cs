using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Decals
{
    /**
    * Allows a single scene to have unique masking layers seperate to the rest of the project. These masking layers will take effect when the component is enabled (on scene load) and the original layers returned when the component is disabled (on scene end).
    */
    public class SceneLayers : MonoBehaviour
    {
        public ProjectionLayer[] layers;
        private ProjectionLayer[] original;

        private void OnEnable()
        {
            //Store original settings
            original = DynamicDecals.System.Settings.Layers;

            //Load custom settings
            DynamicDecals.System.Settings.Layers = layers;
        }
        private void OnDisable()
        {
            //Revert to original settings
            DynamicDecals.System.Settings.Layers = original;
        }
    }
}