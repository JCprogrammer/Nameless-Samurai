Shader "SpookyShaders/DepthBlur" {
Properties {
blurSizeX("BlurSizeX", Float) = 0
blurSizeY("BlurSizeY", Float) = 0
_MainTex ("Texture", 2D) = "white" { }
 
}
SubShader {
Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
float blurSizeX;
float blurSizeY;
sampler2D _MainTex;
struct v2f {
float4  pos : SV_POSITION;
float2  uv : TEXCOORD0;
float2 depth : TEXCOORD1;
};
float4 _MainTex_ST;
v2f vert (appdata_base v)
{
v2f o;
o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
UNITY_TRANSFER_DEPTH(o.depth);
o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
return o;
}
half4 frag (v2f i) : COLOR
{
half4 sum = half4(0.0);
float depth = 1-i.depth.r*0.0005;
// gather pixel color from neighbours, distance to look for depends on camera distance..
sum += tex2D(_MainTex, float2(i.uv.x - 5.0 * depth, i.uv.y - 5.0 * depth)) * 0.025;
sum += tex2D(_MainTex, float2(i.uv.x - 4.0 * depth, i.uv.y - 4.0 * depth)) * 0.05;
sum += tex2D(_MainTex, float2(i.uv.x - 3.0 * depth, i.uv.y - 3.0 * depth)) * 0.09;
sum += tex2D(_MainTex, float2(i.uv.x - 2.0 * depth, i.uv.y - 2.0 * depth)) * 0.12;
sum += tex2D(_MainTex, float2(i.uv.x - 1.0 * depth, i.uv.y - 1.0 * depth)) * 0.15;
sum += tex2D(_MainTex, float2(i.uv.x, i.uv.y)) * 0.16;
sum += tex2D(_MainTex, float2(i.uv.x + 1.0 * depth, i.uv.y + 1.0 * depth)) * 0.15;
sum += tex2D(_MainTex, float2(i.uv.x + 2.0 * depth, i.uv.y + 2.0 * depth)) * 0.12;
sum += tex2D(_MainTex, float2(i.uv.x + 3.0 * depth, i.uv.y + 3.0 * depth)) * 0.09;
sum += tex2D(_MainTex, float2(i.uv.x + 4.0 * depth, i.uv.y + 4.0 * depth)) * 0.05;
sum += tex2D(_MainTex, float2(i.uv.x + 5.0 * depth, i.uv.y + 5.0 * depth)) * 0.025;
return sum;
}
ENDCG
}
}
Fallback "VertexLit"
}