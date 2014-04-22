Shader "Custom/TerrainSplat" {
Properties {
	// set by terrain engine
	_Control ("Control (RGBA)", 2D) = "red" {}
	_Splat3 ("Layer 3 (A)", 2D) = "white" {}
	_Splat2 ("Layer 2 (B)", 2D) = "white" {}
	_Splat1 ("Layer 1 (G)", 2D) = "white" {}
	_Splat0 ("Layer 0 (R)", 2D) = "white" {}
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
//#pragma surface surf Lambert
#pragma target 3.0

#pragma surface surf SimpleLambert

half4 LightingSimpleLambert (SurfaceOutput s, half3 lightDir, half atten) {
          half NdotL = dot (s.Normal, lightDir);
          half4 c;
          c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten * 2);
          c.a = s.Alpha;
          return c;
      }


//void vert (inout appdata_full v)
//{
//	v.tangent.xyz = cross(v.normal, float3(0,0,1));
//	v.tangent.w = -1;
//}

struct Input {
	float2 uv_Control : TEXCOORD0;
	float2 uv_Splat0 : TEXCOORD1;
	float2 uv_Splat1 : TEXCOORD2;
	float2 uv_Splat2 : TEXCOORD3;
	float2 uv_Splat3 : TEXCOORD4;
};

sampler2D _Control;
sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
sampler2D _Normal0,_Normal1,_Normal2,_Normal3;
//half _Shininess;

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 splat_control = tex2D (_Control, IN.uv_Control);
	fixed4 col;
	col  = splat_control.r * tex2D (_Splat0, IN.uv_Splat0);
	col += splat_control.g * tex2D (_Splat1, IN.uv_Splat1);
	//col += splat_control.b * tex2D (_Splat2, IN.uv_Splat2);
	//col += splat_control.a * tex2D (_Splat3, IN.uv_Splat3);
	o.Albedo = col.rgb;

	//fixed4 nrm;
	//nrm  = splat_control.r * tex2D (_Normal0, IN.uv_Splat0);
	//nrm += splat_control.g * tex2D (_Normal1, IN.uv_Splat1);
	//nrm += splat_control.b * tex2D (_Normal2, IN.uv_Splat2);
	//nrm += splat_control.a * tex2D (_Normal3, IN.uv_Splat3);
	// Sum of our four splat weights might not sum up to 1, in
	// case of more than 4 total splat maps. Need to lerp towards
	// "flat normal" in that case.
	//fixed splatSum = dot(splat_control, fixed4(1,1,1,1));
	//fixed4 flatNormal = fixed4(0.5,0.5,1,0.5); // this is "flat normal" in both DXT5nm and xyz*2-1 cases
	//nrm = lerp(flatNormal, nrm, splatSum);
	//o.Normal = UnpackNormal(nrm);

	//o.Gloss = col.a * splatSum;
	//o.Specular = _Shininess;
	//o.Specular = -1.0;
	//o.Gloss = 0.0;
	//o.Alpha = 1.0;
}
ENDCG  
}

//Dependency "AddPassShader" = "Hidden/Nature/Terrain/Bumped Specular AddPass"
//Dependency "BaseMapShader" = "Specular"

Fallback "Diffuse"
}
