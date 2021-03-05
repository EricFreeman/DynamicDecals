using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Misc
{
    public class Press : Trap
    {
        [Header("Components")]
        public Rigidbody press;
        
        [Header("State - Triggered")]
        public Vector3 triggeredPosition = new Vector3(-1.25f, 0, 0);
        public float triggeredAcceleration = 10;
        public float triggeredDuration = 2;
        
        [Header("State - Rearmed")]
        public Vector3 rearmedPosition = new Vector3(0, 0, 0);
        public float rearmedAcceleration = 0.1f;

        //Calculation
        private Vector3 goalPosition;
        private float acceleration;

        //Generic methods
        private void Start()
        {
            goalPosition = rearmedPosition;
            acceleration = rearmedAcceleration;
        }
        private void FixedUpdate()
        {
            if (Vector3.Distance(press.transform.localPosition, goalPosition) > 0.01f || press.velocity.magnitude > 0.001f)
            {
                //Calculate velocity
                Vector3 velocity = (goalPosition - press.transform.localPosition) / Time.fixedDeltaTime;

                //Convert acceleration from m/s^2 to m/loop^2
                float loopAcceleration = acceleration / Time.fixedDeltaTime;

                //Convert to world space
                velocity = transform.TransformDirection(velocity);

                //Clamp velocity by acceleration
                velocity = Vector3.MoveTowards(press.velocity, velocity, loopAcceleration);

                //Set velocity
                press.velocity = velocity;
            }
        }

        //Trap methods
        protected override IEnumerator OnTrigger()
        {
            //Set goal position and velocity
            goalPosition = triggeredPosition;
            acceleration = triggeredAcceleration;

            //Time held
            float timeHeld = 0;
            while (timeHeld < triggeredDuration )
            {
                if (Vector3.Distance(press.transform.localPosition, goalPosition) < 0.01f && press.velocity.magnitude < 0.001f) timeHeld += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            //Trigger complete
            TriggerComplete();
        }
        protected override IEnumerator OnRearm()
        {
            //Set goal position and velocity
            goalPosition = rearmedPosition;
            acceleration = rearmedAcceleration;

            while (Vector3.Distance(press.transform.localPosition, goalPosition) > 0.01f || press.velocity.magnitude > 0.001f)
            {
                yield return new WaitForFixedUpdate();
            }

            //Rearm complete
            RearmComplete();
        }
    }
}