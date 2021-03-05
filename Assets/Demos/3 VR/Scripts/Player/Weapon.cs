using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Misc
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SpringJoint))]
    public class Weapon : MonoBehaviour
    {
        //Weapon State
        public WeaponState WeaponState
        {
            get { return weaponState; }
            set
            {
                if (!weaponState.Equals(value))
                {
                    weaponState = value;
                    UpdateWeaponState();
                }

            }
        }
        private WeaponState weaponState;

        //Base States
        public WeaponState Standard;
        public WeaponState Extended;

        //Throw States
        public WeaponState PreReleased;
        public WeaponState Released;

        //Properties
        public float Velocity
        {
            get { return rb.velocity.magnitude; }
        }

        //Trail
        public TrailRenderer trail;
        public float trailTime = 0.2f;
        public float trailStartVelocity = 8;
        public float trailEndVelocity = 6;

        private float time;
        private float goalTime;
        private float timeDampVelocity;

        //Cached Components
        private Rigidbody rb;
        private SpringJoint joint;

        private void OnEnable()
        {
            //Grab rigidbody
            rb = GetComponent<Rigidbody>();
            joint = GetComponent<SpringJoint>();

            //Set weaponstate
            WeaponState = Standard;
        }
        private void FixedUpdate()
        {
            if (trail != null && rb != null)
            {
                //Calculate velocity
                float velocity = rb.velocity.magnitude;

                //Calculate goal time
                if (goalTime == 0 && velocity > trailStartVelocity) goalTime = trailTime;
                if (goalTime != 0 && velocity < trailEndVelocity) goalTime = 0;

                //Blend towards goal time
                time = Mathf.SmoothDamp(time, goalTime, ref timeDampVelocity, 0.1f);

                //Set trail time
                trail.time = time;
            }
        }

        private void UpdateWeaponState()
        {
            rb.mass = weaponState.mass;
            rb.drag = weaponState.drag;

            joint.maxDistance = weaponState.reach;
            joint.spring = weaponState.spring;
            joint.damper = weaponState.damper;
        }
    }

    [System.Serializable]
    public struct WeaponState
    {
        public float mass;
        public float drag;

        public float reach;

        public float spring;
        public float damper;

        public WeaponState(float Mass, float Drag, float Reach, float Spring, float Damper)
        {
            mass = Mass;
            drag = Drag;

            reach = Reach;

            spring = Spring;
            damper = Damper;
        }
    }
}