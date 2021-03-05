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
    * Multiplicative projection. Multiplies result with whats already on the screen. If rendering in deferred, will be drawn in forward, after all other projections.
    */
    [System.Serializable]
    public class Multiplicative : Forward
    {
        public override Material MobileForward
        {
            get { return MaterialFromShader(Shader.Find("Projection/Decal/Mobile/Multiplicative")); }
        }
        public override Material StandardForward
        {
            get { return MaterialFromShader(Shader.Find("Projection/Decal/Standard/Multiplicative")); }
        }
        public override Material PackedForward
        {
            get { return MaterialFromShader(Shader.Find("Projection/Decal/Packed/Multiplicative")); }
        }
    }
}
