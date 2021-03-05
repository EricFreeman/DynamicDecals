using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Misc
{
    public class FreeCameraController : GenericCameraController
    {
        [Header("Movement")]
        public float movementSpeed = 0.1f;
        public float movementThreshold = 0.1f;

        [Header("Limits")]
        public float minX = -10;
        public float maxX = 10;
        public float minZ = -10;
        public float maxZ = 10;

        //Backing fields
        private Vector2 mousePosition;
        private Vector3 cameraVelocity;

        //Generic methods
        private void Update()
        {
            EdgeScrollInput();
            RotationZoomInput();
        }
        private void LateUpdate()
        {
            ApplyEdgeScroll();
            ApplyRotationZoom();
        }

        //Edge scroll
        private void EdgeScrollInput()
        {
            //Mouse position
            mousePosition = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
        }
        private void ApplyEdgeScroll()
        {
            //Calculate camera movement
            Vector3 movement = Vector3.zero;

            if (mousePosition.x < movementThreshold) movement -= Right * (movementThreshold - mousePosition.x) / movementThreshold * movementSpeed;
            if (1 - mousePosition.x < movementThreshold) movement += Right * (movementThreshold - (1 - mousePosition.x)) / movementThreshold * movementSpeed;
            if (mousePosition.y < movementThreshold) movement -= Forward * (movementThreshold - mousePosition.y) / movementThreshold * movementSpeed;
            if (1 - mousePosition.y < movementThreshold) movement += Forward * (movementThreshold - (1 - mousePosition.y)) / movementThreshold * movementSpeed;

            //Scale movement by zoom
            movement *= zoom / maxZoom;

            //Calculate goal position
            Vector3 goalPosition = transform.position + movement;

            //Clamp goal position
            goalPosition.x = Mathf.Clamp(goalPosition.x, minX, maxX);
            goalPosition.z = Mathf.Clamp(goalPosition.z, minZ, maxZ);

            //Position
            transform.position = Vector3.SmoothDamp(transform.position, goalPosition, ref cameraVelocity, 0.1f);
        }
    }
}
