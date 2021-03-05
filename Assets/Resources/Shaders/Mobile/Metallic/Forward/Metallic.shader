Shader "Projection/Decal/Mobile/Metallic/Forward"
{
	Properties
	{
		_Color("Albedo", Color) = (1,1,1,1)
		_MainTex("Albedo Map", 2D) = "white" {}

		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
		_NormalCutoff("Normal Cutoff", Range(0.0, 1.0)) = 0.5

		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		_MetallicGlossMap("Metallic Gloss Map", 2D) = "white" {}

		_BumpScale("Normal Strength", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}

		_EmissionColor("Emission", Color) = (0,0,0)
		_EmissionMap("Emission Map", 2D) = "white" {}
		
		_TilingOffset("Tiling / Offset", Vector) = (1, 1, 0, 0)

		_MaskBase("Mask Base", Range(0.0, 1.0)) = 0.0
		_MaskLayers("Layers", Color) = (0.5, 0.5, 0.5, 0.5)
	}

	//2.5
	SubShader
	{
		Tags{ "Queue" = "AlphaTest+1" "DisableBatching" = "True"  "IgnoreProjector" = "True" }
		ZWrite Off ZTest Always Cull Front

		//Forward Base
		Pass
		{
			Name "FORWARD"
			Tags{ "LightMode" = "ForwardBase" }

			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
			#pragma glsl

			#pragma multi_compile _ _AlphaTest
			#pragma multi_compile ___ _Omni

			#pragma vertex vertProjection
			#pragma fragment fragForwardMetallic

			#include "../../../Cginc/ForwardProjections.cginc"
			ENDCG
		}

		//Forward Add
		Pass
		{
			Name "FORWARD_ADD"
			Tags{ "LightMode" = "ForwardAdd" }

			Blend SrcAlpha One
			Fog{ Color(0,0,0,0) }

			CGPROGRAM
			#pragma multi_compile_fwdadd_fullshadows
			#pragma multi_compile_fog
			#pragma glsl

			#pragma multi_compile _ _AlphaTest
			#pragma multi_compile ___ _Omni

			#pragma vertex vertProjection
			#pragma fragment fragForwardAddMetallic

			#include "../../../Cginc/ForwardProjections.cginc"
			ENDCG
		}
	}
	Fallback Off
}