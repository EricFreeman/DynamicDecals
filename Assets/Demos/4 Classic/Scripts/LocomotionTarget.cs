using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Misc
{
    [RequireComponent(typeof(Locomotion))]
    public class LocomotionTarget : MonoBehaviour
    {
        public float brakeDistance = 0.4f;

        public Vector3 GoalPosition
        {
            set { goalPosition = value; }
        }
        private Vector3 goalPosition;

        private Locomotion locomotion;

        private void Start()
        {
            locomotion = GetComponent<Locomotion>();
            goalPosition = transform.position;
        }
        private void Update()
        {
            if (Vector3.Distance(transform.position, goalPosition) > brakeDistance)
            {
                Vector3 dir = (transform.position - goalPosition).normalized;

                locomotion.Direction = dir;
                locomotion.Movement = dir;
            }
            else
            {
                locomotion.Movement = Vector3.zero;
            }
        }
    }
}