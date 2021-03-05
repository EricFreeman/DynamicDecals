using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Misc
{
    public class LaserGuider : MonoBehaviour
    {
        //Bounds
        public float xMin = -20;
        public float xMax = 20;
        public float zMin = -20;
        public float zMax = 20;

        //Height
        public float laserHeight = 2.2f;

        //Movement Smooth
        public float smooth = 0.6f;

        //Retarget Distance
        public float retargetDistance = 4;

        //Generate Random Position within Bounds
        private Vector3 NewPosition
        {
            get
            {
                float xPosition = Random.Range(xMin, xMax);
                float zPosition = Random.Range(zMin, zMax);

                return new Vector3(xPosition, laserHeight, zPosition);
            }
        }

        private Vector3 goalPosition;
        //private Vector3 velocity;

        private void Start()
        {
            goalPosition = NewPosition;
        }
        private void Update()
        {
            //Check if we are close to our goal position
            if (Vector3.Distance(transform.position, goalPosition) <= retargetDistance)
            {
                //If so, get a new goal position, need to keep moving
                goalPosition = NewPosition;
            }
            //Move towards our goal position
            //transform.position = Vector3.SmoothDamp(transform.position, goalPosition, ref velocity, smooth);
            transform.position = Vector3.MoveTowards(transform.position, goalPosition, smooth * Time.deltaTime);
        }
    }
}