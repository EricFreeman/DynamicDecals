using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LlockhamIndustries.ExtensionMethods;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LlockhamIndustries.Decals
{
    public class DynamicDecalSettings : ScriptableObject
    {
        //Pools
        public PoolInstance[] pools;

        //Masking
        public bool UseMaskLayers
        {
            get { return (maskMethod == DecalMaskMethod.Layer || maskMethod == DecalMaskMethod.Both); }
        }
        public DecalMaskMethod maskMethod;
        public ProjectionLayer[] Layers
        {
            get { return layers; }
            set
            {
                if (layers != value)
                {
                    layers = value;
                    CalculatePasses();
                }
            }
        }
        public List<ReplacementPass> Passes
        {
            get
            {
                return passes;
            }
        }
        public List<Material> Materials
        {
            get { return materials; }
        }

        //General
        public ShaderReplacementType Replacement
        {
            get { return replacement; }
            set
            {
                if (replacement != value)
                {
                    replacement = value;

                    DynamicDecals.System.UpdateRenderers();
                }
            }
        }

        //VR
        public bool SinglePassVR
        {
            get { return singlepassVR; }
        }

        //Backing fields
        [SerializeField]
        private ProjectionLayer[] layers;
        [SerializeField]
        private List<ReplacementPass> passes;
        [SerializeField]
        private ShaderReplacementType replacement;
        [SerializeField]
        private List<Material> materials;
        [SerializeField]
        private List<int> materialQueues;
        [SerializeField]
        private bool singlepassVR;

        //Constructor
        public DynamicDecalSettings()
        {
            //Initialize pools with default values
            ResetPools();

            //Initialize masking with default values
            ResetMasking();
        }

        //Reset
        public void ResetSettings()
        {
            //Update renderers
            DynamicDecals.System.UpdateRenderers();
        }
        public void ResetPools()
        {
            //Reset pools to default values
            pools = new PoolInstance[] { new PoolInstance("Default", null) };
        }
        public void ResetMasking()
        {
            //Mask method set to both
            maskMethod = DecalMaskMethod.Both;

            //Reset layers
            layers = new ProjectionLayer[] { new ProjectionLayer("Layer 1"), new ProjectionLayer("Layer 2"), new ProjectionLayer("Layer 3"), new ProjectionLayer("Layer 4") };
            CalculatePasses();

            //Initialize or clear material lists
            if (materials == null)
            {
                materials = new List<Material>();
                materialQueues = new List<int>();
            }
            else ClearMaterials();
            
        }
        public void ResetGeneral()
        {
            replacement = ShaderReplacementType.Standard;
        }

        //Mask layers
        public void CalculatePasses()
        {
            //Initialize / clear passes
            if (passes == null) passes = new List<ReplacementPass>();
            else passes.Clear();

            for (int i = 0; i < 32; i++)
            {
                //Generate layer vector
                Vector4 layerVector = LayerVector(i);

                //Add to passes
                AddToPasses(i, layerVector);
            }
        }

        private Vector4 LayerVector (int LayerIndex)
        {
            Vector4 vector = new Vector4(0, 0, 0, 0);

            if (layers[0].layers.Contains(LayerIndex)) vector.x = 1;
            if (layers[1].layers.Contains(LayerIndex)) vector.y = 1;
            if (layers[2].layers.Contains(LayerIndex)) vector.z = 1;
            if (layers[3].layers.Contains(LayerIndex)) vector.w = 1;

            return vector;
        }
        private void AddToPasses(int LayerIndex, Vector4 LayerVector)
        {
            //Check if we can be added to an existing pass
            for (int i = 0; i < passes.Count; i++)
            {
                if (passes[i].vector == LayerVector)
                {
                    passes[i].layers = passes[i].layers.Add(LayerIndex);
                    return;
                }
            }

            //Create a new pass with the current layer vector
            passes.Add(new ReplacementPass(LayerIndex, LayerVector));
        }

        //Mask materials
        public void AddMaterial(Material p_Material)
        {
            if (p_Material.renderQueue < 2999 && !materials.Contains(p_Material))
            {
                //Add the material and store it's original render queue
                materials.Add(p_Material);
                materialQueues.Add(p_Material.renderQueue);

                //Modify it's render queue to draw after decals
                p_Material.renderQueue = 2999;
            }
        }
        public void RemoveMaterial(Material p_Material)
        {
            //Get index of material
            int index = -1;
            for (int i = 0; i < materials.Count; i++)
            {
                if (materials[i] == p_Material)
                {
                    index = i;
                    break;
                }
            }

            //If list contained material
            if (index != -1) RemoveMaterial(index);
        }
        public void RemoveMaterial(int p_Index)
        {
            //Restore original render queue
            materials[p_Index].renderQueue = materialQueues[p_Index];

            //Remove from lists
            materials.RemoveAt(p_Index);
            materialQueues.RemoveAt(p_Index);
        }
        public void ClearMaterials()
        {
            for (int i = materials.Count - 1; i >= 0; i--)
            {
                //Restore original render queue
                materials[i].renderQueue = materialQueues[i];

                //Remove from lists
                materials.RemoveAt(i);
                materialQueues.RemoveAt(i);
            }
        }

        //VR calculation
        public void CalculateVR()
        {
            #if UNITY_EDITOR
            singlepassVR = PlayerSettings.stereoRenderingPath != StereoRenderingPath.MultiPass;
            #endif
        }
    }

    public enum ShaderReplacement { Null, SingleTarget, SinglePass, DoublePass, TriplePass, Classic };
    public enum ShaderReplacementType { Standard, VR, Mobile };
    public enum DecalMaskMethod { Layer, Material, Both, None };

    [System.Serializable]
    public class PoolInstance
    {
        public int id;
        public string title;
        public int[] limits;

        public PoolInstance(string Title, PoolInstance[] CurrentInstances)
        {
            id = UniqueID(CurrentInstances);
            title = Title;

            //15 Quality Settings maximum
            limits = new int[15];
            //Set all defaults
            for (int i = 0; i < limits.Length; i++)
            {
                limits[i] = ((i + 1) * 400);
            }
        }
        private int UniqueID(PoolInstance[] CurrentInstances)
        {
            //We use an ID instead of a name or an index to keep track of our pool as it allows us to rename and reorder pools while maintaining a hidden reference to them. 
            //Also lookup from a dictionary is faster than iterating over all pools for a given name.

            //Start at 0 (1 if not the first) and iterate upwards until we have an ID not currently in use.
            int ID = 0;
            bool Unique = false;

            if (CurrentInstances != null)
            {
                while (!Unique)
                {
                    //ID, wan't unique. Increment and check again.
                    ID++;
                    Unique = true;
                    //Start unique as true, then iterate over all instances to see if its otherwise.
                    for (int i = 0; i < CurrentInstances.Length; i++)
                    {
                        if (CurrentInstances[i] != null && ID == CurrentInstances[i].id) Unique = false;
                    }
                }
            }

            //We have a unique ID! System falls apart if we have more than 2,147,483,647 pools at once. Seems unlikely.
            return ID;
        }
    }

    [System.Serializable]
    public struct ProjectionLayer
    {
        public string name;
        public LayerMask layers;

        public ProjectionLayer(string Name)
        {
            name = Name;
            layers = 0;
        }
        public ProjectionLayer(string Name, int Layer)
        {
            name = Name;
            layers = (1 << Layer);
        }
        public ProjectionLayer(string Name, LayerMask Layers)
        {
            name = Name;
            layers = Layers;
        }
    }

    [System.Serializable]
    public class ReplacementPass
    {
        public Vector4 vector;
        public LayerMask layers;

        public ReplacementPass (LayerMask Mask, Vector4 LayerVector)
        {
            vector = LayerVector;
            layers = Mask;
        }
        public ReplacementPass(int LayerIndex, Vector4 LayerVector)
        {
            vector = LayerVector;
            layers = (1 << LayerIndex);
        }
    }
}