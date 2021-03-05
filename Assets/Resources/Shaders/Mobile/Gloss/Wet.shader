Shader "Projection/Decal/Mobile/Wet"
{
	Properties
	{
		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		_GlossMap("Gloss Map", 2D) = "white" {}

		_NormalCutoff("Normal Cutoff", Range(0.0, 1.0)) = 0.5

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
			
			#pragma multi_compile ____ _AlphaTest
			#pragma multi_compile _ _Omni
			
			#include "../../Cginc/DeferredPasses.cginc"

			#pragma vertex vertProjection
			#pragma fragment fragWet
			ENDCG
		}
	}
	Fallback Off
}