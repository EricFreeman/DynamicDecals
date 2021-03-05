using UnityEngine;
using System.Collections;

using LlockhamIndustries.Decals;

namespace LlockhamIndustries.Misc
{
    //Example Weapon Controller
    [ExecuteInEditMode]
    public class ExampleWeaponController : WeaponController
    {
        [Header("Projectile Parent")]
        public Transform parent;

        [Header("Ray Projectile Fire")]
        public GameObject rayProjectile;
        public Vector3 raySourceOffset;
        public float rayFireRate = 60;
        public float raySpread = 0.3f;
        public float raySpeed = 40;

        [Header("Collision Projectile Fire")]
        public GameObject colliderProjectile;
        public Vector3 colliderSourceOffset;
        public float colliderFireRate = 3;
        public float colliderSpread = 0.1f;
        public float colliderSpeed = 20;

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
                //Ray Projectile Fire
                if (primary && rayProjectile != null)
                {
                    Vector3 projectileDirection = Vector3.Slerp(transform.up, Random.insideUnitSphere.normalized, raySpread / 10);
                    Quaternion projectileRotation = Quaternion.LookRotation(projectileDirection, transform.forward);

                    //Spawn projectile
                    GameObject fire = (GameObject)Instantiate(rayProjectile, transform.TransformPoint(raySourceOffset), projectileRotation, parent);
                    fire.name = "Ray";

                    //Setup initial velocity
                    Rigidbody firebody = fire.GetComponent<Rigidbody>();
                    if (controller != null) firebody.velocity = controller.GetComponent<Rigidbody>().velocity;
                    firebody.AddForce(projectileDirection * raySpeed, ForceMode.VelocityChange);

                    //Apply recoil
                    if (controller != null) controller.ApplyRecoil(20, 0.2f);

                    timeToFire = 1 / rayFireRate;
                }

                //Collision Projectile Fire
                if (secondary && colliderProjectile != null)
                {
                    Vector3 projectileDirection = Vector3.Slerp(transform.up, Random.insideUnitSphere.normalized, colliderSpread / 10);
                    Quaternion projectileRotation = Quaternion.LookRotation(projectileDirection, transform.forward);

                    //Spawn projectile
                    GameObject fire = (GameObject)Instantiate(colliderProjectile, transform.TransformPoint(colliderSourceOffset), projectileRotation, parent);
                    fire.name = "Collider";

                    //Setup initial velocity
                    Rigidbody firebody = fire.GetComponent<Rigidbody>();
                    if (controller != null) firebody.velocity = controller.GetComponent<Rigidbody>().velocity;
                    firebody.AddForce(projectileDirection * colliderSpeed, ForceMode.VelocityChange);

                    //Apply recoil
                    if (controller != null) controller.ApplyRecoil(100, 0.3f);

                    timeToFire = 1 / colliderFireRate;
                }

                //Hit-Scan Fire
                if (alternate && printer != null)
                {
                    if (cameraController == null)
                    {
                        Debug.Log("No Camera Set! Please set a camera for the weapon to aim with");
                        return;
                    }

                    Vector3 rayPosition = cameraController.transform.position;
                    Vector3 rayDirection = cameraController.transform.forward;

                    Ray ray = new Ray(rayPosition, rayDirection);
                    printer.PrintOnRay(ray, 100, cameraController.transform.up);

                    //Apply recoil
                    if (controller != null) controller.ApplyRecoil(200, 0.4f);

                    timeToFire = 1 / hitScanFireRate;
                }
            }
        }
    }
}