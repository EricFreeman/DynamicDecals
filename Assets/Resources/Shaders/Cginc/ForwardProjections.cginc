#include "Projections.cginc"

//UnityLight
UnityLight MainLight(half3 normalWorld)
{
	UnityLight l;
#ifdef LIGHTMAP_OFF

	l.color = _LightColor0.rgb;
	l.dir = _WorldSpaceLightPos0.xyz;
	l.ndotl = LambertTerm(normalWorld, l.dir);
#else
	// no light specified by the engine
	// analytical light might be extracted from Lightmap data later on in the shader depending on the Lightmap type
	l.color = half3(0.f, 0.f, 0.f);
	l.ndotl = 0.f;
	l.dir = half3(0.f, 0.f, 0.f);
#endif

	return l;
}
UnityLight AdditiveLight(half3 normalWorld, half3 lightDir, half atten)
{
	UnityLight l;

	l.color = _LightColor0.rgb;
	l.dir = lightDir;
#ifndef USING_DIRECTIONAL_LIGHT
	l.dir = normalize(l.dir);
#endif
	l.ndotl = LambertTerm(normalWorld, l.dir);

	// shadow the light
	l.color *= atten;
	return l;
}
UnityIndirect ZeroIndirect()
{
	UnityIndirect ind;
	ind.diffuse = 0;
	ind.specular = 0;
	return ind;
}

//Lighting
inline fixed LightAttenuation(float3 posWorld, float2 screenPos)
{
	fixed atten = 1;

	//Correct LightCoords per pixel	
#ifdef POINT
	float3 LightCoord = mul(unity_WorldToLight, float4(posWorld, 1)).xyz;
	atten = (tex2D(_LightTexture0, dot(LightCoord, LightCoord).rr).UNITY_ATTEN_CHANNEL);
#endif
#ifdef SPOT
	float4 LightCoord = mul(unity_WorldToLight, float4(posWorld, 1));
	atten = ((LightCoord.z > 0) * UnitySpotCookie(LightCoord) * UnitySpotAttenuate(LightCoord.xyz));
#endif
#ifdef DIRECTIONAL
	atten = 1;
#endif
#ifdef POINT_COOKIE
	float3 LightCoord = mul(unity_WorldToLight, float4(posWorld, 1)).xyz;
	atten = (tex2D(_LightTextureB0, dot(LightCoord, LightCoord).rr).UNITY_ATTEN_CHANNEL * texCUBE(_LightTexture0, LightCoord).w);
#endif
#ifdef DIRECTIONAL_COOKIE
	float2 LightCoord = mul(unity_WorldToLight, float4(posWorld, 1)).xy;
	atten = (tex2D(_LightTexture0, LightCoord).w);
#endif

	//Correct ShadowCoords per pixel
#if defined (SHADOWS_SCREEN)
#if defined(UNITY_NO_SCREENSPACE_SHADOWS)
	float4 ShadowCoord = mul(unity_WorldToShadow[0], float4(posWorld, 1));
	atten *= unitySampleShadow(ShadowCoord);
#else
	atten *= tex2D(_ShadowMapTexture, screenPos).r;
#endif
#endif
#if defined (SHADOWS_DEPTH) && defined (SPOT)
	//Spot
	float4 ShadowCoord = mul(unity_WorldToShadow[0], float4(posWorld, 1));
	atten *= UnitySampleShadowmap(ShadowCoord);
#endif
#if defined (SHADOWS_CUBE)
	//Point
	float3 ShadowCoord = posWorld - _LightPositionRange.xyz;
	atten *= UnitySampleShadowmap(ShadowCoord);
#endif

	return atten;
}
inline fixed ShadowAttenuation(float3 posWorld, float2 screenPos)
{
	fixed atten = 1;

	//Correct ShadowCoords per pixel
#if defined (SHADOWS_SCREEN)
#if defined(UNITY_NO_SCREENSPACE_SHADOWS)
	float4 ShadowCoord = mul(unity_WorldToShadow[0], float4(posWorld, 1));
	atten *= unitySampleShadow(ShadowCoord);
#else
	atten *= tex2D(_ShadowMapTexture, screenPos).r;
#endif
#endif
#if defined (SHADOWS_DEPTH) && defined (SPOT)
	//Spot
	float4 ShadowCoord = mul(unity_WorldToShadow[0], float4(posWorld, 1));
	atten *= UnitySampleShadowmap(ShadowCoord);
#endif
#if defined (SHADOWS_CUBE)
	//Point
	float3 ShadowCoord = posWorld - _LightPositionRange.xyz;
	atten *= UnitySampleShadowmap(ShadowCoord);
#endif

	return atten;
}
inline float3 LightDiriction(float3 posWorld)
{
	//Calculate LightDirection
	return _WorldSpaceLightPos0.xyz - posWorld * _WorldSpaceLightPos0.w;
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

	UNITY_FOG_COORDS(5)
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

ProjectionInput vertProjection(VertexInput v)
{
	ProjectionInput o;

	UNITY_SETUP_INSTANCE_ID (v);
    UNITY_TRANSFER_INSTANCE_ID (v, o);

	o.pos = UnityObjectToClipPos (v.vertex);
	o.screenPos = ComputeScreenPos(o.pos);
	o.ray = UnityObjectToViewPos(v.vertex) * float3(-1, -1, 1);

	float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
	o.eyeVec = posWorld.xyz - _WorldSpaceCameraPos;

	o.worldForward = mul((float3x3)unity_ObjectToWorld, float3(0, 0, 1));
	o.worldUp = mul((float3x3)unity_ObjectToWorld, float3(1, 0, 0)); //(Now Right)

	UNITY_TRANSFER_FOG(o, o.pos);
	return o;
}

//Vertex program - OmniDecal
struct OmniDecalInput
{
	float4 pos : SV_POSITION;
	float4 screenPos : TEXCOORD0;
	float3 ray : TEXCOORD1;
	UNITY_FOG_COORDS(2)
	UNITY_VERTEX_INPUT_INSTANCE_ID
};
OmniDecalInput vertOmniDecal(VertexInput v)
{
	OmniDecalInput o;

	UNITY_SETUP_INSTANCE_ID (v);
    UNITY_TRANSFER_INSTANCE_ID (v, o);

	o.pos = UnityObjectToClipPos (v.vertex);
	o.screenPos = ComputeScreenPos(o.pos);
	o.ray = UnityObjectToViewPos(v.vertex) * float3(-1, -1, 1);

	UNITY_TRANSFER_FOG(o, o.pos);
	return o;
}

//Output
half4 Output(half3 color, half occlusion)
{
	#ifdef _AlphaTest
	return half4(color, 1);
	#else
	return half4(color, occlusion);
	#endif
}

//Metallic Programs
half4 fragForwardMetallic(ProjectionInput i) : SV_Target
{
	//Setup Instance Data
	UNITY_SETUP_INSTANCE_ID (i);

	//Generate projection
	Projection projection = CalculateProjection(i.screenPos, i.ray, i.worldForward);

	//Generate base data
	FragmentCommonData fragment = FragmentMetallic(projection, i.worldUp, i.eyeVec);

	//Setup Light
	UnityLight mainLight = MainLight(fragment.normalWorld);
	half atten = ShadowAttenuation(projection.posWorld, projection.screenPos);

	//Setup GI
	UnityGI gi = FragmentGI(fragment, 1, half4(0,0,0,0), atten, mainLight, true);

	//Calculate final output
	half4 c = UNITY_BRDF_PBS(fragment.diffColor, fragment.specColor, fragment.oneMinusReflectivity, fragment.oneMinusRoughness, fragment.normalWorld, -fragment.eyeVec, gi.light, gi.indirect);
	c.rgb += UNITY_BRDF_GI(fragment.diffColor, fragment.specColor, fragment.oneMinusReflectivity, fragment.oneMinusRoughness, fragment.normalWorld, -fragment.eyeVec, 1, gi);
	c.rgb += EmissionAlpha(projection.localUV);

	UNITY_APPLY_FOG(i.fogCoord, c.rgb);
	return Output(c.rgb, fragment.occlusion);
}
half4 fragForwardAddMetallic(ProjectionInput i) : SV_Target
{	
	//Generate projection
	Projection projection = CalculateProjection(i.screenPos, i.ray, i.worldForward);

	//Generate base data
	FragmentCommonData fragment = FragmentMetallic(projection, i.worldUp, i.eyeVec);

	//Calculate lighting data
	float atten = LightAttenuation(projection.posWorld, projection.screenPos);
	float3 lightDir = LightDiriction(projection.posWorld);

	//Setup Light
	UnityLight light = AdditiveLight(fragment.normalWorld, lightDir, atten);
	UnityIndirect noIndirect = ZeroIndirect();

	half4 c = UNITY_BRDF_PBS(fragment.diffColor, fragment.specColor, fragment.oneMinusReflectivity, fragment.oneMinusRoughness, fragment.normalWorld, -fragment.eyeVec, light, noIndirect);

	UNITY_APPLY_FOG_COLOR(i.fogCoord, c.rgb, half4(0.0, 0.0, 0.0, 0.0));
	return Output(c.rgb, fragment.occlusion);
}

//Specular Programs
half4 fragForwardSpecular(ProjectionInput i) : SV_Target
{
	//Setup Instance Data
	UNITY_SETUP_INSTANCE_ID (i);

	//Generate projection
	Projection projection = CalculateProjection(i.screenPos, i.ray, i.worldForward);

	//Generate base data
	FragmentCommonData fragment = FragmentSpecular(projection, i.worldUp, i.eyeVec);

	//Setup Light
	UnityLight mainLight = MainLight(fragment.normalWorld);
	half atten = ShadowAttenuation(projection.posWorld, projection.screenPos);

	//Setup GI
	UnityGI gi = FragmentGI(fragment, 1, half4(0,0,0,0), atten, mainLight, true);

	//Calculate final output
	half4 c = UNITY_BRDF_PBS(fragment.diffColor, fragment.specColor, fragment.oneMinusReflectivity, fragment.oneMinusRoughness, fragment.normalWorld, -fragment.eyeVec, gi.light, gi.indirect);
	c.rgb += UNITY_BRDF_GI(fragment.diffColor, fragment.specColor, fragment.oneMinusReflectivity, fragment.oneMinusRoughness, fragment.normalWorld, -fragment.eyeVec, 1, gi);
	c.rgb += EmissionAlpha(projection.localUV);

	UNITY_APPLY_FOG(i.fogCoord, c.rgb);
	return Output(c.rgb, fragment.occlusion);
}
half4 fragForwardAddSpecular(ProjectionInput i) : SV_Target
{
	//Generate projection
	Projection projection = CalculateProjection(i.screenPos, i.ray, i.worldForward);

	//Generate base data
	FragmentCommonData fragment = FragmentSpecular(projection, i.worldUp, i.eyeVec);

	//Calculate lighting data
	float atten = LightAttenuation(projection.posWorld, projection.screenPos);
	float3 lightDir = LightDiriction(projection.posWorld);

	//Setup Light
	UnityLight light = AdditiveLight(fragment.normalWorld, lightDir, atten);
	UnityIndirect noIndirect = ZeroIndirect();

	half4 c = UNITY_BRDF_PBS(fragment.diffColor, fragment.specColor, fragment.oneMinusReflectivity, fragment.oneMinusRoughness, fragment.normalWorld, -fragment.eyeVec, light, noIndirect);

	UNITY_APPLY_FOG_COLOR(i.fogCoord, c.rgb, half4(0, 0, 0, 0));
	return Output(c.rgb, fragment.occlusion);
}