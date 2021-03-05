using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Misc
{
    public class AnimatedScale : MonoBehaviour
    {
        public float desiredScale = 2;
        public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public float speed = 1;

        private Vector3 initialScale;
        private float sampleTime;

        private void OnEnable()
        {
            StartCoroutine(Scale());
        }
        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator Scale()
        {
            yield return new WaitForFixedUpdate();

            initialScale = transform.localScale;

            while (sampleTime < 1)
            {
                //Increment time elapsed
                sampleTime = Mathf.MoveTowards(sampleTime, 1, Time.fixedDeltaTime / speed);

                //Adjust scale
                float scaleModifier = Mathf.Lerp(1, desiredScale, curve.Evaluate(sampleTime));
                transform.localScale = initialScale * scaleModifier;

                yield return new WaitForFixedUpdate();
            }
        }
    }
}