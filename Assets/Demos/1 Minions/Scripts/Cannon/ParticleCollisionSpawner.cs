using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LlockhamIndustries.ExtensionMethods;

namespace LlockhamIndustries.Misc
{
    [RequireComponent(typeof(Rigidbody))]
    public class ParticleCollisionSpawner : MonoBehaviour
    {
        [Header("Particle System")]
        public ParticleSystem particles;

        [Header("Conditions")]
        public float requiredVelocity = 10;
        public LayerMask layers;

        [Header("Pool Parent")]
        public Transform parent;

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (particles != null && rb.velocity.magnitude > requiredVelocity)
            {
                for (int i = 0; i < collision.contacts.Length; i++)
                {
                    if (layers.Contains(collision.contacts[i].otherCollider.gameObject.layer))
                    {
                        ParticleSystem p = null;

                        if (parent != null) p = Instantiate(particles, collision.contacts[i].point, Quaternion.LookRotation(collision.contacts[i].normal), parent);
                        else p = Instantiate(particles, collision.contacts[i].point, Quaternion.LookRotation(collision.contacts[i].normal), transform);

                        p.Play();
                    }
                }
            }
        }
    }
}