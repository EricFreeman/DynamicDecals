using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LlockhamIndustries.ExtensionMethods;

namespace LlockhamIndustries.Misc
{
    public class Ragdoll : MonoBehaviour
    {
        [Header("GameObjects")]
        public GameObject ragdoll;
        public GameObject chunkdoll;

        [Header("Particles")]
        public ParticleSystem ragParticles;
        public ParticleSystem chunkParticles;

        [Header("Layers")]
        public LayerMask triggerLayers;

        [Header("Ragdoll Triggers")]
        public float ragVelocity = 10;

        [Header("Chunkdoll Triggers")]
        public float chunkVelocity = 50;
        public float chunkAngle = 35;

        private void OnCollisionEnter(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                //Grab rigidbody
                Rigidbody rigidbody = contact.otherCollider.GetComponent<Rigidbody>();

                if (rigidbody != null && rigidbody.velocity.magnitude > chunkVelocity && triggerLayers.Contains(rigidbody.gameObject.layer))
                {
                    if (rigidbody.velocity.magnitude > chunkVelocity && Vector3.Angle(rigidbody.velocity, contact.normal) < chunkAngle)
                    {
                        //Trigger chunkdoll
                        TriggerChunkdoll(rigidbody.mass, rigidbody.velocity);

                        //Grab first chunk
                        Transform chunk = chunkdoll.transform.GetChild(0);

                        //Spawn & play particles
                        SpawnParticles(chunkParticles, contact.point, contact.normal, chunk);
                        return;
                    }
                    if (rigidbody.velocity.magnitude > ragVelocity)
                    {
                        //Trigger ragdoll
                        TriggerRagdoll(rigidbody.mass, rigidbody.velocity);

                        //Spawn & play particles
                        SpawnParticles(ragParticles, contact.point, contact.normal, ragdoll.transform);
                        return;
                    }
                }
            }
        }

        private void TriggerRagdoll(float ExternalMass, Vector3 ExternalVelocity)
        {
            //Disable our collider
            Collider collider = GetComponent<Collider>();
            if (collider != null) collider.enabled = false;

            //Spawn ragdoll
            ragdoll = Instantiate(ragdoll, transform.position, transform.rotation, transform.parent);

            //Position ragdoll
            SyncTransformRecursively(ragdoll.transform, transform);

            //Calculate mass
            float currentMass = CalculateMassRecursively(ragdoll.transform);

            //Calculate velocity
            float lerp = ExternalMass / (ExternalMass + currentMass);
            Vector3 velocity = Vector3.Lerp(Vector3.zero, ExternalVelocity, lerp);

            //Set velocity
            SetVelocityRecursively(ragdoll.transform, velocity);

            //Destroy ourself
            Destroy(gameObject);
        }
        private void TriggerChunkdoll(float ExternalMass, Vector3 ExternalVelocity)
        {
            //Disable our collider
            Collider collider = GetComponent<Collider>();
            if (collider != null) collider.enabled = false;

            //Spawn chunkdoll
            chunkdoll = Instantiate(chunkdoll, transform.position, transform.rotation, transform.parent);

            //Calculate mass
            float currentMass = CalculateMassRecursively(chunkdoll.transform);

            //Calculate velocity
            float lerp = ExternalMass / (ExternalMass + currentMass);
            Vector3 velocity = Vector3.Lerp(Vector3.zero, ExternalVelocity, lerp);

            //Set velocity
            SetVelocityRecursively(chunkdoll.transform, velocity * 2);

            //Destroy ourself
            Destroy(gameObject);
        }

        private void SpawnParticles(ParticleSystem Particles, Vector3 Position, Vector3 Normal, Transform Parent)
        {
            if (Particles != null)
            {
                ParticleSystem p = Instantiate(Particles, Position, Quaternion.LookRotation(Normal), Parent);
                p.name = Particles.name;
                p.Play();
            }
        }

        private void SyncTransformRecursively(Transform Transform, Transform Target)
        {
            Transform.localPosition = Target.localPosition;
            Transform.localRotation = Target.localRotation;

            foreach (Transform child in Transform)
            {
                Transform target = Target.Find(child.name);
                SyncTransformRecursively(child, target);
            }
        }
        private float CalculateMassRecursively(Transform Transform)
        {
            float mass = 0;

            //Add rigidbody mass
            Rigidbody rigidbody = Transform.GetComponent<Rigidbody>();
            if (rigidbody != null) mass += rigidbody.mass;

            //Add child rigidbodies mass
            foreach (Transform child in Transform) mass += CalculateMassRecursively(child);

            return mass;
        }
        private void SetVelocityRecursively(Transform Transform, Vector3 Velocity)
        {
            Rigidbody rigidbody = Transform.GetComponent<Rigidbody>();
            if (rigidbody != null) rigidbody.velocity += Vector3.Slerp(Velocity , Random.rotationUniform.eulerAngles, 0.1f);

            foreach (Transform child in Transform)
            {
                SetVelocityRecursively(child, Velocity);
            }
        }
    }
}