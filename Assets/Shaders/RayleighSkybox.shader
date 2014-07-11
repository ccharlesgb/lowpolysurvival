Shader "RenderFX/SkyboxDynamic" {
Properties {
	_Tint ("Tint Color", Color) = (.5, .5, .5, .5)
	_WhiteTex ("BLANK", 2D) = "white" {}
	//_FrontTex ("Front (+Z)", 2D) = "white" {}
	//_BackTex ("Back (-Z)", 2D) = "white" {}
	//_LeftTex ("Left (+X)", 2D) = "white" {}
	//_RightTex ("Right (-X)", 2D) = "white" {}
	//_UpTex ("Up (+Y)", 2D) = "white" {}
	//_DownTex ("down (-Y)", 2D) = "white" {}
	_TopColor ("TopColor", Color) = (1.0, 1.0, 1.0, 1.0)
	_BottomColor ("BottomColor", Color) = (.1, .1, .1, .1)

	_GradOffset("Offset", float) = 0.0
	_GradFalloff("Fallof Speed", float) = 0.0
}

SubShader {
	Tags { "Queue"="Background" "RenderType"="Background" }
	Cull Off ZWrite Off Fog { Mode Off }
	
	CGINCLUDE
	#include "UnityCG.cginc"

	fixed4 _Tint;

	float _GradOffset;
	fixed4 _TopColor;
	fixed4 _BottomColor;

	struct appdata_t {
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
	};
	struct v2f {
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
	};
	v2f vert (appdata_t v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.texcoord = v.texcoord;
		return o;
	}
	fixed4 skybox_frag (v2f i, sampler2D smp, float gradOn)
	{
		fixed4 tex = tex2D (smp, i.texcoord);
		fixed4 col;

		//col.rgb = tex.rgb + _Tint.rgb - unity_ColorSpaceGrey;
		//col.a = tex.a * _Tint.a;

		float height = clamp(1.0-i.texcoord.y - _GradOffset, 0.0, 1.0) * gradOn; 
		col.rgb = (_BottomColor.rgb * height) + (_TopColor.rgb * (1.0-height));
		col.a = tex.a * _Tint.a;
		return col;
	}
	ENDCG
	
	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		sampler2D _WhiteTex;
		fixed4 frag (v2f i) : COLOR { return skybox_frag(i,_WhiteTex, 1.0); }
		ENDCG 
	}
	Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		sampler2D _WhiteTex;
		fixed4 frag (v2f i) : COLOR { return skybox_frag(i,_WhiteTex, 1.0); }
		ENDCG 
	}
	Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		sampler2D _WhiteTex;
		fixed4 frag (v2f i) : COLOR { return skybox_frag(i,_WhiteTex, 1.0); }
		ENDCG
	}
	Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		sampler2D _WhiteTex;
		fixed4 frag (v2f i) : COLOR { return skybox_frag(i,_WhiteTex, 1.0); }
		ENDCG
	}
	//TOP SIDE
	Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		sampler2D _WhiteTex;
		fixed4 frag (v2f i) : COLOR { return skybox_frag(i,_WhiteTex, 0.0); }
		ENDCG
	}
	//BOTTOM SIDE
	Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		sampler2D _WhiteTex;
		fixed4 frag (v2f i) : COLOR { return skybox_frag(i,_WhiteTex, 1.0); }
		ENDCG
	}
}
}