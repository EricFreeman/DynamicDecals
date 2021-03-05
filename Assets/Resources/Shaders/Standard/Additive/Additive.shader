Shader "Projection/Decal/Standard/Additive"
{
	Properties
	{
		_Color("Albedo", Color) = (1,1,1,1)
		_MainTex("Albedo Map", 2D) = "white" {}

		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
		_NormalCutoff("Normal Cutoff", Range(0.0, 1.0)) = 0.5
		
		_TilingOffset("Tiling / Offset", Vector) = (1, 1, 0, 0)

		_MaskBase("Mask Base", Range(0.0, 1.0)) = 0.0
		_MaskLayers("Layers", Color) = (0.5, 0.5, 0.5, 0.5)
	}

	//2.5
	SubShader
	{
		Tags{ "Queue" = "AlphaTest+1" "DisableBatching" = "True"  "IgnoreProjector" = "True" }
		ZWrite Off ZTest Always Cull Front

		Pass
		{
			Name "FORWARD"
			Tags{ "LightMode" = "ForwardBase" }

			Blend One One

			CGPROGRAM
			#pragma target 2.5
			#pragma multi_compile_instancing
			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
			#pragma glsl

			#pragma multi_compile _PrecisionDepthNormals _CustomDepthNormals
			#pragma multi_compile _ _AlphaTest
			#pragma multi_compile ___ _Omni

			#include "../../Cginc/ForwardPasses.cginc"

			#pragma vertex vertProjection
			#pragma fragment fragAdd
			ENDCG
		}
	}
	Fallback Off
}