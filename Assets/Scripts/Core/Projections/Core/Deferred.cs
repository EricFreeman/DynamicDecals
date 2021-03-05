using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Decals
{
    /**
    * The base of all deferred only projections (Gloss, Normal)
    */
    [System.Serializable]
    public abstract class Deferred : Projection
    {
        //Deferred only
        public override RenderingPaths SupportedRendering
        {
            get { return RenderingPaths.Deferred; }
        }
    }
}