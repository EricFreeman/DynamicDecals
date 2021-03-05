using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Decals
{
    /**
    * Culls attached projection (destroy or return to pool) once it's no longer visible by any cameras. Useful for cleaning up your scene without the player noticing.
    * Designed to be printed with your projections. Attach to your prefab and enable print behaviours on your printer. You can turn this on or off by enabling and disbaling this component respectively.
    */
    [RequireComponent(typeof(ProjectionRenderer))]
    public class Cull : Modifier
    {
        //Inspector variables
        /**
        * How long the projection has to be off screen before it's culled. 0 will cull the projection the second it's no longer visible.
        */
        public float cullTime = 4;

        //Backing fields
        private ProjectionRenderer projection;
        private float timeElapsed = 0;

        private void Awake()
        {
            //Grab our projection
            projection = GetComponent<ProjectionRenderer>();
        }

        protected override void Begin()
        {
            timeElapsed = 0;
        }
        public override void Perform()
        {
            //Perform cull check
            if (timeElapsed < cullTime)
            {
                //Update time elapsed
                timeElapsed += UpdateRate;

                //Reset time elapsed if visible
                if (projection.Renderer.isVisible) timeElapsed = 0;
                return;
            }

            //Destroy projection
            projection.Destroy();
        }
    }
}