Shader "SpookyShaders/Blurr"
{
    Properties
	{
		_blurSizeXY ("BlurSizeXY", Range(0, 4)) = 0
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" { }
	}

    SubShader 
	{
        Tags
		{
			"Queue" = "Transparent"
		}

		Pass
		{
			Tags
			{
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
			}
			LOD 100
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha 

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
			
				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					half2 texcoord : TEXCOORD0;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
			
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					return o;
				}
			
				fixed4 frag (v2f i) : COLOR
				{
					fixed4 col = tex2D(_MainTex, i.texcoord);
					return col;
				}
			ENDCG
		}

        GrabPass { }

        Pass
		{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag 
				#pragma target 3.0
				sampler2D _GrabTexture : register(s0);
				float _blurSizeXY;

				struct data
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
				};

				struct v2f
				{
					float4 position : POSITION;
					float4 screenPos : TEXCOORD0;
				};

				v2f vert (data i)
				{
					v2f o;

					o.position = mul(UNITY_MATRIX_MVP, i.vertex);
					o.screenPos = o.position;

					return o;
				}

				half4 frag (v2f i) : COLOR
				{
					float2 screenPos = i.screenPos.xy / i.screenPos.w;
					float depth = _blurSizeXY * 0.0005;

					screenPos.x = (screenPos.x + 1) * 0.5;
					screenPos.y = 1 - (screenPos.y + 1) * 0.5;

					half4 sum = half4(0.0h, 0.0h, 0.0h, 0.0h);

					sum += tex2D(_GrabTexture, float2(screenPos.x-5.0 * depth, screenPos.y+5.0 * depth)) * 0.025;    
					sum += tex2D(_GrabTexture, float2(screenPos.x+5.0 * depth, screenPos.y-5.0 * depth)) * 0.025;
    
					sum += tex2D(_GrabTexture, float2(screenPos.x-4.0 * depth, screenPos.y+4.0 * depth)) * 0.05;
					sum += tex2D(_GrabTexture, float2(screenPos.x+4.0 * depth, screenPos.y-4.0 * depth)) * 0.05;

					sum += tex2D(_GrabTexture, float2(screenPos.x-3.0 * depth, screenPos.y+3.0 * depth)) * 0.09;
					sum += tex2D(_GrabTexture, float2(screenPos.x+3.0 * depth, screenPos.y-3.0 * depth)) * 0.09;
    
					sum += tex2D(_GrabTexture, float2(screenPos.x-2.0 * depth, screenPos.y+2.0 * depth)) * 0.12;
					sum += tex2D(_GrabTexture, float2(screenPos.x+2.0 * depth, screenPos.y-2.0 * depth)) * 0.12;
    
					sum += tex2D(_GrabTexture, float2(screenPos.x-1.0 * depth, screenPos.y+1.0 * depth)) *  0.15;
					sum += tex2D(_GrabTexture, float2(screenPos.x+1.0 * depth, screenPos.y-1.0 * depth)) *  0.15;
    
	

					sum += tex2D(_GrabTexture, screenPos-5.0 * depth) * 0.025;    
					sum += tex2D(_GrabTexture, screenPos-4.0 * depth) * 0.05;
					sum += tex2D(_GrabTexture, screenPos-3.0 * depth) * 0.09;
					sum += tex2D(_GrabTexture, screenPos-2.0 * depth) * 0.12;
					sum += tex2D(_GrabTexture, screenPos-1.0 * depth) * 0.15;    
					sum += tex2D(_GrabTexture, screenPos) * 0.16; 
					sum += tex2D(_GrabTexture, screenPos+5.0 * depth) * 0.15;
					sum += tex2D(_GrabTexture, screenPos+4.0 * depth) * 0.12;
					sum += tex2D(_GrabTexture, screenPos+3.0 * depth) * 0.09;
					sum += tex2D(_GrabTexture, screenPos+2.0 * depth) * 0.05;
					sum += tex2D(_GrabTexture, screenPos+1.0 * depth) * 0.025;
       
					return sum / 2;
				}
			ENDCG
		}
    }

	Fallback Off
}