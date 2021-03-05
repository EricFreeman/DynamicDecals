using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.VR
{
    public abstract class VRController : MonoBehaviour
    {
        //Axes
        string grip = "";
        string trigger = "";
        string trackpad = "";
        string trackpadHorizontal = "";
        string trackpadVertical = "";

        //Controller Orientation
        public Hand hand;

        //Input
        protected bool Grip
        {
            get
            {
                if (grip == "") return false;
                else return (Input.GetAxis(grip) != 0);
                
            }
        }
        protected bool Trigger
        {
            get
            {
                if (trigger == "") return false;
                else return (Input.GetAxis(trigger) != 0);
            }
        }
        protected bool TrackPad
        {
            get
            {
                if (trackpad == "") return false;
                else return (Input.GetAxis(trackpad) != 0);
            }
        }
        protected Vector2 TrackPadPosition
        {
            get
            {
                if (trackpadHorizontal == "") return Vector2.zero;
                else return new Vector2(Input.GetAxis(trackpadHorizontal), Input.GetAxis(trackpadVertical));
                
            }
        }

        protected void OnEnable()
        {
            //TestAxis();
        }
        private void Start()
        {
            TestAxis();
        }
        private void TestAxis()
        {
            switch (hand)
            {
                case Hand.Left:
                    //Grip
                    try { Input.GetAxis("axis 11"); grip = "axis 11"; }
                    catch (System.ArgumentException) { }

                    //Trigger
                    try { Input.GetAxis("axis 14"); trigger = "axis 14"; }
                    catch (System.ArgumentException) { }

                    //Trackpad
                    try { Input.GetAxis("axis 16"); trigger = "axis 16"; }
                    catch (System.ArgumentException) { }

                    //Trackpad touch
                    try
                    {
                        Input.GetAxis("axis 1"); trackpadHorizontal = "axis 1";
                        Input.GetAxis("axis 2"); trackpadVertical = "axis 2";
                    }
                    catch (System.ArgumentException) { }
                    break;

                case Hand.Right:
                    //Grip
                    try { Input.GetAxis("axis 12"); grip = "axis 12"; }
                    catch (System.ArgumentException) { }

                    //Trigger
                    try { Input.GetAxis("axis 15"); trigger = "axis 15"; }
                    catch (System.ArgumentException) { }

                    //Trackpad
                    try { Input.GetAxis("axis 16"); trigger = "axis 17"; }
                    catch (System.ArgumentException) { }

                    //Trackpad touch
                    try
                    {
                        Input.GetAxis("axis 4"); trackpadHorizontal = "axis 4";
                        Input.GetAxis("axis 5"); trackpadVertical = "axis 5";
                    }
                    catch (System.ArgumentException) { }
                    break;
            }

        }
    }
}