using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Decals
{
    /**
    * Randomizes the initial scale of your projection.
    * Designed to be printed with your projections. Attach to your prefab and enable print behaviours on your printer.
    */
    public class RandomScale : MonoBehaviour
    {
        /**
        * The minimum range of the randomized scale (in units).
        */
        public float minSize = 0.5f;
        /**
        * The maximum range of the randomized scale (in units).
        */
        public float maxSize = 0.8f;

        private void Awake()
        {
            //Scale projection in
            float scale = Random.Range(minSize, maxSize);
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}