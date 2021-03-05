using UnityEngine;
using System.Collections;

namespace LlockhamIndustries.Misc
{
    //Weapon Controller
    public class WeaponController : MonoBehaviour
    {
        [Header("Controllers")]
        public Camera cameraController;
        public FirstPersonCharacterController controller;

        [Header("Aiming")]
        public LayerMask layers;
        public float aimSmooth = 60;
        public float aimDistance = 40;
        public Vector3 rotationOffset = new Vector3(0, 0, 0);

        //Aiming
        private Vector3 targetPosition;

        //Input
        protected bool primary;
        protected bool secondary;
        protected bool alternate;

        //Projectile fire
        protected float timeToFire;

        void OnEnable()
        {
            if (cameraController == null) cameraController = Camera.main;
        }

        void Update()
        {
            //Check for Input
            primary = (Input.GetMouseButton(0)) ? true : false;
            secondary = (!Input.GetMouseButton(0) && Input.GetMouseButton(1)) ? true : false;
            alternate = (!Input.GetMouseButton(0) && !Input.GetMouseButton(1) && Input.GetMouseButton(2)) ? true : false;
        }

        public virtual void UpdateWeapon()
        {
            Aim();

            //Increment time since fired
            timeToFire = Mathf.Clamp(timeToFire - Time.fixedDeltaTime, 0, Mathf.Infinity);
        }
        private void Aim()
        {
            //Calculate target position
            if (Application.isPlaying)
            {
                RaycastHit hit;
                if (Physics.Raycast(cameraController.transform.position, cameraController.transform.forward, out hit, Mathf.Infinity, layers.value))
                {
                    targetPosition = hit.point;
                }
                else
                {
                    targetPosition = cameraController.transform.position + cameraController.transform.forward * aimDistance;
                }

                //Aim
                Quaternion GoalRotation = Quaternion.LookRotation((targetPosition - transform.position).normalized, Vector3.up) * Quaternion.Euler(rotationOffset);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, GoalRotation, aimSmooth * Time.deltaTime);
            }
            else
            {
                //Aim
                targetPosition = cameraController.transform.position + cameraController.transform.forward * aimDistance;
                transform.rotation = Quaternion.LookRotation((targetPosition - transform.position).normalized, Vector3.up) * Quaternion.Euler(rotationOffset);
            }
        }
    }
}