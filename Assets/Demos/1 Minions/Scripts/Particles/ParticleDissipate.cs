using UnityEngine;
using System.Collections;

namespace LlockhamIndustries.Misc
{
    public class ParticleDissipate : MonoBehaviour
    {
        private ParticleSystem partSystem;

        void Start()
        {
            partSystem = GetComponent<ParticleSystem>();
        }
        void Update()
        {
            if (partSystem != null && !partSystem.IsAlive()) Destroy(gameObject);
        }
    }
}