using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Misc
{
    public class SpawnAction : ClampedAction
    {
        public GameObject spawnable;
        public float spawnHeight = 10;

        protected override void Perform(Vector3 point)
        {
            if (spawnable != null)
            {
                Instantiate(spawnable, point + (Vector3.up * spawnHeight), Quaternion.identity);
            }
        }
    }
}