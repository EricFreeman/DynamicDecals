using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LlockhamIndustries.ExtensionMethods;

namespace LlockhamIndustries.VR
{
    public class MovementController : VRController
    {
        //Inspector variables
        public VRPlayspace playspace;
        public GameObject movementHandle;

        public float cooldown = 0.5f;
        public float speed = 0.08f;

        public MovementMode movementMode;
        public float combatLimit = 3;

        //Cooldown
        private float downtime = 0;

        //Smoothing
        private Vector3 targetVelocity;
        private Vector3 targetPoint;

        //Handle offset
        private Vector3 HandleOffset
        {
            get
            {
                Vector3 offset = transform.position - playspace.Position;
                offset.y = 0;
                return offset;
            }
        }

        //Generic methods
        private void Start()
        {
            //Instantiate our movement handle
            if (!movementHandle.activeInHierarchy)
            {
                movementHandle = Instantiate(movementHandle, transform.parent);
            }
        }
        private void LateUpdate()
        {
            //Validity check
            if (playspace == null || movementHandle == null)
            {
                Debug.LogWarning("Please assign a valid playspace and handle to your movement controller.");
                return;
            }

            //Cooldown
            if (downtime > 0)
            {
                downtime = Mathf.MoveTowards(downtime, 0, Time.deltaTime);
                return;
            }  

            //Grip enables movement
            if (Grip)
            {
                //Calculate teleport direction
                Vector3 dir = transform.forward;

                //Zero out y axis
                dir.y = 0;

                //Normalize
                dir = dir.normalized;

                //Calculate teleport distance
                float distance = Vector3.Dot(transform.forward, dir * combatLimit);

                //Calculate handle offset
                Vector3 offset = HandleOffset;

                //Calculate target point
                Vector3 point = playspace.Position + (dir * distance);

                //Smooth target point over time
                targetPoint = Vector3.SmoothDamp(targetPoint, point, ref targetVelocity, speed);

                //Trial moving to point to get position
                Vector3 position = playspace.TrialPosition(targetPoint);

                //Update handle
                UpdateHandle(position, offset);

                //Trigger moves to target position
                if (Trigger)
                {
                    //Set cooldown
                    downtime = cooldown;

                    //Move playspace
                    playspace.Position = position;

                    //Disable handle
                    DisableHandle();
                }
            }
            else DisableHandle();
        }
        
        //Movement handle
        private void UpdateHandle(Vector3 targetPosition, Vector3 handleOffset)
        {
            //Activate handle
            if (!movementHandle.activeInHierarchy) movementHandle.SetActive(true);

            //Position handle with offset
            movementHandle.transform.position = targetPosition + handleOffset;
        }
        private void DisableHandle()
        {
            //Deactivate handle
            if (movementHandle.activeInHierarchy) movementHandle.SetActive(false);

            //Reset target point & velocity
            targetPoint = playspace.Position;
            targetVelocity = Vector3.zero;
        }

        public enum MovementMode { Immediate, Combat };
    }
}