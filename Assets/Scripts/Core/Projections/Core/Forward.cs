using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LlockhamIndustries.Decals
{
    /**
    * The base of all unlit forward projections (Unlit, Additive, Multiplicative)
    */
    [System.Serializable]
    public abstract class Forward : Projection
    {
        //Instanced count
        public override int InstanceLimit
        {
            get { return 500; }
        }

        //Forward only
        public override RenderingPaths SupportedRendering
        {
            get { return RenderingPaths.Forward; }
        }

        //Apply
        protected override void Apply(Material Material)
        {
            //Apply base
            base.Apply(Material);

            //Apply albedo
            albedo.Apply(Material);
        }

        //Static Properties
        /**
        * The primary color details of your projection.
        * The alpha channel of these properties is used to determine the projections transparency.
        */
        public AlbedoPropertyGroup albedo;

        protected override void OnEnable()
        {
            //Initialize our property groups
            if (albedo == null) albedo = new AlbedoPropertyGroup(this);

            base.OnEnable();
        }
        protected override void GenerateIDs()
        {
            base.GenerateIDs();

            albedo.GenerateIDs();
        }

        //Instanced Properties
        public override void UpdateProperties()
        {
            //Initialize property array
            if (properties == null || properties.Length != 1) properties = new ProjectionProperty[1];

            //Albedo Color
            properties[0] = new ProjectionProperty("Albedo", albedo._Color, albedo.Color);
        }
    }
}
