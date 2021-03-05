Shader "Projection/Decal/Mobile/Metallic/DeferredTransparent"
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

	//3.0
	SubShader
	{
		Tags{ "Queue" = "AlphaTest+1" "DisableBatching" = "True"  "IgnoreProjector" = "True" }
		ZWrite Off ZTest Always Cull Front

		Pass
		{
			Name "DEFERRED"
			Tags{ "LightMode" = "Deferred" }

			Blend SrcAlpha OneMinusSrcAlpha, Zero One

			CGPROGRAM
			#pragma target 3.0
			#pragma multi_compile_instancing
			#pragma exclude_renderers nomrt
			#pragma glsl

			#pragma multi_compile ___ UNITY_HDR_ON
			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
			#pragma multi_compile ___ DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
			#pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
			
			#pragma multi_compile _ _Omni

			#include "../../../Cginc/DeferredPasses.cginc"

			#pragma vertex vertProjection
			#pragma fragment fragMetallicTransparent
			ENDCG
		}

		Pass
		{
			Name "DEFERRED ADD"
			Tags{ "LightMode" = "Deferred" }

			BlendOp Max

			CGPROGRAM
			#pragma target 3.0
			#pragma multi_compile_instancing
			#pragma exclude_renderers nomrt
			#pragma glsl

			#pragma multi_compile ___ UNITY_HDR_ON
			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
			#pragma multi_compile ___ DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
			#pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
			
			#pragma multi_compile _ _Omni

			#include "../../../Cginc/DeferredPasses.cginc"

			#pragma vertex vertProjection
			#pragma fragment fragMetallicTransparentAdd
			ENDCG
		}
	}
	Fallback Off
}