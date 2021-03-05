using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Misc
{
    public class Cannon : MonoBehaviour
    {
        [Header("References")]
        public GameObject ball;
        public Rigidbody barrel;
        public ParticleSystem particles;

        [Header("Firing")]
        public Vector3 offset;
        public Vector3 velocity = new Vector3(0, -10, 0);
        public float fireRate = 0.25f;
        
        private float timeElapsed;

        private void OnEnable()
        {
            //Start fire routine
            StartCoroutine(FireRoutine());
        }
        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator FireRoutine()
        {
            while (true)
            {
                //Increment time elapsed
                timeElapsed += Time.fixedDeltaTime;

                if (timeElapsed > 1 / fireRate)
                {
                    //Fire!!
                    Fire();

                    //Reduce time elapsed
                    timeElapsed -= 1 / fireRate;
                }

                yield return new WaitForFixedUpdate();
            }
        }
        private void Fire()
        {
            //Play particle effect
            if (particles != null) particles.Play();
            
            if (barrel != null && ball != null)
            {
                //Instantiate cannonball
                GameObject cannonBall = Instantiate(ball, barrel.transform.position + barrel.transform.rotation * offset, Quaternion.identity, transform);
                Rigidbody crb = cannonBall.GetComponent<Rigidbody>();

                //Calculate velocity
                Vector3 ballVelocity = barrel.transform.rotation * velocity;

                //Give cannonball velocity
                crb.velocity = ballVelocity;

                //Calculare barrel velocity
                Vector3 barrelVelocity = -ballVelocity * (crb.mass / barrel.mass);

                //Apply equal force against barrel
                barrel.velocity = barrelVelocity;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (barrel != null)
            {
                Vector3 position = barrel.transform.position + barrel.transform.rotation * offset;
                Vector3 direction = barrel.transform.rotation * velocity;

                Gizmos.DrawWireSphere(position, 0.2f);
                Gizmos.DrawRay(position, direction);
            }
        }
    }
}