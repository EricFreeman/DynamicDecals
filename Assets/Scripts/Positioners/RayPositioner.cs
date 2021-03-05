using UnityEngine;
using System.Collections;

namespace LlockhamIndustries.Decals
{
    /**
* The ray positioner component. Positions a projection at the hit point of a raycast. The ray is created starting at a transforms position and casts in the transforms forward direction.
*/
    public class RayPositioner : Positioner
    {
        /**
        * The transform that acts as the base of the raycast. If null will this objects transform
        */
        public Transform rayTransform;
        /**
        * A position offset applied to the base of the transform to get the starting position of the ray.
        */
        public Vector3 positionOffset;
        /**
        * A rotation offset applied to the transforms forward direction to get the direction of the ray.
        */
        public Vector3 rotationOffset;
        /**
        * The cast length of the ray.
        */
        public float castLength = 100;

        void LateUpdate()
        {
            Transform origin = (rayTransform != null) ? rayTransform : transform;
            Quaternion Rotation = origin.rotation * Quaternion.Euler(rotationOffset);
            Vector3 Position = origin.position + (Rotation * positionOffset);

            //Reproject every late update
            Ray ray = new Ray(Position, Rotation * Vector3.forward);
            Reproject(ray, castLength, Rotation * Vector3.up);
        }
        void OnDrawGizmosSelected()
        {
            Transform origin = (rayTransform != null) ? rayTransform : transform;
            Quaternion Rotation = origin.rotation * Quaternion.Euler(rotationOffset);
            Vector3 Position = origin.position + (Rotation * positionOffset);

            Gizmos.color = Color.black;
            Gizmos.DrawRay(Position, Rotation * Vector3.up * 0.4f);

            Gizmos.color = Color.white;
            Gizmos.DrawRay(Position, Rotation * Vector3.forward * castLength);
        }
    }
}