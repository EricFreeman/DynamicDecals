Shader "Projection/Internal/Mask" 
{
	Properties
	{
		//Clipping
		_MainTex("", 2D) = "white" {}
		_Cutoff("", Float) = 0.5
		_Color("", Color) = (1,1,1,1)
	}
	SubShader
	{
		ZWrite On ZTest LEqual Cull Off

		Tags{ "RenderType" = "Opaque" }
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vertMask
			#pragma fragment fragMask

			struct inputMask
			{
				float4 pos : SV_POSITION;
			};

			inputMask vertMask(appdata_full v)
			{
				inputMask o;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			};

			float4 _MaskWrite;

			half4 fragMask(inputMask i) : SV_Target
			{
				//Output
				return _MaskWrite;
			}
			ENDCG
		}
	}

	SubShader
	{
		ZWrite On ZTest LEqual Cull Off

		Tags{ "RenderType" = "TransparentCutout" }
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "UnityStandardInput.cginc"
			#pragma vertex vertMask
			#pragma fragment fragMask

			struct inputMask
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};


			float4 _MaskWrite;

			inputMask vertMask(appdata_full v)
			{
				inputMask o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			};

			half4 fragMask(inputMask i) : SV_Target
			{
				//Clip
				fixed4 texcol = tex2D(_MainTex, i.uv);
				clip(texcol.a*_Color.a - _Cutoff);
				
				//Output
				return _MaskWrite;
			}
			ENDCG
		}
	}
	
	SubShader
	{
		ZWrite On ZTest LEqual Cull Off

		Tags{ "RenderType" = "TreeBark" }
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityBuiltin3xTreeLibrary.cginc"
			
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata_full v) {
				v2f o;
				TreeVertBark(v);

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy;
				return o;
			}

			float4 _MaskWrite;

			half4 frag(v2f i) : SV_Target
			{
				//Output
				return _MaskWrite;
			}
			ENDCG
		}
	}
	
	SubShader
	{
		ZWrite On ZTest LEqual Cull Off

		Tags{ "RenderType" = "TreeLeaf" }
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityBuiltin3xTreeLibrary.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			
			v2f vert(appdata_full v) {
				v2f o;
				TreeVertLeaf(v);

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy;
				return o;
			}

			sampler2D _MainTex;
			float4 _MaskWrite;
			float _Cutoff;

			half4 frag(v2f i) : SV_Target
			{
				//Clip
				half alpha = tex2D(_MainTex, i.uv).a;
				clip(alpha - _Cutoff);

				//Output
				return _MaskWrite;
			}
			ENDCG
		}
	}
	
	SubShader
	{
		ZWrite On ZTest LEqual Cull Off

		Tags{ "RenderType" = "TreeOpaque" "DisableBatching" = "True" }
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "TerrainEngine.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
			};

			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				fixed4 color : COLOR;
			};

			v2f vert(appdata v) {
				v2f o;
				TerrainAnimateTree(v.vertex, v.color.w);
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}

			float4 _MaskWrite;

			half4 frag(v2f i) : SV_Target
			{
				//Output
				return _MaskWrite;
			}
			ENDCG
		}
	}
	
	SubShader
	{
		Tags{ "RenderType" = "TreeTransparentCutout" "DisableBatching" = "True" }
		Pass
		{
			ZWrite On ZTest LEqual Cull Back

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "TerrainEngine.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				fixed4 color : COLOR;
				float4 texcoord : TEXCOORD0;
			};

			v2f vert(appdata v) {
				v2f o;
				TerrainAnimateTree(v.vertex, v.color.w);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy;
				return o;
			}

			sampler2D _MainTex;
			float4 _MaskWrite;
			float4 _Color;
			float _Cutoff;

			half4 frag(v2f i) : SV_Target
			{
				//Clip
				half alpha = tex2D(_MainTex, i.uv).a;
				clip(alpha - _Cutoff);

				//Output
				return _MaskWrite;
			}
			ENDCG
		}

		Pass
		{
			ZWrite On ZTest LEqual Cull Front
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "TerrainEngine.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				fixed4 color : COLOR;
				float4 texcoord : TEXCOORD0;
			};

			v2f vert(appdata v) {
				v2f o;
				TerrainAnimateTree(v.vertex, v.color.w);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy;
				return o;
			}
			
			sampler2D _MainTex;
			float4 _MaskWrite;
			float4 _Color;
			float _Cutoff;

			half4 frag(v2f i) : SV_Target
			{
				//Clip
				half alpha = tex2D(_MainTex, i.uv).a;
				clip(alpha - _Cutoff);

				//Output
				return _MaskWrite;
			}
			ENDCG
		}
	}
	
	SubShader
	{
		Tags{ "RenderType" = "TreeBillboard" }
		Pass
		{
			ZWrite On ZTest LEqual Cull Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "TerrainEngine.cginc"
			
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata_tree_billboard v) {
				v2f o;
				TerrainBillboardTree(v.vertex, v.texcoord1.xy, v.texcoord.y);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.x = v.texcoord.x;
				o.uv.y = v.texcoord.y > 0;
				return o;
			}

			sampler2D _MainTex;
			float4 _MaskWrite;
			float4 _Color;
			float _Cutoff;

			half4 frag(v2f i) : SV_Target
			{
				//Clip
				fixed4 texcol = tex2D(_MainTex, i.uv);
				clip(texcol.a - 0.001);

				//Output
				return _MaskWrite;
			}
			ENDCG
		}
	}
	
	SubShader
	{
		Tags{ "RenderType" = "GrassBillboard" }
		Pass
		{
			ZWrite On ZTest LEqual Cull Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "TerrainEngine.cginc"
			
			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata_full v) {
				v2f o;
				WavingGrassBillboardVert(v);
				o.color = v.color;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy;
				return o;
			}
			
			sampler2D _MainTex;
			float4 _MaskWrite;
			float4 _Color;
			float _Cutoff;

			half4 frag(v2f i) : SV_Target
			{
				//Clip
				fixed4 texcol = tex2D(_MainTex, i.uv);
				fixed alpha = texcol.a * i.color.a;
				clip(alpha - _Cutoff);

				//Output
				return _MaskWrite;
			}
			ENDCG
		}
	}
	
	SubShader
	{
		Tags{ "RenderType" = "Grass" }
		Pass
		{
			ZWrite On ZTest LEqual Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "TerrainEngine.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata_full v) {
				v2f o;
				WavingGrassVert(v);
				o.color = v.color;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			sampler2D _MainTex;
			float4 _MaskWrite;
			float4 _Color;
			float _Cutoff;

			half4 frag(v2f i) : SV_Target
			{
				//Clip
				fixed4 texcol = tex2D(_MainTex, i.uv);
				fixed alpha = texcol.a * i.color.a;
				clip(alpha - _Cutoff);

				//Output
				return _MaskWrite;
			}
			ENDCG
		}
	}
	Fallback Off
}