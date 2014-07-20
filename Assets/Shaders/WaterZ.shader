Shader "Custom/WaterZ" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 0)
	_WaveAmp ("Wave Height", Float) = 1.0 //Amplitude of the waves
	_WaveAmpTan ("Wave Height Tangent", Float) = 1.0 //Amplitude of surface waves
	_Wavelength ("Wavelength", Float) = 1.0 //Length of the waves
	_WaveSpeed ("Wave Speed", Float) = 1.0 //How fast they oscillate
	_WaveCount ("Wave Count", Int) = 1 //How many waves there are
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	_RandomLighting("Facet", Range(0, 1.0)) = 0.3 //How much the faces of the water 'pop'
	_MainTex ("Base (RGB) TransGloss (A)", 2D) = "white" {}
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 300

CGPROGRAM
#pragma surface surf BlinnPhong alpha vertex:vert

sampler2D _MainTex;
fixed4 _Color;
half _Shininess;
float _WaveAmp;
float _Wavelength;
float _WaveSpeed;
float _WaveAmpTan;
float _RandomLighting;

struct Input {
	float2 uv_MainTex;
	float lightFactor;
};

float rand(float i)
{
	return fract(sin(i * 12.9898) * 43758.5453);
}

void vert (inout appdata_full v, out Input o)
{       
    //float3 castToWorld = round(mul(_Object2World, v.vertex) );
    float3 pos = v.vertex;
    float heightChange = sin(((pos.x - pos.y)/_Wavelength) - _Time * _WaveSpeed);
	//Compute the tangential waves
  	v.vertex.x+= heightChange * _WaveAmpTan* fmod(v.vertex.x, 1.0);
  	v.vertex.y+= -heightChange * _WaveAmpTan * fmod(v.vertex.y, 1.0);

	//Compute vertical wave
	float newWavelength = _Wavelength;
	float newWaveAmp = _WaveAmp;
	float scaleChange = 4.0;
	float scaleChangeAmp = 1.4;
	for (int i = 1; i <= 4; i++)
    {
		heightChange = sin(((pos.x - pos.y)/(newWavelength)) - (_Time - newWaveAmp) * _WaveSpeed);
		v.vertex.z+= heightChange * newWaveAmp;

		newWavelength = newWavelength / scaleChange;
		newWaveAmp = newWaveAmp / scaleChangeAmp;
    }

	//Adjust the vertex colour by how the slope
	//Makes faces 'pop out'
	float light = rand(v.normal.x);
	o.lightFactor = 1.0 + _RandomLighting - (light * _RandomLighting * 2.0);

}

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Albedo.rgb = o.Albedo.rgb * IN.lightFactor;
	o.Gloss = 1.0;
	o.Alpha = tex.a * _Color.a;
	o.Specular = _Shininess;
}
ENDCG
}

Fallback "Transparent/VertexLit"
}
