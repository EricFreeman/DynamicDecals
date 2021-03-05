using UnityEngine;
using System.Collections;

namespace LlockhamIndustries.Decals
{
    /**
    * This component blocks a camera from rendering decals. Useful if you only want some of your cameras to render decals in the scene.
    */
    [RequireComponent(typeof(Camera))]
    public class ProjectionBlocker : MonoBehaviour
    {
        //An empty component recognized by the system. Must be enabled to work correctly.
    }
}