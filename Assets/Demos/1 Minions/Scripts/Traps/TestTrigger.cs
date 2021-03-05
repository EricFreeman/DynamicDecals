using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LlockhamIndustries.ExtensionMethods;

namespace LlockhamIndustries.Misc
{
    public class TestTrigger : MonoBehaviour
    {
        public Trap[] traps;
        public float delay = 1;

        float timeCode;
        float timeElapsed;

        private void OnTriggerStay(Collider other)
        {
            //Layer & duplicate check
            if (other.GetComponent<Selectable>())
            {
                //Update elapsed time
                if (Time.timeSinceLevelLoad - 1 > timeCode) timeElapsed = 0;
                else timeElapsed += Time.fixedDeltaTime;

                //Update time code
                timeCode = Time.timeSinceLevelLoad;

                //Trigger
                if (timeElapsed > delay)
                {
                    foreach (Trap trap in traps)
                    {
                        trap.Trigger();
                    }
                }
            }
        }
    }
}