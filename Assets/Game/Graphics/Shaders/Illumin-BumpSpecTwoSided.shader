Shader "Self-Illumin/Bumped Specular TwoSided" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_Illum ("Illumin (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_EmissionLM ("Emission (Lightmapper)", Float) = 0
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 400
	Cull Off
	
CGPROGRAM
#pragma surface surf BlinnPhong

sampler2D _MainTex;
sampler2D _BumpMap;
sampler2D _Illum;
fixed4 _Color;
half _Shininess;

struct Input {
	float2 uv_MainTex;
	float2 uv_Illum;
	float2 uv_BumpMap;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 c = tex * _Color;
	o.Albedo = c.rgb;
	o.Emission = c.rgb * tex2D(_Illum, IN.uv_Illum).a;
	o.Gloss = tex.a;
	o.Alpha = c.a;
	o.Specular = _Shininess;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
}
ENDCG
}
FallBack "Self-Illumin/Specular"
}
