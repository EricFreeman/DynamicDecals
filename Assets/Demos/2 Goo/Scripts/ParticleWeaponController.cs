using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Misc
{
    public class ParticleWeaponController : WeaponController
    {
        [Header("Particle Fire")]
        public ParticleSystem blueParticles;
        public ParticleSystem orangeParticles;
        public int fireRate = 250;

        [Header("Recoil")]
        public float recoil = 2;

        private ParticleSystem.EmissionModule blueEmissionModule;
        private ParticleSystem.EmissionModule orangeEmissionModule;

        public void OnEnable()
        {
            if (blueParticles != null) blueEmissionModule = blueParticles.emission;
            if (orangeParticles != null) orangeEmissionModule = orangeParticles.emission;
        }

        public override void UpdateWeapon()
        {
            base.UpdateWeapon();
            Fire();
        }
        private void Fire()
        {
            if (timeToFire == 0)
            {
                //Primary fire
                if (blueParticles != null)
                {
                    if (primary)
                    {
                        //Spawn particles
                        blueEmissionModule.rateOverTimeMultiplier = fireRate;

                        //Apply recoil
                        if (controller != null) controller.ApplyRecoil(recoil, 0.2f);
                    }
                    else
                    {
                        //Don't spawn particles
                        blueEmissionModule.rateOverTimeMultiplier = 0;
                    }
                }

                //Secondary fire
                if (orangeParticles != null)
                {
                    if (secondary)
                    {
                        //Spawn particles
                        orangeEmissionModule.rateOverTimeMultiplier = fireRate;

                        //Apply recoil
                        if (controller != null) controller.ApplyRecoil(recoil, 0.2f);
                    }
                    else
                    {
                        //Don't spawn particles
                        orangeEmissionModule.rateOverTimeMultiplier = 0;
                    }
                }
            }
        }
    }
}