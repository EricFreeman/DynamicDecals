using UnityEngine;
using UnityEditor;
using System.Collections;

namespace LlockhamIndustries.Decals
{
    public class DecalsAssetProcessor : UnityEditor.AssetModificationProcessor
    {
        //Callback for when the project or scene is saved
        static string[] OnWillSaveAssets(string[] paths)
        {
            foreach (string path in paths)
            {
                //Scene
                if (path.Contains(".unity"))
                {
                    DynamicDecals.System.RestoreDepthTextureModes();
                }
            }
            return paths;
        }
    }
}