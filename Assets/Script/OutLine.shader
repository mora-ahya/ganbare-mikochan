Shader "Unlit/OutLine"
{
	Properties{
		_MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[Toggle]_Light("Light", Int) = 0
		_Thick("Thick", Int) = 0
	}
	SubShader{

		Tags {
			"Queue" = "Transparent"
		}

		Blend SrcAlpha OneMinusSrcAlpha

		ZWrite off
		Cull off//flip関連、超重要

		Pass {

		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct VertexInput {
				float4 pos  :   POSITION;    // 3D座標
				float4 color:   COLOR;
				float2 uv   :   TEXCOORD0;   // テクスチャ座標
			};

			struct VertexOutput {
				float4 v    :   SV_POSITION; // 2D座標
				float4 color:   COLOR;
				float2 uv   :   TEXCOORD0;   // テクスチャ座標
			};

//プロパティの内容を受け取る
			float4 _Color;
			float4 _Flip;
			sampler2D _MainTex;
			int _Light;
			int _Thick;
			float2 _MainTex_TexelSize;

			VertexOutput vert(VertexInput input) {
				VertexOutput output;
				//input.uv.x = 0.5 - input.uv.x;
				output.v = UnityObjectToClipPos(input.pos);
				output.uv = input.uv;
//もとの色(SpriteRendererのColor)と設定した色(TintColor)を掛け合わせる
				output.color = input.color * _Color;
				return output;
			}

			float4 frag(VertexOutput output) : SV_Target {
				/*
				float4 col = tex2D(_MainTex, output.uv);
				return lerp(col, float4(0, 0, 0, 1), fwidth(col.a));
				*/
				float4 c = tex2D(_MainTex, output.uv);
				float4 c2 = tex2D(_MainTex, float2(output.uv.x + _MainTex_TexelSize.x * _Thick, output.uv.y));
				float4 c3 = tex2D(_MainTex, float2(output.uv.x - _MainTex_TexelSize.x * _Thick, output.uv.y));
				float4 c4 = tex2D(_MainTex, float2(output.uv.x, output.uv.y + _MainTex_TexelSize.y * _Thick));
				float4 c5 = tex2D(_MainTex, float2(output.uv.x, output.uv.y - _MainTex_TexelSize.y * _Thick));
				c.rgb = c.a == 0 ? float3(1.0f, 1.0f, 1.0f) : c.rgb;
				c.a = c2.a + c3.a + c4.a + c5.a >= 1.0f ? 1.0f : 0.0f;
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
}
}