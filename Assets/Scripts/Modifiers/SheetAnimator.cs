using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Decals
{
    /**
    * This component allows you to animate your projections. Attach to a projection renderer with a projection set up as a sprite sheet.
    * Designed to be printed with your projections. Attach to your prefab and enable print behaviours on your printer.
    */
    [RequireComponent(typeof(ProjectionRenderer))]
    public class SheetAnimator : MonoBehaviour
    {
        [Header("Basics")]
        /**
        * The number of collumns in the sprite sheet being sampled.
        */
        [Tooltip("The number of collumns in the sprite sheet being sampled.")]
        public int collumns = 1;

        /**
        * The number of rows in the sprite sheet being sampled.
        */
        [Tooltip("The number of rows in the sprite sheet being sampled.")]
        public int rows = 1;

        /**
        * The playback speed, in frames per second.
        */
        [Tooltip("The playback speed, in frames per second.")]
        public float speed = 30;

        [Header("Advanced")]
        /**
        * Skip the first x frames of the animation.
        */
        [Tooltip("Skip the first x frames of the animation.")]
        public int skipFirst;

        /**
        * Skip the last x frames of the animation.
        */
        [Tooltip("Skip the last x frames of the animation.")]
        public int skipLast;

        /**
        * Sample frames from the bottom instead of the top.
        */
        [Tooltip("Sample frames from the bottom instead of the top.")]
        public bool invertY;

        /**
        * Destroy the projection when the animator has finished its first loop.
        */
        [Tooltip("Destroy the projection when the animator has finished its first loop.")]
        public bool destroyOnComplete;

        //Backing fields
        private ProjectionRenderer projection;

        private float time = 0;
        private bool paused;

        private void Awake()
        {
            //Grab our projection
            projection = GetComponent<ProjectionRenderer>();
        }
        private void Update()
        {
            //Calculate count
            int count = (collumns * rows) - (skipFirst + skipLast);

            //Increment time
            if (!paused) time += Time.deltaTime * speed;
            if (time > count)
            {
                if (destroyOnComplete)
                {
                    projection.Destroy();
                    return;
                }
                else time -= count;
            }
                 
            //Calculate current frame
            int frame = skipFirst + Mathf.FloorToInt(time);

            //Calculate frame size
            Vector2 size = new Vector2(1.0f / collumns, 1.0f / rows);

            //Calculate current row & collumn
            int row = frame / collumns;
            int collumn = frame % collumns;

            //Calculate offset
            float x = size.x * collumn;
            float y = size.y * row;
            if (!invertY) y = 1 - size.y - y;

            //Set tiling
            projection.Tiling = new Vector2(size.x, size.y);

            //Set offset
            projection.Offset = new Vector2(x, y);

            //Update projection with new properties
            projection.UpdateProperties();
        }

        //Access methods
        /**
        * Plays the sprite animation.
        */
        public void Play()
        {
            paused = false;
        }
        /**
        * Pauses the sprite animation. Calling Play() will begin the animation again from the current position.
        */
        public void Pause()
        {
            paused = true;
        }
        /**
        * Stops the sprite animation. Calling Play() will begin the animation from the begining.
        */
        public void Stop()
        {
            paused = true;
            time = 0;
        }
    }
}