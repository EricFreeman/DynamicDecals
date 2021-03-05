Shader "Projection/Internal/DepthNormalMask"
{
	Properties
	{
		_MainTex("", 2D) = "white" {}
		_Cutoff("", Float) = 0.5
		_Color("", Color) = (1,1,1,1)
	}

	SubShader{
		Tags{ "RenderType" = "Opaque" }
		Pass{
			CGPROGRAM
			#pragma vertex vertDepthNormal
			#pragma fragment fragDepthNormal
			#include "Replacement.cginc"
			
			void fragDepthNormal(inputDepthNormal i, out float4 outMask : SV_Target0, out float4 outNormal : SV_Target1, out float4 outDepth : SV_Target2)
			{
				//Encode Normal (0 - 1)
				float3 normal = i.nz.xyz * 0.5 + 0.5;
				
				//Output
				outNormal = float4(normal, 0);
				outMask = _MaskWrite;
				outDepth = float4(i.nz.w, 0, 0, 0);
			}
			ENDCG
		}
	}
	
	SubShader{
		Tags{ "RenderType" = "TransparentCutout" }
		Pass{
			CGPROGRAM
			#pragma vertex vertDepthNormal
			#pragma fragment fragDepthNormal
			#include "Replacement.cginc"

			void fragDepthNormal(inputDepthNormal i, out float4 outMask : SV_Target0, out float4 outNormal : SV_Target1, out float4 outDepth : SV_Target2)
			{
				//Clip
				fixed4 texcol = tex2D(_MainTex, i.uv);
				clip(texcol.a*_Color.a - _Cutoff);
				
				//Encode Normal (0 - 1)
				float3 normal = i.nz.xyz * 0.5 + 0.5;

				//Output
				outNormal = float4(normal, 0);
				outMask = _MaskWrite;
				outDepth = float4(i.nz.w, 0, 0, 0);
			}
			ENDCG
		}
	}
	
	SubShader{
		Tags{ "RenderType" = "TreeBark" }
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragDepthNormal
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityBuiltin3xTreeLibrary.cginc"
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 nz : TEXCOORD1;
			};
			v2f vert(appdata_full v) {
				v2f o;
				TreeVertBark(v);

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy;
				o.nz.xyz = COMPUTE_VIEW_NORMAL;
				o.nz.w = COMPUTE_DEPTH_01;
				return o;
			}

			uniform float4 _MaskWrite;

			void fragDepthNormal(v2f i, out float4 outMask : SV_Target0, out float4 outNormal : SV_Target1, out float4 outDepth : SV_Target2)
			{
				//Encode Normal (0 - 1)
				float3 normal = i.nz.xyz * 0.5 + 0.5;

				//Output
				outNormal = float4(normal, 0);
				outMask = _MaskWrite;
				outDepth = float4(i.nz.w, 0, 0, 0);
			}
			ENDCG
		}
	}
	
	SubShader{
		Tags{ "RenderType" = "TreeLeaf" }
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragDepthNormal
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityBuiltin3xTreeLibrary.cginc"
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 nz : TEXCOORD1;
			};
			v2f vert(appdata_full v) {
				v2f o;
				TreeVertLeaf(v);

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy;
				o.nz.xyz = COMPUTE_VIEW_NORMAL;
				o.nz.w = COMPUTE_DEPTH_01;
				return o;
			}
			
			uniform float4 _MaskWrite;
			uniform sampler2D _MainTex;
			uniform fixed _Cutoff;

			void fragDepthNormal(v2f i, out float4 outMask : SV_Target0, out float4 outNormal : SV_Target1, out float4 outDepth : SV_Target2)
			{
				//Clip
				half alpha = tex2D(_MainTex, i.uv).a;
				clip(alpha - _Cutoff);

				//Encode Normal (0 - 1)
				float3 normal = i.nz.xyz * 0.5 + 0.5;

				//Output
				outNormal = float4(normal, 0);
				outMask = _MaskWrite;
				outDepth = float4(i.nz.w, 0, 0, 0);
			}
			ENDCG
		}
	}
	
	SubShader{
		Tags{ "RenderType" = "TreeOpaque" "DisableBatching" = "True" }
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragDepthNormal
			#include "UnityCG.cginc"
			#include "TerrainEngine.cginc"
			struct v2f {
				float4 pos : SV_POSITION;
				float4 nz : TEXCOORD0;
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
				o.nz.xyz = COMPUTE_VIEW_NORMAL;
				o.nz.w = COMPUTE_DEPTH_01;
				return o;
			}

			uniform float4 _MaskWrite;

			void fragDepthNormal(v2f i, out float4 outMask : SV_Target0, out float4 outNormal : SV_Target1, out float4 outDepth : SV_Target2)
			{
				//Encode Normal (0 - 1)
				float3 normal = i.nz.xyz * 0.5 + 0.5;

				//Output
				outNormal = float4(normal, 0);
				outMask = _MaskWrite;
				outDepth = float4(i.nz.w, 0, 0, 0);
			}
			ENDCG
		}
	}
	
	SubShader{
		Tags{ "RenderType" = "TreeTransparentCutout" "DisableBatching" = "True" }
		Pass{
			Cull Back
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragDepthNormal
			#include "UnityCG.cginc"
			#include "TerrainEngine.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 nz : TEXCOORD1;
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
				o.nz.xyz = COMPUTE_VIEW_NORMAL;
				o.nz.w = COMPUTE_DEPTH_01;
				return o;
			}
			
			uniform float4 _MaskWrite;
			uniform sampler2D _MainTex;
			uniform fixed _Cutoff;

			void fragDepthNormal(v2f i, out float4 outMask : SV_Target0, out float4 outNormal : SV_Target1, out float4 outDepth : SV_Target2)
			{
				//Clip
				half alpha = tex2D(_MainTex, i.uv).a;
				clip(alpha - _Cutoff);

				//Encode Normal (0 - 1)
				float3 normal = i.nz.xyz * 0.5 + 0.5;

				//Output
				outNormal = float4(normal, 0);
				outMask = _MaskWrite;
				outDepth = float4(i.nz.w, 0, 0, 0);
			}
			ENDCG
		}
		Pass{
			Cull Front
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragDepthNormal
			#include "UnityCG.cginc"
			#include "TerrainEngine.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 nz : TEXCOORD1;
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
				o.nz.xyz = -COMPUTE_VIEW_NORMAL;
				o.nz.w = COMPUTE_DEPTH_01;
				return o;
			}
			
			uniform float4 _MaskWrite;
			uniform sampler2D _MainTex;
			uniform fixed _Cutoff;
			
			void fragDepthNormal(v2f i, out float4 outMask : SV_Target0, out float4 outNormal : SV_Target1, out float4 outDepth : SV_Target2)
			{
				//Clip
				half alpha = tex2D(_MainTex, i.uv).a;
				clip(alpha - _Cutoff);

				//Encode Normal (0 - 1)
				float3 normal = i.nz.xyz * 0.5 + 0.5;

				//Output
				outNormal = float4(normal, 0);
				outMask = _MaskWrite;
				outDepth = float4(i.nz.w, 0, 0, 0);
			}
			ENDCG
		}
	}
	
	SubShader{
		Tags{ "RenderType" = "TreeBillboard" }
		Pass{
			Cull Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragDepthNormal
			#include "UnityCG.cginc"
			#include "TerrainEngine.cginc"
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 nz : TEXCOORD1;
			};
			v2f vert(appdata_tree_billboard v) {
				v2f o;
				TerrainBillboardTree(v.vertex, v.texcoord1.xy, v.texcoord.y);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.x = v.texcoord.x;
				o.uv.y = v.texcoord.y > 0;
				o.nz.xyz = float3(0,0,1);
				o.nz.w = COMPUTE_DEPTH_01;
				return o;
			}
			
			uniform float4 _MaskWrite;
			uniform sampler2D _MainTex;

			void fragDepthNormal(v2f i, out float4 outMask : SV_Target0, out float4 outNormal : SV_Target1, out float4 outDepth : SV_Target2)
			{
				//Clip
				fixed4 texcol = tex2D(_MainTex, i.uv);
				clip(texcol.a - 0.001);

				//Encode Normal (0 - 1)
				float3 normal = i.nz.xyz * 0.5 + 0.5;

				//Output
				outNormal = float4(normal, 0);
				outMask = _MaskWrite;
				outDepth = float4(i.nz.w, 0, 0, 0);
			}
			ENDCG
		}
	}
	
	SubShader{
		Tags{ "RenderType" = "GrassBillboard" }
		Pass{
			Cull Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragDepthNormal
			#include "UnityCG.cginc"
			#include "TerrainEngine.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
				float2 uv : TEXCOORD0;
				float4 nz : TEXCOORD1;
			};

			v2f vert(appdata_full v) {
				v2f o;
				WavingGrassBillboardVert(v);
				o.color = v.color;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy;
				o.nz.xyz = COMPUTE_VIEW_NORMAL;
				o.nz.w = COMPUTE_DEPTH_01;
				return o;
			}

			uniform float4 _MaskWrite;
			uniform sampler2D _MainTex;
			uniform fixed _Cutoff;
			
			void fragDepthNormal(v2f i, out float4 outMask : SV_Target0, out float4 outNormal : SV_Target1, out float4 outDepth : SV_Target2)
			{
				//Clip
				fixed4 texcol = tex2D(_MainTex, i.uv);
				fixed alpha = texcol.a * i.color.a;
				clip(alpha - _Cutoff);

				//Encode Normal (0 - 1)
				float3 normal = i.nz.xyz * 0.5 + 0.5;

				//Output
				outNormal = float4(normal, 0);
				outMask = _MaskWrite;
				outDepth = float4(i.nz.w, 0, 0, 0);
			}
			ENDCG
		}
	}
	
	SubShader{
		Tags{ "RenderType" = "Grass" }
		Pass{
			Cull Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragDepthNormal
			#include "UnityCG.cginc"
			#include "TerrainEngine.cginc"
			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
				float2 uv : TEXCOORD0;
				float4 nz : TEXCOORD1;
			};

			v2f vert(appdata_full v) {
				v2f o;
				WavingGrassVert(v);
				o.color = v.color;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				o.nz.xyz = COMPUTE_VIEW_NORMAL;
				o.nz.w = COMPUTE_DEPTH_01;
				return o;
			}

			uniform float4 _MaskWrite;
			uniform sampler2D _MainTex;
			uniform fixed _Cutoff;

			void fragDepthNormal(v2f i, out float4 outMask : SV_Target0, out float4 outNormal : SV_Target1, out float4 outDepth : SV_Target2)
			{
				//Clip
				fixed4 texcol = tex2D(_MainTex, i.uv);
				fixed alpha = texcol.a * i.color.a;
				clip(alpha - _Cutoff);

				//Encode Normal (0 - 1)
				float3 normal = i.nz.xyz * 0.5 + 0.5;

				//Output
				outNormal = float4(normal, 0);
				outMask = _MaskWrite;
				outDepth = float4(i.nz.w, 0, 0, 0);
			}
			ENDCG
		}
	}

Fallback Off
}