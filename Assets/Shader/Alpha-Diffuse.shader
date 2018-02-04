Shader "myShader/Diffuse" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
  // 剔除 关闭 //
	Cull off
  // 顶点光照 关闭 //
	Lighting Off
	ZWrite off
  // 设置深度测试模式,如果是always，前面的物体无法遮住这个物体 //
	ZTest Always  
  // 设置alpha混合模式, 让该材质半透明，并能显示其他材质，到达融合效果。 //
	Blend One One
	LOD 200

CGPROGRAM
#pragma surface surf Lambert alpha:blend

sampler2D _MainTex;
fixed4 _Color;

struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	o.Albedo = c.rgb;
	o.Alpha = c.a;
}
ENDCG
}

Fallback "Legacy Shaders/Transparent/VertexLit"
}
