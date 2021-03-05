//Include projections core
#include "Projections.cginc"

//Gloss tex - wet / dry decals
sampler2D _GlossMap;

//UnityLight
UnityLight DummyLight(half3 normalWorld)
{
	UnityLight l;
	l.color = 0;
	l.dir = half3 (0, 1, 0);
	l.ndotl = LambertTerm(normalWorld, l.dir);
	return l;
}

//Vertex program - Projection
struct ProjectionInput
{
	float4 pos : SV_POSITION;
	float4 screenPos : TEXCOORD0;
	float3 ray : TEXCOORD1;

	half3 worldForward : TEXCOORD2;
	half3 worldUp : TEXCOORD3;

	half3 eyeVec : TEXCOORD4;

	UNITY_VERTEX_INPUT_INSTANCE_ID
};

ProjectionInput vertProjection(VertexInput v)
{
	ProjectionInput o;

	UNITY_SETUP_INSTANCE_ID (v);
    UNITY_TRANSFER_INSTANCE_ID (v, o);

	o.pos = UnityObjectToClipPos(float4(v.vertex.xyz, 1));
	o.screenPos = ComputeScreenPos(o.pos);
	o.ray = UnityObjectToViewPos(v.vertex) * float3(-1, -1, 1);

	float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
	o.eyeVec = posWorld.xyz - _WorldSpaceCameraPos;

	o.worldForward = mul((float3x3)unity_ObjectToWorld, float3(0, 0, 1));	//WorldSpace Forward
	o.worldUp = mul((float3x3)unity_ObjectToWorld, float3(1, 0, 0)); //WorldSpace Up (Now Right)
	return o;
}

//Vertex program - OmniDecal
struct OmniDecalInput
{
	float4 pos : SV_POSITION;
	float4 screenPos : TEXCOORD0;
	float3 ray : TEXCOORD1;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};
OmniDecalInput vertOmniDecal(VertexInput v)
{
	OmniDecalInput o;

	UNITY_SETUP_INSTANCE_ID (v);
    UNITY_TRANSFER_INSTANCE_ID (v, o);

	o.pos = UnityObjectToClipPos(float4(v.vertex.xyz, 1));
	o.screenPos = ComputeScreenPos(o.pos);
	o.ray = UnityObjectToViewPos(v.vertex) * float3(-1, -1, 1);
	return o;
}

//Ambient & reflections
inline half3 Ambient(FragmentCommonData fragment)
{
	//Check reflection
#if UNITY_ENABLE_REFLECTION_BUFFERS
	bool sampleReflectionsInDeferred = false;
#else
	bool sampleReflectionsInDeferred = true;
#endif

	//Create dummy light for global illumination
	UnityLight dummyLight = DummyLight(fragment.normalWorld);

	//Ambient, global illumination & reflections
	UnityGI gi = FragmentGI(fragment, 1, 0, 1, dummyLight, sampleReflectionsInDeferred);

	half3 Ambient = UNITY_BRDF_PBS(fragment.diffColor, fragment.specColor, fragment.oneMinusReflectivity, fragment.oneMinusRoughness, fragment.normalWorld, -fragment.eyeVec, gi.light, gi.indirect).rgb;
	Ambient += UNITY_BRDF_GI(fragment.diffColor, fragment.specColor, fragment.oneMinusReflectivity, fragment.oneMinusRoughness, fragment.normalWorld, -fragment.eyeVec, 1, gi);

	return Ambient;
}

//Output
half4 AlbedoOutput(half3 color, half occlusion)
{
	return half4(color, occlusion);
}

half4 SpecSmoothOutput(half4 specSmooth, half occlusion)
{
	return specSmooth;
}

half4 NormalOutput(half3 normal)
{
	return half4(normal * 0.5 + 0.5, 1);
}

half4 EmissionOutput(float4 emission, half occlusion)
{
	#ifndef UNITY_HDR_ON
	//Logrithmically scale lighting data in HDR
	emission.rgb = exp2(-emission.rgb);
	#endif

	return half4(emission.rgb, occlusion);
}

half4 SpecSmoothOutputPassOne(half4 specSmooth, half occlusion)
{
	return half4(specSmooth.rgb, occlusion);
}
half4 SpecSmoothOutputPassTwo(half4 specSmooth, half occlusion)
{
	return half4(0, 0, 0, specSmooth.a * occlusion);
}