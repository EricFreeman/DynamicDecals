using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Misc
{
    public class LocomotionAction : ClampedAction
    {
        public LocomotionTarget locomotion;

        protected override void Perform(Vector3 point)
        {
            if (locomotion != null)
            {
                locomotion.GoalPosition = point;
            }
        }
    }
}