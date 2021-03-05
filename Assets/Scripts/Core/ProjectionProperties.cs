using UnityEngine;
using System.Collections;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LlockhamIndustries.Decals
{
    [Serializable]
    public class ShapePropertyGroup
    {
        //Core
        protected Projection projection;

        //Runtime Methods
        protected void Mark()
        {
            projection.Mark();
        }

        //Constructor
        public ShapePropertyGroup(Projection Projection)
        {
            projection = Projection;
        }
        public void GenerateIDs()
        {
            //Grab Ids
            _MainTex = Shader.PropertyToID("_MainTex");
            _Multiplier = Shader.PropertyToID("_Multiplier");
        }
        
        //Apply
        public void Apply(Material Material)
        {
            Material.SetTexture(_MainTex, texture);
            Material.SetFloat(_Multiplier, multiplier);
        }

        //Properties
        public Texture Texture
        {
            get { return texture; }
            set
            {
                if (texture != value)
                {
                    texture = value;
                    Mark();
                }
            }
        }
        public float Multiplier
        {
            get { return multiplier; }
            set
            {
                if (multiplier != value)
                {
                    multiplier = value;
                    Mark();
                }
            }
        }

        //Backing Fields
        [SerializeField]
        private Texture texture;
        [SerializeField]
        private float multiplier = 1;

        //Property Ids
        public int _MainTex;
        public int _Multiplier;
    }
    [Serializable]
    public class AlbedoPropertyGroup
    {
        //Core
        protected Projection projection;

        //Runtime Methods
        protected void Mark()
        {
            projection.Mark();
        }

        //Constructor
        public AlbedoPropertyGroup(Projection Projection)
        {
            projection = Projection;
        }
        public void GenerateIDs()
        {
            //Grab Ids
            _MainTex = Shader.PropertyToID("_MainTex");
            _Color = Shader.PropertyToID("_Color");
        }
        
        //Apply
        public void Apply(Material Material)
        {
            Material.SetTexture(_MainTex, texture);
            Material.SetColor(_Color, color);
        }

        //Properties
        /**
        * The primary color texture of your projection. Multiplied with the albedo color. 
        * The alpha channel of this texture is used to determine the projections transparency.
        */
        public Texture Texture
        {
            get { return texture; }
            set
            {
                if (texture != value)
                {
                    texture = value;
                    Mark();
                }
            }
        }
        /**
        * The primary color of your projection. Multiplied with the albedo map. 
        * The alpha channel is used to determine the projections transparency.
        */
        public Color Color
        {
            get { return color; }
            set
            {
                if (color != value)
                {
                    color = value;
                    Mark();
                }
            }
        }

        //Backing Fields
        [SerializeField]
        private Texture texture;
        [SerializeField]
        private Color color = Color.grey;

        //Property Ids
        public int _MainTex;
        public int _Color;
    }
    [Serializable]
    public class GlossPropertyGroup
    {
        //Core
        protected Projection projection;

        //Runtime Methods
        protected void Mark()
        {
            projection.Mark();
        }

        //Constructor
        public GlossPropertyGroup(Projection Projection)
        {
            projection = Projection;
        }
        public void GenerateIDs()
        {
            //Grab Ids
            _GlossMap = Shader.PropertyToID("_GlossMap");
            _Glossiness = Shader.PropertyToID("_Glossiness");
        }

        //Apply
        public void Apply(Material Material)
        {
            Material.SetTexture(_GlossMap, texture);
            Material.SetFloat(_Glossiness, glossiness);
        }

        //Properties
        public Texture Texture
        {
            get { return texture; }
            set
            {
                if (texture != value)
                {
                    texture = value;
                    Mark();
                }
            }
        }
        public float Glossiness
        {
            get { return glossiness; }
            set
            {
                if (glossiness != value)
                {
                    glossiness = value;
                    Mark();
                }
            }
        }

        //Backing Fields
        [SerializeField]
        private Texture texture;
        [SerializeField]
        private float glossiness = 1;

        //Property Ids
        public int _GlossMap;
        public int _Glossiness;
    }
    [Serializable]
    public class MetallicPropertyGroup
    {
        //Core
        protected Projection projection;

        //Runtime Methods
        protected void Mark()
        {
            projection.Mark();
        }

        //Constructor
        public MetallicPropertyGroup(Projection Projection)
        {
            projection = Projection;
        }
        public void GenerateIDs()
        {
            //Grab Ids
            _MetallicGlossMap = Shader.PropertyToID("_MetallicGlossMap");
            _Metallic = Shader.PropertyToID("_Metallic");
            _Glossiness = Shader.PropertyToID("_Glossiness");
        }

        //Apply
        public void Apply(Material Material)
        {
            Material.SetTexture(_MetallicGlossMap, texture);
            Material.SetFloat(_Metallic, metallicity);
            Material.SetFloat(_Glossiness, glossiness);
        }

        //Properties
        public Texture Texture
        {
            get { return texture; }
            set
            {
                if (texture != value)
                {
                    texture = value;
                    Mark();
                }
            }
        }
        public float Metallicity
        {
            get { return metallicity; }
            set
            {
                if (metallicity != value)
                {
                    metallicity = value;
                    Mark();
                }
            }
        }
        public float Glossiness
        {
            get { return glossiness; }
            set
            {
                if (glossiness != value)
                {
                    glossiness = value;
                    Mark();
                }
            }
        }

        //Backing Fields
        [SerializeField]
        private Texture texture;
        [SerializeField]
        private float metallicity = 0.5f;
        [SerializeField]
        private float glossiness = 1;

        //Property Ids
        public int _MetallicGlossMap;
        public int _Metallic;
        public int _Glossiness;
    }
    [Serializable]
    public class SpecularPropertyGroup
    {
        //Core
        protected Projection projection;

        //Runtime Methods
        protected void Mark()
        {
            projection.Mark();
        }

        //Constructor
        public SpecularPropertyGroup(Projection Projection)
        {
            projection = Projection;
        }
        public void GenerateIDs()
        {
            //Grab Ids
            _SpecGlossMap = Shader.PropertyToID("_SpecGlossMap");
            _SpecColor = Shader.PropertyToID("_SpecColor");
            _Glossiness = Shader.PropertyToID("_Glossiness");
        }

        //Apply
        public void Apply(Material Material)
        {
            Material.SetTexture(_SpecGlossMap, texture);
            Material.SetColor(_SpecColor, color);
            Material.SetFloat(_Glossiness, glossiness);
        }

        //Properties
        public Texture Texture
        {
            get { return texture; }
            set
            {
                if (texture != value)
                {
                    texture = value;
                    Mark();
                }
            }
        }
        public Color Color
        {
            get { return color; }
            set
            {
                if (color != value)
                {
                    color = value;
                    Mark();
                }
            }
        }
        public float Glossiness
        {
            get { return glossiness; }
            set
            {
                if (glossiness != value)
                {
                    glossiness = value;
                    Mark();
                }
            }
        }

        //Backing Fields
        [SerializeField]
        private Texture texture;
        [SerializeField]
        private Color color = new Color(0.2f, 0.2f, 0.2f, 1);
        [SerializeField]
        private float glossiness = 1;

        //Property Ids
        public int _SpecGlossMap;
        public int _SpecColor;
        public int _Glossiness;
    }
    [Serializable]
    public class NormalPropertyGroup
    {
        //Core
        protected Projection projection;

        //Runtime Methods
        protected void Mark()
        {
            projection.Mark();
        }

        //Constructor
        public NormalPropertyGroup(Projection Projection)
        {
            projection = Projection;
        }
        public void GenerateIDs()
        {
            //Grab Ids
            _BumpMap = Shader.PropertyToID("_BumpMap");
            _BumpScale = Shader.PropertyToID("_BumpScale");
        }

        //Apply
        public void Apply(Material Material)
        {
            Material.SetTexture(_BumpMap, texture);
            Material.SetFloat(_BumpScale, strength);
        }

        //Properties
        public Texture Texture
        {
            get { return texture; }
            set
            {
                if (texture != value)
                {
                    texture = value;
                    Mark();
                }
            }
        }
        public float Strength
        {
            get { return strength; }
            set
            {
                if (strength != value)
                {
                    strength = value;
                    Mark();
                }
            }
        }

        //Backing Fields
        [SerializeField]
        private Texture texture;
        [SerializeField]
        private float strength = 1;

        //Property Ids
        public int _BumpMap;
        public int _BumpScale;
    }
    [Serializable]
    public class EmissivePropertyGroup
    {
        //Core
        protected Projection projection;

        //Runtime Methods
        protected void Mark()
        {
            projection.Mark();
        }

        //Constructor
        public EmissivePropertyGroup(Projection Projection)
        {
            projection = Projection;
        }
        public void GenerateIDs()
        {
            //Grab Ids
            _EmissionMap = Shader.PropertyToID("_EmissionMap");
            _EmissionColor = Shader.PropertyToID("_EmissionColor");
        }

        //Apply
        public void Apply(Material Material)
        {
            Material.SetTexture(_EmissionMap, texture);
            Material.SetColor(_EmissionColor, color * intensity);
        }

        //Properties
        public Texture Texture
        {
            get { return texture; }
            set
            {
                if (texture != value)
                {
                    texture = value;
                    Mark();
                }
            }
        }
        public float Intensity
        {
            get { return intensity; }
            set
            {
                if (intensity != value)
                {
                    intensity = value;
                    Mark();
                }
            }
        }
        public Color Color
        {
            get { return color; }
            set
            {
                if (color != value)
                {
                    color = value;
                    Mark();
                }
            }
        }

        //Backing Fields
        [SerializeField]
        private Texture texture;
        [SerializeField]
        private Color color = Color.black;
        [SerializeField]
        private float intensity = 1;

        //Property Ids
        public int _EmissionMap;
        public int _EmissionColor;
    }
}