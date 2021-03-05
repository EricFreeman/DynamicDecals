using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LlockhamIndustries.Misc
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody))]
    public class Locomotion : MonoBehaviour
    {
        //Inspector
        public float acceleration = 0.2f;
        public float rotationSpeed = 0.2f;

        //Properties
        public Vector3 Movement
        {
            set { movement = value; }
        }
        public Vector3 Direction
        {
            set { rotationDirection = value; }
        }

        //Backing fields
        Animator anim;

        private float rotation;
        private float rotationVelocity;
        private Vector3 rotationDirection;

        private Vector2 locomotion;
        private Vector3 movement;
        private float previousMoveSpeed;
        private float moveSpeedVelocity;

        private Vector3 previousMovement;
        private Vector3 moveDirectionVelocity;

        private float locomotionStrength = 0;
        private float previousLocomotionStrength = 0;
        private float locomotionStrengthVelocity = 0;

        //Generic Methods
        protected void Awake()
        {
            //Grab components
            anim = GetComponent<Animator>();

            //set rotation forward
            rotation = Quaternion.LookRotation(transform.forward).eulerAngles.y;
        }
        protected void FixedUpdate()
        {
            UpdateRotation();
            UpdateLocomotionDirection();
            UpdateLocomotionStrength();
        }

        //Rotation
        private void UpdateRotation()
        {
            float goalRotation = rotation;

            if (rotationDirection != Vector3.zero)
            {
                goalRotation = Quaternion.LookRotation(rotationDirection).eulerAngles.y;

                //Reduce our rotationSpeed as the unit moves faster
                float MoveSpeedModifier = 1 + Mathf.Clamp01(locomotion.magnitude - 1);
                float RotationSpeed = (1 / rotationSpeed * MoveSpeedModifier);

                rotation = Mathf.SmoothDampAngle(rotation, goalRotation, ref rotationVelocity, RotationSpeed * 0.02f);
            }
            else rotationVelocity = 0;

            //Set our rotation
            transform.rotation = Quaternion.Euler(new Vector3(0, rotation, 0));
        }

        //Movement
        private void UpdateLocomotionDirection()
        {
            //Smooth our Movement Direction
            Vector3 Movement = Vector3.SmoothDamp(previousMovement, movement, ref moveDirectionVelocity, (1 / acceleration * 0.02f));
            previousMovement = Movement;

            //Get the direction as an angle from forward, in Degrees, from -180 to 180..
            float directionAngle = Vector3.Angle(transform.forward, Movement.normalized);
            if (Mathf.Sign(Vector3.Dot(transform.right, Movement.normalized)) < 0)
            {
                directionAngle = 360 - directionAngle;
            }
            directionAngle = directionAngle * (Mathf.PI / 180);

            //Convert This Angle into seperate X and Y vectors..
            locomotion.x = Mathf.Sin(directionAngle);
            locomotion.y = Mathf.Cos(directionAngle);

            //Multiply Locomotion by the MovementSpeed of the character..
            locomotion *= Movement.magnitude;

            //The Sideward Movement of the character can only reach 0.5f, so if it's over, normalise our locomotion until it's at 0.5f
            if (locomotion.x > 0.5f)
            {
                float locNormalise = 0.5f / locomotion.x;
                locomotion *= locNormalise;
            }

            anim.SetFloat("X", locomotion.x);
            anim.SetFloat("Y", locomotion.y);
        }
        private void UpdateLocomotionStrength()
        {
            //General Movement
            float LocomotionStrengthMovement = Mathf.Clamp01(movement.magnitude * 2);

            //Rotating on the Spot
            float LocomotionStrengthRotation = Mathf.Clamp(Mathf.Abs(rotationVelocity / 400), 0, 1);

            //Determine Locomotion Strength and Smooth
            locomotionStrength = Mathf.Max(LocomotionStrengthMovement, LocomotionStrengthRotation);
            locomotionStrength = Mathf.SmoothDamp(previousLocomotionStrength, locomotionStrength, ref locomotionStrengthVelocity, 1 / acceleration * 0.02f);
            previousLocomotionStrength = locomotionStrength;

            anim.SetFloat("Locomote", locomotionStrength);
        }
    }
}