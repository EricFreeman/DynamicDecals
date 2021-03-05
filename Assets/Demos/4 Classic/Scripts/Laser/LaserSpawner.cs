using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Misc
{
    public class LaserSpawner : MonoBehaviour
    {
        //Inspector
        public GameObject laser;

        //Properties
        public int LaserCount
        {
            get { return laserCount; }
            set
            {
                laserCount = value;
                LaserCountChange();
            }
        }

        //Backing fields
        [SerializeField]
        private int laserCount = 1;

        [SerializeField]
        private int spawnRate = 5;

        //Generic methods
        void Awake()
        {
            lasers = new List<GameObject>();
            laserPool = new List<GameObject>();
        }
        void Start()
        {
            LaserCountChange();
        }

        //Laser pool
        private List<GameObject> lasers;
        private List<GameObject> laserPool;
        public GameObject RequestLaser()
        {
            GameObject Laser = null;

            if (laserPool.Count > 0)
            {
                //Grab our laser
                Laser = laserPool[0];

                //Enable
                Laser.SetActive(true);

                //Remove from pool
                laserPool.RemoveAt(0);
            }
            else
            {
                //Create a new laser
                Laser = (GameObject)Instantiate(laser, Vector3.zero, Quaternion.LookRotation(-Vector3.up, -Vector3.right), transform);
            }

            //Add to active lasers
            lasers.Add(Laser);

            //Position
            Laser.transform.position = transform.position;

            return Laser;
        }
        public void ReturnLaser(GameObject laser)
        {
            //Remove from active lasers
            lasers.Remove(laser);

            //Disable
            laser.SetActive(false);

            //Move to origin
            laser.transform.position = Vector3.zero;

            //Add to pool
            laserPool.Add(laser);
        }
        
        //Laser count
        public void LaserCountChange()
        {
            if (Application.isPlaying)
            {
                int lasersSpawned = 0;

                //Add as required, limited by spawn rate
                while (lasers != null && lasers.Count < laserCount && lasersSpawned < spawnRate)
                {
                    RequestLaser();
                    lasersSpawned++;
                }
                //Remove as required
                while (lasers != null && lasers.Count > laserCount)
                {
                    ReturnLaser(lasers[lasers.Count - 1]);
                }
            }
        }
    }
}