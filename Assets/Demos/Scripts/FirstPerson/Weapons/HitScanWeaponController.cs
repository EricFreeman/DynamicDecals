using UnityEngine;
using System.Collections;

using LlockhamIndustries.Decals;

namespace LlockhamIndustries.Misc
{
    //Hit Scan Weapon Controller
    public class HitScanWeaponController : WeaponController
    {
        [Header("Hitscan Fire")]
        public RayPrinter printer;
        public float hitScanFireRate = 1;

        public override void UpdateWeapon()
        {
            base.UpdateWeapon();
            Fire();
        }
        private void Fire()
        {
            if (timeToFire == 0)
            {
                //Hit-Scan Fire
                if ((primary || secondary) && printer != null)
                {
                    Vector3 rayPosition = cameraController.transform.position;
                    Vector3 rayDirection = cameraController.transform.forward;

                    Ray ray = new Ray(rayPosition, rayDirection);
                    printer.PrintOnRay(ray, 100, cameraController.transform.up);

                    //Apply recoil
                    if (controller != null) controller.ApplyRecoil(120, 0.2f);

                    timeToFire = 1 / hitScanFireRate;
                }
            }
        }
    }
}