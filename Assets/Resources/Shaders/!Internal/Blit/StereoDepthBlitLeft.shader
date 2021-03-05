Shader "Projection/Internal/StereoDepthBlitLeft"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_DepthTex("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off ZWrite On ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			sampler2D_float _DepthTex;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			float4 frag(v2f i, out float outDepth : SV_Depth) : SV_Target
			{
				clip(0.5 - i.uv.x); // Skip right half

				float2 uv = i.uv;
				uv.x *= 2;

				float depth = SAMPLE_DEPTH_TEXTURE(_DepthTex, uv);
				outDepth = depth;
				
				return 0;
			}
			ENDCG
		}
	}
}
