Shader "Custom/Water" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 0)
	_WaveAmp ("Wave Height", Float) = 1.0 //Amplitude of the waves
	_WaveAmpTan ("Wave Height Tangent", Float) = 1.0 //Amplitude of surface waves
	_Wavelength ("Wavelength", Float) = 1.0 //Length of the waves
	_WaveSpeed ("Wave Speed", Float) = 1.0 //How fast they oscillate
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	_FacetScale("Facet", Range(0, 4.0)) = 2.0 //How much the faces of the water 'pop'
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
float _FacetScale;

struct Input {
	float2 uv_MainTex;
	float colmod;
};

void vert (inout appdata_full v, out Input o)
{       
    //float3 castToWorld = round(mul(_Object2World, v.vertex) );
    float3 pos = v.vertex;
    float heightChange = sin(((pos.x - pos.z)/_Wavelength) - _Time * _WaveSpeed);
    v.vertex.z+= heightChange * _WaveAmp;
  	v.vertex.x+= heightChange * _WaveAmpTan* fmod(v.vertex.x, 1.0);
  	v.vertex.y+= -heightChange * _WaveAmpTan * fmod(v.vertex.z, 1.0);
	//Adjust the vertex colour by how the slope
	//Makes faces 'pop out'
	float3 scaledNormal = v.normal;
	scaledNormal.xz = scaledNormal.xz * _FacetScale;
	o.colmod = (_FacetScale / 8.0) - length(scaledNormal.xz);
}

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Albedo.rgb = o.Albedo.rgb + IN.colmod;
	o.Gloss = 1.0;
	o.Alpha = tex.a * _Color.a;
	o.Specular = _Shininess;
}
ENDCG
}

Fallback "Transparent/VertexLit"
}
