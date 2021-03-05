using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LlockhamIndustries.Decals
{
    /**
    * Unlit projection. Draws a flat color to the screen. Useful for projected UI elements. If rendering in deferred, will be drawn in forward, after all other projections.
    */
    [System.Serializable]
    public class Unlit : Forward
    {
        public override Material MobileForward
        {
            get { return MaterialFromShader(Shader.Find("Projection/Decal/Mobile/Unlit")); }
        }
        public override Material StandardForward
        {
            get { return MaterialFromShader(Shader.Find("Projection/Decal/Standard/Unlit")); }
        }
        public override Material PackedForward
        {
            get { return MaterialFromShader(Shader.Find("Projection/Decal/Packed/Unlit")); }
        }
    }
}
