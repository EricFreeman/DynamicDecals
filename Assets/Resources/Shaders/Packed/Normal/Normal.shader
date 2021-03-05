Shader "Projection/Decal/Packed/Normal"
{
	Properties
	{
		_MainTex("Shape Map", 2D) = "white" {}
		_Multiplier("Multiplier", Range(0.0, 1.0)) = 0.0

		_BumpScale("Normal Strength", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}
		_BumpFlip("Invert Normals", Range(0.0, 1.0)) = 0.0

		_NormalCutoff("Normal Cutoff", Range(0.0, 1.0)) = 0.5

		_MaskBase("Mask Base", Range(0.0, 1.0)) = 0.0
		_MaskLayers("Layers", Color) = (0.5, 0.5, 0.5, 0.5)
	}

	//3.5
	SubShader
	{
		Tags{ "Queue" = "AlphaTest+1" "DisableBatching" = "True"  "IgnoreProjector" = "True" }
		ZWrite Off ZTest Always Cull Front

		
		Pass
		{
			Name "DEFERRED"
			Tags{ "LightMode" = "Deferred" }

			Blend 0 Zero One
			Blend 1 Zero One
			Blend 2 SrcAlpha OneMinusSrcAlpha, Zero One
			Blend 3 Zero One

			CGPROGRAM
			#pragma target 3.5
			#pragma multi_compile_instancing
			#pragma exclude_renderers nomrt
			#pragma glsl

			#pragma multi_compile ___ UNITY_HDR_ON
			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
			#pragma multi_compile ___ DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
			#pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
			
			#define _PackedDepthNormals
			#pragma multi_compile ____ _AlphaTest
			#pragma multi_compile _ _Omni

			#include "../../Cginc/DeferredPasses.cginc"

			#pragma vertex vertProjection
			#pragma fragment fragNormal
			ENDCG
		}
	}
	Fallback Off
}