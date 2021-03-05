using UnityEngine;
using System.Collections;
using System;

namespace LlockhamIndustries.Decals
{
    /**
    * Deferred Only gloss projection. Only affects the gloss channel of the deferred buffers. Useful for making things wetter or rougher.
    */
    [System.Serializable]
    public class Gloss : Deferred
    {
        /**
        * Defines how the gloss modifcation affects the surface.
        * Shine will have the decal shine the surface it's applied too. Great for making surfaces appear wet.
        * Dull will have the decal dull the surface it's applied too. Great for making surfaces appear worn or weathered.
        */
        public GlossType GlossType
        {
            get { return glossType; }
            set
            {
                if (glossType != value)
                {
                    glossType = value;
                    Mark();
                }
            }
        }

        //Mobile
        private Material Mobile
        {
            get
            {
                //Find relevant shader
                Shader shader = null;
                switch (glossType)
                {
                    case GlossType.Shine:
                        shader = Shader.Find("Projection/Decal/Mobile/Wet");
                        break;
                    case GlossType.Dull:
                        shader = Shader.Find("Projection/Decal/Mobile/Dry");
                        break;
                }
                return MaterialFromShader(shader);
            }
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
            get
            {
                //Find relevant shader
                Shader shader = null;
                switch (glossType)
                {
                    case GlossType.Shine:
                        shader = Shader.Find("Projection/Decal/Standard/Wet");
                        break;
                    case GlossType.Dull:
                        shader = Shader.Find("Projection/Decal/Standard/Dry");
                        break;
                }
                return MaterialFromShader(shader);
            }
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
            get
            {
                //Find relevant shader
                Shader shader = null;
                switch (glossType)
                {
                    case GlossType.Shine:
                        shader = Shader.Find("Projection/Decal/Packed/Wet");
                        break;
                    case GlossType.Dull:
                        shader = Shader.Find("Projection/Decal/Packed/Dry");
                        break;
                }
                return MaterialFromShader(shader);
            }
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

            //Apply metallic
            gloss.Apply(Material);
        }

        //Static Properties
        [SerializeField]
        public GlossType glossType;

        /**
        * The primary color details of your projection.
        * The alpha channel of these properties is used to determine the projections transparency.
        */
        public GlossPropertyGroup gloss;

        protected override void OnEnable()
        {
            //Initialize our property groups
            if (gloss == null) gloss = new GlossPropertyGroup(this);

            base.OnEnable();
        }
        protected override void GenerateIDs()
        {
            base.GenerateIDs();

            gloss.GenerateIDs();
        }

        //Instanced Properties
        public override void UpdateProperties()
        {
            //Initialize property array
            if (properties == null || properties.Length != 1) properties = new ProjectionProperty[1];

            //Normal Strength
            properties[0] = new ProjectionProperty("Glossiness", gloss._Glossiness, gloss.Glossiness);
        }
    }

    public enum GlossType { Shine, Dull };
}
