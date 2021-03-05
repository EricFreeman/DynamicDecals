#include "ForwardProjections.cginc"

half4 fragAdd(ProjectionInput i) : SV_Target
{
	//Setup Instance Data
	UNITY_SETUP_INSTANCE_ID(i);

	//Generate projection
	Projection projection = CalculateProjection(i.screenPos, i.ray, i.worldForward);

	//Generate base data
	FragmentCommonData fragment = FragmentUnlit(projection, i.worldUp, i.eyeVec);

	//Grab color
	half3 c = fragment.diffColor;

	//Apply fog
	UNITY_APPLY_FOG(i.fogCoord, c);

	//Apply occlusion
	c *= fragment.occlusion;
	
	return Output(c, 1);
}

half4 fragMultiply(ProjectionInput i) : SV_Target
{
	//Setup Instance Data
	UNITY_SETUP_INSTANCE_ID(i);

	//Generate projection
	Projection projection = CalculateProjection(i.screenPos, i.ray, i.worldForward);

	//Generate base data
	FragmentCommonData fragment = FragmentUnlit(projection, i.worldUp, i.eyeVec);

	//Grab color
	half3 c = fragment.diffColor;

	//Apply fog
	UNITY_APPLY_FOG(i.fogCoord, c);

	//Apply occlusion
	c = lerp(half3(1,1,1) , c, fragment.occlusion);
	
	return Output(c, 1);
}

half4 fragUnlit(ProjectionInput i) : SV_Target
{
	//Setup Instance Data
	UNITY_SETUP_INSTANCE_ID(i);

	//Generate projection
	Projection projection = CalculateProjection(i.screenPos, i.ray, i.worldForward);

	//Generate base data
	FragmentCommonData fragment = FragmentUnlit(projection, i.worldUp, i.eyeVec);

	//Grab color
	half3 c = fragment.diffColor;

	//Apply fog
	UNITY_APPLY_FOG(i.fogCoord, c);
	return Output(c, fragment.occlusion);
}