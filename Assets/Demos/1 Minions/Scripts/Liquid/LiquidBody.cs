using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LlockhamIndustries.Decals;

namespace LlockhamIndustries.Misc
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class LiquidBody : MonoBehaviour
    {
        [Header("Collision ripples")]
        public ProjectionRenderer ripple;

        [Header("Tide")]
        public float tidalHeight = 1f;
        public float tidalScale = 1.2f;

        public float speed = 10;
        public AnimationCurve curve;

        private float tidalPosition;

        private Vector3 initialPosition;
        private Vector3 initialScale;

        private void Awake()
        {
            initialPosition = transform.position;
            initialScale = transform.localScale;
        }
        private void FixedUpdate()
        {
            curve.postWrapMode = WrapMode.PingPong;

            //Increment tidal position
            tidalPosition += (Time.fixedDeltaTime / speed);

            //Evaluate curve
            float targetPosition = curve.Evaluate(tidalPosition);

            //Adjust height
            transform.position = initialPosition + (Vector3.up * tidalHeight * targetPosition);

            //Adjust scale
            transform.localScale = initialScale * (Mathf.Lerp(1, tidalScale, targetPosition));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (ripple != null && other.GetComponent<Rigidbody>() != null)
            {
                ProjectionRenderer proj = Instantiate(ripple);

                //Set pos / rot
                proj.transform.position = other.transform.position;
                proj.transform.rotation = Quaternion.LookRotation(Vector3.down);

                //Set scale
                Bounds bounds = other.bounds;
                proj.transform.localScale = Vector3.one * (Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z));

                //Set parent
                proj.transform.parent = transform;
            }
        }
    }
}