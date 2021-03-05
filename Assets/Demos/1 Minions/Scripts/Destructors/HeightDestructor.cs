using UnityEngine;
using System.Collections;

namespace LlockhamIndustries.Misc
{
    public class HeightDestructor : MonoBehaviour
    {
        public float height = -10;

        void Update()
        {
            if (transform.position.y < height)
            {
                Destroy(gameObject);
            }
        }
    }
}