using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.ExtensionMethods
{
    public static class TransformExtensionMethods
    {
        public static void LocalZero(this Transform Transform)
        {
            Transform.localPosition = Vector3.zero;
            Transform.localRotation = Quaternion.identity;
            Transform.localScale = Vector3.one;
        }
        public static void WorldZero(this Transform Transform)
        {
            Transform.position = Vector3.zero;
            Transform.rotation = Quaternion.identity;
            Transform.localScale = Vector3.one;
        }
        public static void Copy(this Transform Transform, Transform Target)
        {
            Transform.position = Target.position;
            Transform.rotation = Target.rotation;
            Transform.localScale = Target.localScale;
        }

        public static Transform FindRecursively(this Transform Transform, string Name)
        {
            //Check if we the requested transform
            if (Transform.name == Name) return Transform;

            //If not check each child recursively
            foreach (Transform child in Transform)
            {
                Transform recursive = child.FindRecursively(Name);
                if (recursive != null) return recursive;
            }

            //If no valid child was found return null
            return null;
        }
        public static Transform FindParentRecursively(this Transform Transform, string Name)
        {
            //Check if we the requested transform
            if (Transform.name == Name) return Transform;

            //If not check parent recursively
            Transform recursive = Transform.parent.FindParentRecursively(Name);
            if (recursive != null) return recursive;

            //If no valid child was found return null
            return null;
        }
    }
}