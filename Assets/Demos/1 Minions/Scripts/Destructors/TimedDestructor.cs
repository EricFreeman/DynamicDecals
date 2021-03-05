using UnityEngine;
using System.Collections;

namespace LlockhamIndustries.Misc
{
    public class TimedDestructor : MonoBehaviour
    {
        public float time = 10;
        private float t = 0;

        void Update()
        {
            t += Time.deltaTime;
            if (t > time)
            {
                Destroy(gameObject);
            }
        }
    }
}