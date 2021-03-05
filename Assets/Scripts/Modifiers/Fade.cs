using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Decals
{
    /**
    * Fades attached projection in and out over time.
    * Designed to be printed with your projections. Attach to your prefab and enable print behaviours on your printer.
    */
    [RequireComponent(typeof(ProjectionRenderer))]
    public class Fade : Modifier
    {
        //Inspector variables
        /**
        * What's being used to fade in/out your projection.
        * Alpha will adjust the alpha value, which can be used with blended transparency for a traditional fade, or cutout transparency for a dissolve.
        * Scale will adjust the scale, allowing you to shrink your projection in and out.
        * Both will adjust both the alpha value and scale of your projection
        */
        public FadeType type;
        /**
        * Determines what the fade method actually does. Are you fading the projection in, out, making it throb?
        * Once is the traditional fade out. This will play through the fade curve once and then destroy the projection.
        * Clamp is the traditional fade in. This will play through the fade curve once, then never touch your projection again.
        * Loop can be used to make a projection appear to throb. This will play through the fade curve repeatedly.
        * PingPong, can be used to make your projection appear to throb in a different manner. This will play through your fade curve, then through it backwards, then forwards again, repeating indefinitely.
        */
        public FadeWrapMode wrapMode;

        /**
        * Determines the fade value over time. 
        * A curve starting at 0 and ending at 1 would have your projection fade in.
        * A curve starting at 1 and ending at 0 would have your projection fade out.
        * A curve starting and ending at 0, but peaking at 1 in the center will have your projection fade in and out.
        */
        public AnimationCurve fade = AnimationCurve.EaseInOut(0,1,1,0);
        /**
        * Determines how long the fade takes, in seconds. Cannot be set to 0.
        */
        public float fadeLength = 1;

        //Backing fields
        private ProjectionRenderer projection;
        
        private float timeElapsed = 0;
        private bool executing;

        float originalAlpha;
        Vector3 originalScale;

        private void Awake()
        {
            //Grab our projection
            projection = GetComponent<ProjectionRenderer>();
        }

        protected override void Begin()
        {
            //Reset time elapsed
            timeElapsed = 0;
            executing = true;

            //Cache alpha & scale
            originalAlpha = GetAlpha(projection);
            originalScale = projection.transform.localScale;

            //Setup wrap mode
            switch (wrapMode)
            {
                case FadeWrapMode.Loop:
                    fade.postWrapMode = WrapMode.Loop;
                    break;
                case FadeWrapMode.PingPong:
                    fade.postWrapMode = WrapMode.PingPong;
                    break;
                case FadeWrapMode.Once:
                case FadeWrapMode.Clamp:
                    fade.postWrapMode = WrapMode.ClampForever;
                    break;
            }

            //Apply original fade
            ApplyFade(projection, 0);
        }
        public override void Perform()
        {
            //Execution check
            if (!executing || fade == null) return;

            //Perform fade
            if ((wrapMode != FadeWrapMode.Clamp && wrapMode != FadeWrapMode.Once) || timeElapsed <= fadeLength)
            {
                //Update time elapsed
                timeElapsed += UpdateRate;

                //Apply fade
                ApplyFade(projection, timeElapsed / fadeLength);
                return;
            }

            //Apply final value
            if (wrapMode == FadeWrapMode.Clamp) ApplyFade(projection, 1);
            
            //Destroy
            if (wrapMode == FadeWrapMode.Once) projection.Destroy();

            //No longer executing
            executing = false;
        }

        private void ApplyFade(ProjectionRenderer Projection, float Time)
        {
            //Calculate fade value
            float fadeValue = fade.Evaluate(Time);

            //Update projection
            switch (type)
            {
                case FadeType.Alpha:
                    SetAlpha(Projection, originalAlpha * fadeValue);
                    break;
                case FadeType.Scale:
                    SetScale(Projection, originalScale * fadeValue);
                    break;
                case FadeType.Both:
                    SetAlpha(Projection, originalAlpha * fadeValue);
                    SetScale(Projection, originalScale * fadeValue);
                    break;
            }
        }
        private float GetAlpha(ProjectionRenderer Projection)
        {
            switch (Projection.Properties[0].type)
            {
                case PropertyType.Float:
                    return Projection.Properties[0].value;
                case PropertyType.Color:
                    return Projection.Properties[0].color.a;
                case PropertyType.Combo:
                    return Projection.Properties[0].value;
            }
            return 1;
        }
        private void SetAlpha(ProjectionRenderer Projection, float Alpha)
        {
            switch (Projection.Properties[0].type)
            {
                case PropertyType.Float:
                case PropertyType.Combo:
                    Projection.SetFloat(0, Alpha);
                    break;

                case PropertyType.Color:
                    Color color = Projection.Properties[0].color;
                    color.a = Alpha;
                    Projection.SetColor(0, color);
                    break;
            }

            Projection.UpdateProperties();
        }
        private void SetScale(ProjectionRenderer Projection, Vector3 Scale)
        {
            Projection.transform.localScale = Scale;
        }
    }

    public enum FadeType { Alpha, Scale, Both};
    public enum FadeWrapMode { Once, Clamp, Loop, PingPong };
}