using UnityEngine;
using System.Collections;

namespace LlockhamIndustries.Decals
{
    /**
    * The RayPrinter Component. Given a ray, prints a copy of a projection at the ray collision point.
    */
    public class RayPrinter : Printer
    {

        /**
        * The layers the raycast collide with, this should be set in editor or once at initialization.
        */
        public LayerMask layers;

        /**
        * Prints immediately at the hit position of a raycast, with a rotation relative to the normal of the surface hit.
        * This should be used in situations where you want to define when and how you print. Bullet-holes or laser-burns for a hitscan weapon would be perfect use-cases for this printer.
        * @param Ray The ray to be cast, to determine the print position and rotation.
        * @param RayLength The length of the ray being cast.
        * @param DecalUp The rotation of the decal relative to the surface. By default, points upwards.
        */
        public void PrintOnRay(Ray Ray, float RayLength, Vector3 DecalUp = default(Vector3))
        {
            //Must have a rotation
            if (DecalUp == Vector3.zero) DecalUp = Vector3.up;

            RaycastHit hit;
            if (Physics.Raycast(Ray, out hit, RayLength, layers.value))
            {
                Print(hit.point, Quaternion.LookRotation(-hit.normal, DecalUp), hit.transform, hit.collider.gameObject.layer);
            }
        }
    }
}