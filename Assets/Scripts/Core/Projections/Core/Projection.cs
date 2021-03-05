using UnityEngine;
using UnityEngine.Rendering;

using System.Collections;

namespace LlockhamIndustries.Decals
{
    /**
    * The abstract projection class. All projections inherit from this singular class. 
    * If you want to make your own projection types, inherit from this class. The system's UI should automatically detect and allow you to create projections of your custom type.
    */
    [System.Serializable]
    public abstract class Projection : ScriptableObject
    {
        //Material in use
        public Material Mat
        {
            get
            {
                if (SupportedRendering == RenderingPaths.Forward || DynamicDecals.System.SystemPath == SystemPath.Forward || ForceForward)
                {
                    return Forward;
                }
                else
                {
                    switch (TransparencyType)
                    {
                        case TransparencyType.Cutout:
                            return DeferredOpaque;

                        case TransparencyType.Blend:
                            return DeferredTransparent;
                    }
                }
                return null;
            }
        }

        //Material types
        private Material Forward
        {
            get
            {
                switch (DynamicDecals.System.Settings.Replacement)
                {
                    case ShaderReplacementType.Mobile: return MobileForward;
                    case ShaderReplacementType.VR: return PackedForward;
                }
                return StandardForward;
            }
        }
        private Material DeferredOpaque
        {
            get
            {
                switch (DynamicDecals.System.Settings.Replacement)
                {
                    case ShaderReplacementType.Mobile: return MobileDeferredOpaque;
                    case ShaderReplacementType.VR: return PackedDeferredOpaque;
                }
                return StandardDeferredOpaque;
            }
        }
        private Material DeferredTransparent
        {
            get
            {
                switch (DynamicDecals.System.Settings.Replacement)
                {
                    case ShaderReplacementType.Mobile: return MobileDeferredTransparent;
                    case ShaderReplacementType.VR: return PackedDeferredTransparent;
                }
                return StandardDeferredTransparent;
            }
        }

        //Mobile
        public virtual Material MobileForward
        {
            get { return null; }
        }
        public virtual Material MobileDeferredOpaque
        {
            get { return null; }
        }
        public virtual Material MobileDeferredTransparent
        {
            get { return null; }
        }

        //Standard
        public virtual Material StandardForward
        {
            get { return null; }
        }
        public virtual Material StandardDeferredOpaque
        {
            get { return null; }
        }
        public virtual Material StandardDeferredTransparent
        {
            get { return null; }
        }

        //Packed
        public virtual Material PackedForward
        {
            get { return null; }
        }
        public virtual Material PackedDeferredOpaque
        {
            get { return null; }
        }
        public virtual Material PackedDeferredTransparent
        {
            get { return null; }
        }

        //Material Constructor
        protected Material MaterialFromShader(Shader p_Shader)
        {
            //Create material
            if (material == null)
            {
                material = new Material(p_Shader);
                material.hideFlags = HideFlags.DontSave;
                Apply(material);
            }

            //Set relevant shader
            if (material.shader != p_Shader) material.shader = p_Shader;

            return material;
        }

        //Rendering settings
        public abstract RenderingPaths SupportedRendering
        {
            get;
        }
        public bool Valid
        {
            get
            {
                if (SupportedRendering == RenderingPaths.Deferred && DynamicDecals.System.SystemPath == SystemPath.Forward) return false;
                return true;
            }
        }

        protected void DestroyMaterial()
        {
            //Grab material
            Material mat = Mat;

            //Destroy
            if (mat != null)
            {
                if (Application.isPlaying) Destroy(mat);
                else DestroyImmediate(mat, true);
            }
        }
        protected void UpdateMaterial()
        {
            //Grab material
            Material mat = Mat;

            //Update
            if (mat != null) Apply(mat);
        }
        protected virtual void Apply(Material Material)
        {
            //Type
            switch (type)
            {
                case ProjectionType.Decal:
                    Material.DisableKeyword("_Omni");
                    break;
                case ProjectionType.OmniDecal:
                    Material.EnableKeyword("_Omni");
                    break;
            }

            //Instancing
            #if !UNITY_5_5
            Material.enableInstancing = Instanced;
            #endif

            //Transparency
            switch (transparencyType)
            {
                case TransparencyType.Blend:
                    Material.DisableKeyword("_AlphaTest");
                    break;
                case TransparencyType.Cutout:
                    Material.EnableKeyword("_AlphaTest");
                    break;
            }
            Material.SetFloat(_Cutoff, cutoff);

            //Tiling/Offset
            Vector4 tilingOffset = new Vector4(tiling.x, tiling.y, offset.x, offset.y);
            Material.SetVector(_TilingOffset, tilingOffset);

            //Masking
            if (masks.Length == 4)
            {
                switch (maskMethod)
                {
                    case MaskMethod.DrawOnEverythingExcept:
                        Material.SetFloat(_MaskBase, 1);

                        Color except = Color.clear;
                        except.r = (masks[0]) ? 0 : 0.5f;
                        except.g = (masks[1]) ? 0 : 0.5f;
                        except.b = (masks[2]) ? 0 : 0.5f;
                        except.a = (masks[3]) ? 0 : 0.5f;

                        Material.SetVector(_MaskLayers, except);
                        break;

                    case MaskMethod.OnlyDrawOn:
                        Material.SetFloat(_MaskBase, 0);

                        Color only = Color.clear;
                        only.r = (masks[0]) ? 1 : 0.5f;
                        only.g = (masks[1]) ? 1 : 0.5f;
                        only.b = (masks[2]) ? 1 : 0.5f;
                        only.a = (masks[3]) ? 1 : 0.5f;

                        Material.SetVector(_MaskLayers, only);
                        break;
                }
            }

            //Projection Limit
            float limit = Mathf.Cos(Mathf.Deg2Rad * projectionLimit);
            Material.SetFloat(_NormalCutoff, limit);
        }

        //Static Properties
        /**
        * Defines how the projection is projected.
        * Decals project from a plane in a single direction. This allows complex detail to be projected accurately.
        * OmniDecals project from a point in all directions. This samples a gradient based on how far from the point a surface being projected on is.
        */
        public ProjectionType ProjectionType
        {
            get { return type; }
            set
            {
                if (type != value)
                {
                    type = value;
                    Mark();
                }
            }
        }

        /**
        * Defines whether this projection appears above or below other projections. Higher priority projections will appear above lower priority projections. ie. a priority 10 projection will appear to overlap a priority 5 projection. 
        * Values should be positive and less than 100.
        */
        public int Priority
        {
            get { return priority; }
            set
            {
                if (priority != value)
                {
                    priority = value;
                    Mark();
                }
            }
        }

        /**
        * Defines the transparency method. 
        * Cutout will simply cull any pixels under a certain alpha value and is the cheaper method.
        * Blend will blend the projection with the surface it's drawing on based on the alpha value.
        */
        public TransparencyType TransparencyType
        {
            get { return transparencyType; }
            set
            {
                if (transparencyType != value)
                {
                    transparencyType = value;
                    Mark();
                }
            }
        }
        /**
        * The alpha cutoff of the projection.
        * Any pixels with an alpha value below this value will not be rendered.
        */
        public float Cutoff
        {
            get { return cutoff; }
            set
            {
                if (cutoff != value)
                {
                    cutoff = value;
                    Mark();
                }
            }
        }

        /**
        * The tiling of all textures applied to your decal.
        * Higher values will cause your texture to repeat while lower values will cause it to stretch.
        */
        public Vector2 Tiling
        {
            get { return tiling; }
            set
            {
                if (tiling != value)
                {
                    tiling = value;
                    Mark();
                }
            }
        }
        /**
        * The offset of all textures applied to your decal.
        * Allows you to adjust the position of your texture, useful for scrolling effects, atlassing or to tweak tiling textures.
        */
        public Vector2 Offset
        {
            get { return offset; }
            set
            {
                if (offset != value)
                {
                    offset = value;
                    Mark();
                }
            }
        }

        /**
        * Defines which masking method we should apply to this projection. Either "DrawOnEverythingExcept" or "OnlyDrawOn".
        * Draw On Everything Except - will draw on all surface except those in the selected mask layers.
        * Only Draw On - will only draw on surfaces that are part  of the selected mask layers.
        */
        public MaskMethod MaskMethod
        {
            get { return maskMethod; }
            set
            {
                if (maskMethod != value)
                {
                    maskMethod = value;
                    Mark();
                }
            }
        }
        /**
        * Defines whether this projection is affected by the first masking layer. 
        * To add surfaces to this mask layer add a Mask component to a renderable gameObject and toggle on the appropriate mask layer.
        */
        public bool MaskLayer1
        {
            get { return masks[0]; }
            set
            {
                if (masks[0] != value)
                {
                    masks[0] = value;
                    Mark();
                }
            }
        }
        /**
        * Defines whether this projection is affected by the second masking layer.
        * To add surfaces to this mask layer add a Mask component to a renderable gameObject and toggle on the appropriate mask layer.
        */
        public bool MaskLayer2
        {
            get { return masks[1]; }
            set
            {
                if (masks[1] != value)
                {
                    masks[1] = value;
                    Mark();
                }
            }
        }
        /**
        * Defines whether this projection is affected by the third masking layer.
        * To add surfaces to this mask layer add a Mask component to a renderable gameObject and toggle on the appropriate mask layer.
        */
        public bool MaskLayer3
        {
            get { return masks[2]; }
            set
            {
                if (masks[2] != value)
                {
                    masks[2] = value;
                    Mark();
                }
            }
        }
        /**
        * Defines whether this projection is affected by the fourth masking layer.
        * To add surfaces to this mask layer add a Mask component to a renderable gameObject and toggle on the appropriate mask layer.
        */
        public bool MaskLayer4
        {
            get { return masks[3]; }
            set
            {
                if (masks[3] != value)
                {
                    masks[3] = value;
                    Mark();
                }
            }
        }

        /**
        * The normal cutoff angle of the decal.
        * If the angle between the surface and the inverse direction of projection is beyond this limit, the pixel will not be rendered. 
        * This is designed to prevent your decals from stretching when they project onto near parralel surfaces, or surfaces in which they would appear streched.
        * Setting this to 180 will render all pixels.
        */
        public float ProjectionLimit
        {
            get { return projectionLimit; }
            set
            {
                if (projectionLimit != value)
                {
                    projectionLimit = Mathf.Clamp(value, 0, 180);
                    Mark();
                }
            }
        }

        /**
        * Defines whether this projection should be instanced.
        * Disable this if you need your projections to be drawn specifically in the order they where created, instead of instanced together and drawn in a pseudo random order.
        * Whether instanced or not, the priority of the projection will be respected, as this is set projection wide.
        */
        public bool Instanced
        {
            get { return (DynamicDecals.System.Instanced && instanced); }
            set { instanced = value; }
        }
        public abstract int InstanceLimit
        {
            get;
        }

        /**
        * Defines whether this projection should be forced to render in a forward renderloop. This only affects those using deferred rendering.
        * This is useful when you need to draw decals on objects that are rendered in the forward rendering loop, usually objects with shaders that don't support deferred rendering.
        */
        public bool ForceForward
        {
            get { return (forceForward); }
            set { forceForward = value; }
        }

        //Instanced Properties
        public ProjectionProperty[] Properties
        {
            get
            {
                UpdateProperties();
                return properties;
            }
        }
        public virtual void UpdateProperties()
        {

        }

        //Enable - Disable
        protected virtual void OnEnable()
        {
            GenerateIDs();
        }
        protected virtual void OnDisable()
        {
            DestroyMaterial();
        }

        //Primary Loop
        public void Update()
        {
            if (marked)
            {
                //Update properties
                UpdateProperties();

                //Update initialized materials
                UpdateMaterial();

                //No longer marked
                marked = false;
            }
        }

        public void Mark(bool UpdateImmediately = false)
        {
            marked = true;
            if (UpdateImmediately) Update();
        }
        private bool marked;

        #region Backing Fields
        //Material
        protected Material material;

        //Instanced Properties
        protected ProjectionProperty[] properties;

        //Static Properties
        [SerializeField]
        protected ProjectionType type;

        [SerializeField]
        protected bool instanced = false;

        [SerializeField]
        protected bool forceForward = false;

        [SerializeField]
        protected int priority;

        [SerializeField]
        protected TransparencyType transparencyType;
        [SerializeField]
        protected float cutoff = 0.2f;

        [SerializeField]
        protected Vector2 tiling;
        [SerializeField]
        protected Vector2 offset;

        [SerializeField]
        protected MaskMethod maskMethod;
        [SerializeField]
        protected bool[] masks = new bool[4];

        [SerializeField]
        protected float projectionLimit = 80;
        #endregion
        #region Property Ids
        public int _Cutoff;
        public int _TilingOffset;
        public int _MaskBase;
        public int _MaskLayers;
        protected int _NormalCutoff;

        protected virtual void GenerateIDs()
        {
            _Cutoff = Shader.PropertyToID("_Cutoff");
            _TilingOffset = Shader.PropertyToID("_TilingOffset");
            _MaskBase = Shader.PropertyToID("_MaskBase");
            _MaskLayers = Shader.PropertyToID("_MaskLayers");
            _NormalCutoff = Shader.PropertyToID("_NormalCutoff");
        }
        #endregion
    }

    public enum RenderingPaths { Both, Forward, Deferred }
    public enum ProjectionType { Decal, OmniDecal };
    public enum TransparencyType { Cutout, Blend };
    public enum MaskMethod { DrawOnEverythingExcept, OnlyDrawOn };
}