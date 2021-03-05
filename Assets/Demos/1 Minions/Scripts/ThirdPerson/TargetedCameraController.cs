using UnityEngine;
using System.Collections;

namespace LlockhamIndustries.Misc
{
    //Camera Controller
    public class TargetedCameraController : GenericCameraController
    {
        [Header("Target")]
        public Transform target;
        public float trackingSpeed = 0.1f;

        [Header("Look")]
        public float lookSensitivity = 0.3f;
        public float lookSpeed = 0.2f;
        public AnimationCurve lookCurve;

        //Backing fields
        private Vector2 screenOffset;

        private Vector3 basePos;
        private Vector3 cameraVelocity;

        private Vector3 offset;
        private Vector3 offsetVelocity;

        //Generic methods
        private void Update()
        {
            OffsetInput();
            RotationZoomInput();
        }
        private void LateUpdate()
        {
            ApplyPosition();
            ApplyRotationZoom();
        }

        //Targeted offset
        private void OffsetInput()
        {
            Vector2 screenCentre = new Vector2(Screen.width / 2, Screen.height / 2);
            screenOffset.x = (Input.mousePosition.x - screenCentre.x) / screenCentre.x;
            screenOffset.y = (Input.mousePosition.y - screenCentre.y) / screenCentre.y;
        }
        private void ApplyPosition()
        {
            if (target != null)
            {
                //Base position
                basePos = Vector3.SmoothDamp(basePos, target.position, ref cameraVelocity, trackingSpeed);

                //Offset position
                screenOffset = screenOffset.normalized * lookCurve.Evaluate(screenOffset.magnitude) * lookSensitivity;

                //Convert screen offset into a position offset
                Vector3 positionOffset = (Forward * screenOffset.y) + (Right * screenOffset.x);

                //Smooth offset
                offset = Vector3.SmoothDamp(offset, positionOffset, ref offsetVelocity, lookSpeed);

                //Final Position
                transform.position = basePos + (offset * zoom);
            }
        }
    }
}