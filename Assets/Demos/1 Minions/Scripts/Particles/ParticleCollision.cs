using UnityEngine;
using System.Collections;

namespace LlockhamIndustries.Misc
{
    public class ParticleCollision : MonoBehaviour
    {
        public ParticleSystem partSystem;

        void OnCollisionEnter(Collision collision)
        {
            GameObject ps = ((ParticleSystem)Instantiate(partSystem, transform.position, partSystem.transform.rotation, transform.parent)).gameObject;
            ps.name = "Splash Particles";
        }
    }
}