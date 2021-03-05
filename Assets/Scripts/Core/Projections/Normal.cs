using UnityEngine;
using System.Collections;
using System;

namespace LlockhamIndustries.Decals
{
    /**
    * Deferred Only normal projection. Only affects the normal buffer. Useful for adding cracks or normal details to tiled surfaces.
    */
    [System.Serializable]
    public class Normal : Deferred
    {
        //Mobile
        private Material Mobile
        {
            get { return MaterialFromShader(Shader.Find("Projection/Decal/Mobile/Normal")); }
        }

        public override Material MobileDeferredOpaque
        {
            get { return Mobile; }
        }
        public override Material MobileDeferredTransparent
        {
            get { return Mobile; }
        }

        //Standard
        private Material Standard
        {
            get { return MaterialFromShader(Shader.Find("Projection/Decal/Standard/Normal")); }
        }

        public override Material StandardDeferredOpaque
        {
            get { return Standard; }
        }
        public override Material StandardDeferredTransparent
        {
            get { return Standard; }
        }

        //Packed
        private Material Packed
        {
            get { return MaterialFromShader(Shader.Find("Projection/Decal/Packed/Normal")); }
        }

        public override Material PackedDeferredOpaque
        {
            get { return Packed; }
        }
        public override Material PackedDeferredTransparent
        {
            get { return Packed; }
        }

        //Instanced count
        public override int InstanceLimit
        {
            get { return 500; }
        }
        protected override void Apply(Material Material)
        {
            //Apply base
            base.Apply(Material);

            //Apply shape
            shape.Apply(Material);

            //Apply normal
            normal.Apply(Material);
        }

        //Static Properties
        /**
        * The shape of your projection.
        * The r channel of these properties is used to determine the projections transparency.
        */
        public ShapePropertyGroup shape;
        /**
        * The primary color details of your projection.
        * The alpha channel of these properties is used to determine the projections transparency.
        */
        public NormalPropertyGroup normal;

        protected override void OnEnable()
        {
            //Initialize our property groups
            if (shape == null) shape = new ShapePropertyGroup(this);
            if (normal == null) normal = new NormalPropertyGroup(this);

            base.OnEnable();
        }
        protected override void GenerateIDs()
        {
            base.GenerateIDs();

            shape.GenerateIDs();
            normal.GenerateIDs();
        }

        //Instanced Properties
        public override void UpdateProperties()
        {
            //Initialize property array
            if (properties == null || properties.Length != 2) properties = new ProjectionProperty[2];

            //Shape Modifier
            properties[0] = new ProjectionProperty("Opacity", shape._Multiplier, shape.Multiplier);

            //Normal Strength
            properties[1] = new ProjectionProperty("Normal Strength", normal._BumpScale, normal.Strength);
        }

        //Materials
        protected Material[] deferredMaterials;
    }
}
