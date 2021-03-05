using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.ExtensionMethods
{
    public static class LayerMaskExtensionMethods
    {
        public static bool Contains(this LayerMask Mask, int Layer)
        {
            return (Mask == (Mask | (1 << Layer)));
        }

        public static LayerMask Remove(this LayerMask Mask, int Layer)
        {
            return Mask & ~(1 << Layer);
        }
        public static LayerMask Remove(this LayerMask Mask, LayerMask Layers)
        {
            return Mask & ~Layers;
        }

        public static LayerMask Add(this LayerMask Mask, int Layer)
        {
            Mask |= (1 << Layer);
            return Mask;
        }
        public static LayerMask Add(this LayerMask Mask, LayerMask Layers)
        {
            Mask |= Layers;
            return Mask;
        }

        public static int[] ContainedLayers(this LayerMask Mask)
        {
            List<int> layers = new List<int>();

            for (int i = 0; i < 32; i++)
            {
                if (Mask.Contains(i)) layers.Add(i);
            }

            return layers.ToArray();
        }
        public static string[] ContainedLayerNames(this LayerMask Mask)
        {
            List<string> layers = new List<string>();

            for (int i = 0; i < 32; i++)
            {
                if (Mask.Contains(i)) layers.Add(LayerMask.LayerToName(i));
            }

            return layers.ToArray();
        }

        public static void LogLayers(this LayerMask Mask)
        {
            foreach (int index in Mask.ContainedLayers()) Debug.Log(index);
        }
        public static void LogLayerNames(this LayerMask Mask)
        {
            foreach (string name in Mask.ContainedLayerNames()) Debug.Log(name);
        }
    }
}