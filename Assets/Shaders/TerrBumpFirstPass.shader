Shader "Custom/TerrainSplat" {
Properties {
	// set by terrain engine	
	_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
	_Position ("Control Position", Vector) = (0,0,0,0) //WX are the offset coords. YZ is the scale (Should be the same)
	_Control ("Control (RGBA)", 2D) = "red" {}
	_Splat3 ("Layer 3 (A)", 2D) = "white" {}
	_Splat2 ("Layer 2 (B)", 2D) = "white" {}
	_Splat1 ("Layer 1 (G)", 2D) = "white" {}
	_Splat0 ("Layer 0 (R)", 2D) = "white" {}

	_RandomLighting("Randomise Light", Range(0.0,0.5)) = 0.1
	
	// used in fallback on old cards & base map
	_MainTex ("BaseMap (RGB)", 2D) = "white" {}
	_Color ("Main Color", Color) = (1,1,1,1)
}

	
SubShader {
	Tags {
		"SplatCount" = "4"
		"Queue" = "Geometry-100"
		"RenderType" = "Opaque"
	}
CGPROGRAM
#pragma surface surf Lambert vertex:vert

#pragma target 3.0


struct Input {
	float2 uv_Control : TEXCOORD0;
	float2 uv_Splat0 : TEXCOORD1;
	float2 uv_Splat1 : TEXCOORD2;
	float2 uv_Splat2 : TEXCOORD3;
	float2 uv_Splat3 : TEXCOORD4;

	float lightFactor;
};

//For gradient dithering
float rand(float i)
{
	return fract(sin(i * 12.9898) * 43758.5453);
}

float _RandomLighting;

void vert (inout appdata_full v, out Input o)
{  
	float light = rand(v.normal.x);
	o.lightFactor = 1.0 + _RandomLighting - (light * _RandomLighting * 2.0);
} 

float2 controlUV;
fixed4 _Position;
sampler2D _Control;
sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
sampler2D _Normal0,_Normal1,_Normal2,_Normal3;
half _Shininess;

void surf (Input IN, inout SurfaceOutput o) {
	controlUV = IN.uv_Control * _Position.z; //Scale the UV (Z is scale)
	controlUV = controlUV + _Position.wx; //WX is offset
	fixed4 splat_control = tex2D (_Control, controlUV); //Use this new UV
	fixed4 col;
	col  = splat_control.r * tex2D (_Splat0, IN.uv_Splat0);
	col += splat_control.g * tex2D (_Splat1, IN.uv_Splat1);
	col += splat_control.b * tex2D (_Splat2, IN.uv_Splat2);
	//col += splat_control.a * tex2D (_Splat3, IN.uv_Splat3);
	o.Albedo = col.rgb * IN.lightFactor;

	// Sum of our four splat weights might not sum up to 1, in
	// case of more than 4 total splat maps. Need to lerp towards
	// "flat normal" in that case.
	fixed splatSum = dot(splat_control, fixed4(1,1,1,1));

	o.Gloss = col.a * splatSum;
	o.Specular = _Shininess;
	o.Alpha = 0.0;
}
ENDCG  
}

//Dependency "AddPassShader" = "Hidden/Nature/Terrain/Bumped Specular AddPass"
Dependency "BaseMapShader" = "Specular"

Fallback "Diffuse"
}
