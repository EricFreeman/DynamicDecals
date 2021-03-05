using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Decals
{
    /**
    * The CollisionPrinter Component. Prints a projection under set conditions related to the collision of the object attached to this printer.
    */
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class CollisionPrinter : Printer
    {
        /**
        * Defines the orientation of the projection relative to the surface of the collision. Velocity will orient the projection as if its up is the direction the collision object is moving in. Random will orient the projection as if its up is random.
        */
        public RotationSource rotationSource;

        /**
        * Defines the condition on which a projection is printed. Enter will print whenever a collision occurs. Delay will print the conditionTime seconds after a collision occurs. Constant will print every fixed update during a collision. Exit will print upon exiting a collision.
        */
        public CollisionCondition condition;
        /**
        * If the collision condition is set to delay, the conditionTime determines the length of that delay.
        */
        public float conditionTime;

        /**
        * The layers that, when collided with, cause a print.
        */
        public LayerMask layers;

        private float timeElapsed;
        private bool delayPrinted;

        void OnCollisionEnter(Collision collision)
        {
            //Print on Enter or Constant
            if (condition == CollisionCondition.Enter || condition == CollisionCondition.Constant)
            {
                PrintCollision(collision);
            }

            //Set up our collision
            timeElapsed = 0;
            delayPrinted = false;
        }
        void OnCollisionStay(Collision collision)
        {
            timeElapsed += Time.deltaTime;

            //Print on constant
            if (condition == CollisionCondition.Constant)
            {
                PrintCollision(collision);
            }

            //Print on delay if time elapsed is greater than delay and we haven't during this collision
            if (condition == CollisionCondition.Delay && timeElapsed > conditionTime && !delayPrinted)
            {
                PrintCollision(collision);
                delayPrinted = true;
            }
        }
        void OnCollisionExit(Collision collision)
        {
            //Print on exit
            if (condition == CollisionCondition.Exit)
            {
                PrintCollision(collision);
            }

            //Print on delay if we haven't during this collision
            if (condition == CollisionCondition.Delay && !delayPrinted)
            {
                PrintCollision(collision);
            }
        }

        public void PrintCollision(Collision collision)
        {
            Transform surface = null;
            Vector3 position = Vector3.zero;
            Vector3 normal = Vector3.zero;

            //Iterate over all contact points
            int validContacts = 0;
            foreach (ContactPoint contact in collision.contacts)
            {
                //Make sure our colliding object is within layermask
                if (layers.value == (layers.value | 1 << contact.otherCollider.gameObject.layer))
                {
                    //We have a valid contact
                    validContacts++;

                    if (validContacts == 1) surface = contact.otherCollider.transform;
                    if (validContacts == 1) position = contact.point;
                    if (validContacts == 1) normal = contact.normal;
                }
            }

            if (validContacts > 0)
            {
                //Calculate final position and surface normal (Not collision normal)
                RaycastHit hit;
                if (Physics.Raycast(position + (normal * 0.3f), -normal, out hit, 0.4f, layers.value))
                {
                    position = hit.point;
                    normal = hit.normal;
                    surface = hit.collider.transform;

                    //Calculate our rotation
                    Vector3 rot;
                    if (rotationSource == RotationSource.Velocity && GetComponent<Rigidbody>().velocity != Vector3.zero) rot = GetComponent<Rigidbody>().velocity.normalized;
                    else if (rotationSource == RotationSource.Random) rot = Random.insideUnitSphere.normalized;
                    else rot = Vector3.up;

                    //Print
                    Print(position, Quaternion.LookRotation(-normal, rot), surface, hit.collider.gameObject.layer);
                }
            }
        }
    }

    public enum CollisionCondition { Enter, Delay, Constant, Exit }
    public enum RotationSource { Velocity, Random }
}