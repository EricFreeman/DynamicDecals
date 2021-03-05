using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Decals
{
    /**
* The cursor positioner component. Positions a projection at the cursor position.
*/
    public class CursorPositioner : Positioner
    {
        /**
        * The Camera used to intepret the mouse position. If null will default to the main camera.
        */
        public Camera projectionCamera;

        protected override void Start()
        {
            //If no camera provided, use main
            if (projectionCamera == null) projectionCamera = Camera.main;

            base.Start();
        }
        void LateUpdate()
        {
            //Reproject every update
            Reproject(projectionCamera.ScreenPointToRay(Input.mousePosition), Mathf.Infinity, projectionCamera.transform.up);
        }
    }
}