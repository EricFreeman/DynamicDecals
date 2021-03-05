#include "UnityStandardInput.cginc"
#include "UnityCG.cginc"

//Global mask value
uniform float4 _MaskWrite;

//Vertex to fragment struct
struct inputDepthNormal
{
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
    float4 nz : TEXCOORD1;
};

//Vertex to fragment program
inputDepthNormal vertDepthNormal(appdata_full v)
{
    inputDepthNormal o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
    o.nz.xyz = COMPUTE_VIEW_NORMAL;
    o.nz.w = COMPUTE_DEPTH_01;
    return o;
};