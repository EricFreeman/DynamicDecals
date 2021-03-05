using UnityEngine;
using System.Collections;

namespace LlockhamIndustries.Misc
{
    public class VSyncDisable : MonoBehaviour
    {
        void Start()
        {
            QualitySettings.vSyncCount = 0;
        }
    }
}