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
    * Standard Shader - specular setup.
    */
    [System.Serializable]
    public class Specular : Projection
    {
        //Mobile
        public override Material MobileForward
        {
            get { return MaterialFromShader(Shader.Find("Projection/Decal/Mobile/Specular/Forward")); }
        }
        public override Material MobileDeferredOpaque
        {
            get { return MaterialFromShader(Shader.Find("Projection/Decal/Mobile/Specular/DeferredOpaque")); }
        }
        public override Material MobileDeferredTransparent
        {
            get { return MaterialFromShader(Shader.Find("Projection/Decal/Mobile/Specular/DeferredTransparent")); }
        }

        //Standard
        public override Material StandardForward
        {
            get { return MaterialFromShader(Shader.Find("Projection/Decal/Standard/Specular/Forward")); }
        }
        public override Material StandardDeferredOpaque
        {
            get { return MaterialFromShader(Shader.Find("Projection/Decal/Standard/Specular/DeferredOpaque")); }
        }
        public override Material StandardDeferredTransparent
        {
            get { return MaterialFromShader(Shader.Find("Projection/Decal/Standard/Specular/DeferredTransparent")); }
        }

        //Packed
        public override Material PackedForward
        {
            get { return MaterialFromShader(Shader.Find("Projection/Decal/Packed/Specular/Forward")); }
        }
        public override Material PackedDeferredOpaque
        {
            get { return MaterialFromShader(Shader.Find("Projection/Decal/Packed/Specular/DeferredOpaque")); }
        }
        public override Material PackedDeferredTransparent
        {
            get { return MaterialFromShader(Shader.Find("Projection/Decal/Packed/Specular/DeferredTransparent")); }
        }

        //Supported Rendering
        public override RenderingPaths SupportedRendering
        {
            get { return RenderingPaths.Both; }
        }

        //Instanced Count
        public override int InstanceLimit
        {
            get { return 500; }
        }

        //Apply
        protected override void Apply(Material Material)
        {
            //Apply base
            base.Apply(Material);

            //Apply metallic
            albedo.Apply(Material);
            specular.Apply(Material);
            normal.Apply(Material);
            emissive.Apply(Material);
        }

        //Static Properties
        /**
        * The primary color details of your projection.
        * The alpha channel of these properties is used to determine the projections transparency.
        */
        public AlbedoPropertyGroup albedo;
        /**
        * The specular texture, with a color multiplier.
        * Determines how the light bouncing off the surface of the decal appears.
        * The color defines the color of the light that reflects of your surface.
        * The alpha defines how glossy your surface appears.
        */
        public SpecularPropertyGroup specular;
        /**
        * The normal texture of your decal, multiplied by the normal strength. 
        * Normals determine how the surface of your decal interacts with lights.
        */
        public NormalPropertyGroup normal;
        /**
        * The emission texture of your projection, multiplied by the emission color and intensity.
        * Emission allows us to make a decal appear as if it's emitting light. Supports HDR.
        */
        public EmissivePropertyGroup emissive;

        protected override void OnEnable()
        {
            //Initialize our property groups
            if (albedo == null) albedo = new AlbedoPropertyGroup(this);  
            if (specular == null) specular = new SpecularPropertyGroup(this);
            if (normal == null) normal = new NormalPropertyGroup(this);
            if (emissive == null) emissive = new EmissivePropertyGroup(this);

            base.OnEnable();
        }
        protected override void GenerateIDs()
        {
            base.GenerateIDs();

            albedo.GenerateIDs();
            specular.GenerateIDs();
            normal.GenerateIDs();
            emissive.GenerateIDs();
        }

        //Instanced Properties
        public override void UpdateProperties()
        {
            //Initialize property array
            if (properties == null || properties.Length != 2) properties = new ProjectionProperty[2];

            //Albedo Color
            properties[0] = new ProjectionProperty("Albedo", albedo._Color, albedo.Color);

            //Emission Color
            properties[1] = new ProjectionProperty("Emission", emissive._EmissionColor, emissive.Color, emissive.Intensity);
        }

        //Materials
        protected Material[] forwardMaterials;
        protected Material[] deferredOpaqueMaterials;
        protected Material[] deferredTransparentMaterials;
    }
}
