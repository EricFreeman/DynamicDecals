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
    * Additive projection. Draws to the screen additively. If rendering in deferred, will be drawn in forward, after all other projections.
    */
    [System.Serializable]
    public class Additive : Forward
    {
        public override Material MobileForward
        {
            get { return MaterialFromShader(Shader.Find("Projection/Decal/Mobile/Additive")); }
        }
        public override Material StandardForward
        {
            get { return MaterialFromShader(Shader.Find("Projection/Decal/Standard/Additive")); }
        }
        public override Material PackedForward
        {
            get { return MaterialFromShader(Shader.Find("Projection/Decal/Packed/Additive")); }
        }
    }
}
