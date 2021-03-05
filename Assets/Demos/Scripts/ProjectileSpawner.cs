using UnityEngine;
using System.Collections;

namespace LlockhamIndustries.Misc
{
    public class ProjectileSpawner : MonoBehaviour
    {
        public GameObject projectile;
        public float spawnRate = 60;
        public float spread = 0.3f;
        public Transform parent;

        public Vector3 spawnVelocity;

        //Backing fields
        private float timeToSpawn;

        //Generic methods
        void Start()
        {
            if (parent == null) parent = transform;
        }
        void Update()
        {
            //Update time to fire
            timeToSpawn = Mathf.Clamp(timeToSpawn - Time.deltaTime, 0, Mathf.Infinity);

            //Spawn
            if (timeToSpawn == 0)
            {
                Vector3 projectileDirection = Vector3.Slerp(spawnVelocity, Random.insideUnitSphere.normalized * spawnVelocity.magnitude, spread / 10);
                Quaternion projectileRotation = Quaternion.LookRotation(projectileDirection, transform.forward);

                //Spawn projectile
                GameObject spawn = (GameObject)Instantiate(projectile, transform.position, projectileRotation, parent);
                spawn.name = "Ray";

                //Setup initial velocity
                Rigidbody spawnbody = spawn.GetComponent<Rigidbody>();
                spawnbody.AddForce(projectileDirection, ForceMode.VelocityChange);

                timeToSpawn = 1 / spawnRate;
            }
        }
    }
}