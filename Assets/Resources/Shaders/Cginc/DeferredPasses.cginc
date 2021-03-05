//Include deferred projections
#include "DeferredProjections.cginc"

void fragWet(ProjectionInput i, out half4 outAlbedo : SV_Target, out half4 outSmoothSpec : SV_Target1, out half4 outNormal : SV_Target2, out half4 outEmission : SV_Target3)
{
	//Setup Instance Data
	UNITY_SETUP_INSTANCE_ID(i);

	//Generate projection
	Projection projection = CalculateProjection(i.screenPos, i.ray, i.worldForward);

	//Specsmooth output
	float w = _Glossiness * tex2D(_GlossMap, projection.localUV).r;
	outSmoothSpec = half4(0, 0, 0, w);

	//Other outputs
	outAlbedo = half4(0, 0, 0, 0);
	outNormal = half4(0, 0, 0, 0);
	outEmission = half4(0, 0, 0, 0);
}

void fragDry(ProjectionInput i, out half4 outAlbedo : SV_Target, out half4 outSmoothSpec : SV_Target1, out half4 outNormal : SV_Target2, out half4 outEmission : SV_Target3)
{
	//Setup Instance Data
	UNITY_SETUP_INSTANCE_ID(i);

	//Generate projection
	Projection projection = CalculateProjection(i.screenPos, i.ray, i.worldForward);

	//Specsmooth output
	float w = 1 - (_Glossiness * tex2D(_GlossMap, projection.localUV).r);
	outSmoothSpec = half4(100, 100, 100, w);

	//Other outputs
	outAlbedo = half4(100, 100, 100, 100);
	outNormal = half4(100, 100, 100, 100);
	outEmission = half4(100, 100, 100, 100);
}

void fragNormal(ProjectionInput i, out half4 outAlbedo : SV_Target, out half4 outSmoothSpec : SV_Target1, out half4 outNormal : SV_Target2, out half4 outEmission : SV_Target3)
{
	//Setup Instance Data
	UNITY_SETUP_INSTANCE_ID(i);

	//Generate projection
	Projection projection = CalculateProjection(i.screenPos, i.ray, i.worldForward);

	//Calculate normals
	float3x3 surface2WorldTranspose = Surface2WorldTranspose(i.worldUp, projection.normal);
	half3 normalWorld = WorldNormal(projection.localUV, surface2WorldTranspose, 1);

	//Calculate occlusion
	half occlusion = ShapeOcclusion(projection.localUV);

	//Normal output
	half4 n = half4(normalWorld, occlusion);
	outNormal = NormalOutput(n);

	//Other outputs
	outAlbedo = half4(0, 0, 0, 0);
	outSmoothSpec = half4(0, 0, 0, 0);
	outEmission = half4(0, 0, 0, 0);
}

void fragMetallicOpaque(ProjectionInput i, out half4 outAlbedo : SV_Target, out half4 outSmoothSpec : SV_Target1, out half4 outNormal : SV_Target2, out half4 outEmission : SV_Target3)
{
	//Setup Instance Data
	UNITY_SETUP_INSTANCE_ID(i);

	//Generate projection
	Projection projection = CalculateProjection(i.screenPos, i.ray, i.worldForward);

	//Generate base data
	FragmentCommonData fragment = FragmentMetallic(projection, i.worldUp, i.eyeVec);

	//Calculate ambient & reflections
	half3 a = Ambient(fragment);

	//Emission
	a += EmissionAlpha(projection.localUV);

	//Albedo output
	half3 c = fragment.diffColor;
	outAlbedo = AlbedoOutput(c, fragment.occlusion);
	//Specsmooth output
	half4 s = half4(fragment.specColor, fragment.oneMinusRoughness);
	outSmoothSpec = SpecSmoothOutput(s, fragment.occlusion);
	//Normal output
	half4 n = half4(fragment.normalWorld, 1.0);
	outNormal = NormalOutput(n);
	//Emission output
	outEmission = EmissionOutput(half4(a, 1.0), fragment.occlusion);
}

void fragMetallicTransparent(ProjectionInput i, out half4 outAlbedo : SV_Target, out half4 outSmoothSpec : SV_Target1, out half4 outNormal : SV_Target2, out half4 outEmission : SV_Target3)
{
	//Setup Instance Data
	UNITY_SETUP_INSTANCE_ID(i);

	//Generate projection
	Projection projection = CalculateProjection(i.screenPos, i.ray, i.worldForward);

	//Generate base data
	FragmentCommonData fragment = FragmentMetallic(projection, i.worldUp, i.eyeVec);

	//Calculate ambient & reflections
	half3 a = Ambient(fragment);

	//Emission
	a += EmissionAlpha(projection.localUV);

	//Albedo output
	half3 c = fragment.diffColor;
	outAlbedo = AlbedoOutput(c, fragment.occlusion);
	//Specsmooth output
	half4 s = half4(fragment.specColor, fragment.oneMinusRoughness);
	outSmoothSpec = SpecSmoothOutputPassOne(s, fragment.occlusion);
	//Normal output
	half4 n = half4(fragment.normalWorld, 1.0);
	outNormal = NormalOutput(n);
	//Emission output
	outEmission = EmissionOutput(half4(a, 1.0), fragment.occlusion);
}
void fragMetallicTransparentAdd(ProjectionInput i, out half4 outAlbedo : SV_Target, out half4 outSmoothSpec : SV_Target1, out half4 outNormal : SV_Target2, out half4 outEmission : SV_Target3)
{
	//Setup Instance Data
	UNITY_SETUP_INSTANCE_ID(i);

	//Generate projection
	Projection projection = CalculateProjection(i.screenPos, i.ray, i.worldForward);

	//Generate base data
	FragmentCommonData fragment = FragmentMetallic(projection, i.worldUp, i.eyeVec);

	//Calculate ambient & reflections
	half3 a = Ambient(fragment);

	//Specsmooth output
	half4 s = half4(fragment.specColor, fragment.oneMinusRoughness);

	outAlbedo = half4(0, 0, 0, 0);
	outSmoothSpec = SpecSmoothOutputPassTwo(s, fragment.occlusion);
	outNormal = half4(0, 0, 0, 0);
	outEmission = half4(0, 0, 0, 0);
}

void fragSpecularOpaque(ProjectionInput i, out half4 outAlbedo : SV_Target, out half4 outSmoothSpec : SV_Target1, out half4 outNormal : SV_Target2, out half4 outEmission : SV_Target3)
{
	//Setup Instance Data
	UNITY_SETUP_INSTANCE_ID(i);

	//Generate projection
	Projection projection = CalculateProjection(i.screenPos, i.ray, i.worldForward);

	//Generate base data
	FragmentCommonData fragment = FragmentSpecular(projection, i.worldUp, i.eyeVec);

	//Calculate ambient & reflections
	half3 a = Ambient(fragment);

	//Emission
	a += EmissionAlpha(projection.localUV);

	//Albedo output
	half3 c = fragment.diffColor;
	outAlbedo = AlbedoOutput(c, fragment.occlusion);
	//Specsmooth output
	half4 s = half4(fragment.specColor, fragment.oneMinusRoughness);
	outSmoothSpec = SpecSmoothOutput(s, fragment.occlusion);
	//Normal output
	half4 n = half4(fragment.normalWorld, 1.0);
	outNormal = NormalOutput(n);
	//Emission output
	outEmission = EmissionOutput(half4(a, 1.0), fragment.occlusion);
}
void fragSpecularTransparent(ProjectionInput i, out half4 outAlbedo : SV_Target, out half4 outSmoothSpec : SV_Target1, out half4 outNormal : SV_Target2, out half4 outEmission : SV_Target3)
{
	//Setup Instance Data
	UNITY_SETUP_INSTANCE_ID(i);

	//Generate projection
	Projection projection = CalculateProjection(i.screenPos, i.ray, i.worldForward);

	//Generate base data
	FragmentCommonData fragment = FragmentSpecular(projection, i.worldUp, i.eyeVec);

	//Calculate ambient & reflections
	half3 a = Ambient(fragment);

	//Emission
	a += EmissionAlpha(projection.localUV);

	//Albedo output
	half3 c = fragment.diffColor;
	outAlbedo = AlbedoOutput(c, fragment.occlusion);
	//Specsmooth output
	half4 s = half4(fragment.specColor, fragment.oneMinusRoughness);
	outSmoothSpec = SpecSmoothOutputPassOne(s, fragment.occlusion);
	//Normal output
	half4 n = half4(fragment.normalWorld, 1.0);
	outNormal = NormalOutput(n);
	//Emission output
	outEmission = EmissionOutput(half4(a, 1.0), fragment.occlusion);
}
void fragSpecularTransparentAdd(ProjectionInput i, out half4 outAlbedo : SV_Target, out half4 outSmoothSpec : SV_Target1, out half4 outNormal : SV_Target2, out half4 outEmission : SV_Target3)
{
	//Setup Instance Data
	UNITY_SETUP_INSTANCE_ID(i);

	//Generate projection
	Projection projection = CalculateProjection(i.screenPos, i.ray, i.worldForward);

	//Generate base data
	FragmentCommonData fragment = FragmentSpecular(projection, i.worldUp, i.eyeVec);

	//Calculate ambient & reflections
	half3 a = Ambient(fragment);

	//Specsmooth output
	half4 s = half4(fragment.specColor, fragment.oneMinusRoughness);

	outAlbedo = half4(0, 0, 0, 0);
	outSmoothSpec = SpecSmoothOutputPassTwo(s, fragment.occlusion);
	outNormal = half4(0, 0, 0, 0);
	outEmission = half4(0, 0, 0, 0);
}