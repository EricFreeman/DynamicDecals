using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LlockhamIndustries.ExtensionMethods;

namespace LlockhamIndustries.Misc
{
    public class Bleeder : MonoBehaviour
    {
        public GameObject prefab;
        public int bleedRate = 2;
        public int bleedLimit = 12;

        public LayerMask triggerLayers;
        public float triggerVelocity = 10;

        private void OnCollisionEnter(Collision collision)
        {
            if (Valid)
            {
                for (int i = 0; i < Mathf.Min(bleedLimit, collision.contacts.Length); i++)
                {
                    //Rigidbody check
                    Rigidbody r = collision.contacts[i].otherCollider.GetComponent<Rigidbody>();
                    if (r == null || r.velocity.magnitude <= triggerVelocity) continue;

                    //Blood check
                    Blood b = collision.contacts[i].otherCollider.GetComponent<Blood>();
                    if (b != null && b.source == this) continue;

                    //Layer check
                    if (!triggerLayers.Contains(collision.contacts[i].otherCollider.gameObject.layer)) continue;

                    //Bleed
                    Bleed(collision.contacts[i].point, collision.contacts[i].normal);
                }
            }
        }

        private bool Valid
        {
            get
            {
                if (prefab == null) return false;
                if (prefab.GetComponent<Collider>() == null) return false;
                if (prefab.GetComponent<Rigidbody>() == null) return false;
                if (prefab.GetComponent<Blood>() == null) return false;

                return true;
            }
        }
        private void Bleed(Vector3 Point, Vector3 Normal)
        {
            //Grab blood collider bounds
            Bounds bounds = prefab.GetComponent<Collider>().bounds;
            float offset = 1.5f * Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z);

            //Calculate position to prevent self collision
            Vector3 position = Point + Normal * offset;

            for (int i = 0; i < bleedRate; i++)
            {
                //Calculate direction
                Vector3 direction = (Normal + (Random.onUnitSphere * 0.2f)).normalized;

                //Spawn droplets
                SpawnDroplet(position, direction);
            }
            
        }
        private void SpawnDroplet(Vector3 Point, Vector3 Velocity)
        {
            //Spawn
            Blood b = Instantiate(prefab.gameObject, Point, Quaternion.identity).GetComponent<Blood>();
            b.source = this;

            //Grab rigidbody & set velocity
            Rigidbody rigidbody = b.GetComponent<Rigidbody>();
            rigidbody.velocity = Velocity;
        }
    }
}