using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LlockhamIndustries.Decals;

namespace LlockhamIndustries.Misc
{
    [RequireComponent(typeof(ProjectionRenderer))]
    public class SceneFadeIn : MonoBehaviour
    {
        public float holdTime;
        public float inTime;

        private ProjectionRenderer projectionRenderer;

        private void OnEnable()
        {
            //Grab projection renderer
            projectionRenderer = GetComponent<ProjectionRenderer>();
            projectionRenderer.enabled = true;

            //Fade projection out
            StartCoroutine(FadeIn());
        }
        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator FadeIn()
        {
            float timeElapsed = 0;

            //Get color
            Color color = projectionRenderer.Properties[0].color;

            while (timeElapsed < holdTime + inTime)
            {
                //Adjust time elapsed
                timeElapsed += Time.deltaTime;

                //Calculate alpha modifier
                float modifier = Mathf.Pow(Mathf.Clamp01(1 - ((timeElapsed - holdTime) / inTime)), 0.6f);

                //Apply color
                projectionRenderer.SetColor(0, Color.Lerp(Color.white, color, modifier));
                projectionRenderer.UpdateProperties();

                yield return new WaitForEndOfFrame();
            }

            //Destory projection after fade
            Destroy(gameObject);
        }
    }
}