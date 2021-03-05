using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LlockhamIndustries.Decals;

namespace LlockhamIndustries.Misc
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(FirstPersonCharacterController))]
    public class GooController : MonoBehaviour
    {
        [Header("Slide")]
        public float slideSpeed = 25;

        [Header("Bounce")]
        public float bouncyness = 1;

        //Backing fields
        private PhysicMaterial material;
        private Rigidbody attachedRigidbody;
        private FirstPersonCharacterController controller;

        private float originalSpeed;
        private float originalSFriction;
        private float originalDFriction;
        private PhysicMaterialCombine originalFrictionCombine;

        //Generic methods
        private void Awake()
        {
            //Grab components
            material = GetComponent<Collider>().material;
            attachedRigidbody = GetComponent<Rigidbody>();
            controller = GetComponent<FirstPersonCharacterController>();
            
            //Grab original values
            originalSpeed = controller.moveSpeed;
            originalSFriction = material.staticFriction;
            originalDFriction = material.dynamicFriction;
            originalFrictionCombine = material.frictionCombine;
        }

        //Physics methods
        private void FixedUpdate()
        {
            if (controller.Grounded)
            {
                if (GooManager.WithinGoo(GooType.Slide, transform.position + new Vector3(0, -1, 0), 0.2f))
                {
                    //Increase movement speed
                    controller.moveSpeed = slideSpeed;

                    //Remove all friction
                    material.staticFriction = 0;
                    material.dynamicFriction = 0;
                    material.frictionCombine = PhysicMaterialCombine.Minimum;
                }
                else
                {
                    //Back to original speed
                    controller.moveSpeed = originalSpeed;

                    //Return friction
                    material.staticFriction = originalSFriction;
                    material.dynamicFriction = originalDFriction;
                    material.frictionCombine = originalFrictionCombine;
                }
            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (GooManager.WithinGoo(GooType.Bounce, transform.position + new Vector3(0, -1, 0), 0.2f))
            {
                attachedRigidbody.AddForce(collision.impulse * bouncyness, ForceMode.Impulse);
            }
        }
    }
}