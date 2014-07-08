Shader "Custom/Water" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 0)
	_WaveAmp ("Wave Height", Float) = 1.0 //Amplitude of the waves
	_WaveAmpTan ("Wave Height Tangent", Float) = 1.0 //Amplitude of surface waves
	_Wavelength ("Wavelength", Float) = 1.0 //Length of the waves
	_WaveSpeed ("Wave Speed", Float) = 1.0 //How fast they oscillate
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
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

struct Input {
	float2 uv_MainTex;
};

void vert (inout appdata_full v)
{       
    //float3 castToWorld = round(mul(_Object2World, v.vertex) );
    float3 pos = v.vertex;
    float heightChange = sin(((pos.x - pos.z)/_Wavelength) - _Time * _WaveSpeed);
    v.vertex.y+= heightChange * _WaveAmp;
  	v.vertex.x+= heightChange * _WaveAmpTan* fmod(v.vertex.x, 1.0);
  	v.vertex.z+= -heightChange * _WaveAmpTan * fmod(v.vertex.z, 1.0);
}

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Gloss = tex.a;
	o.Alpha = tex.a * _Color.a;
	o.Specular = _Shininess;
}
ENDCG
}

Fallback "Transparent/VertexLit"
}
