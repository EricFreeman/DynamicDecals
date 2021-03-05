float PackNormalMask(float3 Normal, float4 Mask)
{
	//Encode normal (0 - 1)
	float2 normal = EncodeViewNormalStereo(Normal);
	
	//Pack into float
	uint pkd;
	pkd = ((uint)(normal.x * 1023.0 + 0.5)); //Bits 0 to 9 (MMMMMMMMMM)
	pkd |= ((uint)(normal.y * 1023.0 + 0.5)) << 10; //Bits 10 to 19 (MMMMMMMMMM)
	pkd |= ((uint)(Mask.x + 0.5)) << 20; //Bit 20 (M)
	pkd |= ((uint)(Mask.y + 0.5)) << 21; //Bit 21 (M)
	pkd |= ((uint)(Mask.z + 0.5)) << 22; //Bit 22 (M)
	pkd |= ((uint)(Mask.w + 1.5)) << 23; //Bit 23 (E), To prevent an exponent that is 0 we add 1.0

	return (float)pkd;
}