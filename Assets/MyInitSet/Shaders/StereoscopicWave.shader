Shader "MyShader/StereoscopicWave"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[Toggle]_Light("Light", Int) = 0

		[Enum(UnityEngine.Rendering.BlendMode)]
		_SrcFactor("Src Factor", Float) = 1  //spriteに充てるときは5(SrcAlpha)に設定

		[Enum(UnityEngine.Rendering.BlendMode)]
		_DstFactor("Dst Factor", Float) = 0  //spriteに充てるときは10(OneMinusSrcAlpha)に設定
	}

		SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Blend [_SrcFactor] [_DstFactor]

		Tags {
			"Queue" = "Transparent"
		}

		//Blend SrcAlpha OneMinusSrcAlpha

		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			static const float pi = 3.14159265;

			bool _IsVertical = true;
			float _Interval;
			float _Scope;
			float _Speed;

			v2f vert(appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex.y *= -_ProjectionParams.x;
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target{
				bool _IsVertical = true;
				float st_1 = _IsVertical ? i.uv.x : i.uv.y;
				float st_2 = _IsVertical ? i.uv.y : i.uv.x;
				_Interval = 0.6;
				_Scope = 0.05;
				float l = (_Time.x * _Speed) % _Interval;
				float baseLine = (float)(floor((st_1 - l + _Scope) / _Interval)) * _Interval + l;
				//_BaseLine = 0.5;
				l = abs(st_1 - baseLine);
				float2 st;
				st_1 -= baseLine;
				//st.x *= 0.5 + pow(l, 6) / scope;
				st_1 *= 0.5 + 0.5 * sin((l / _Scope) * (pi / 2));
				st_2 -= 0.025 * cos((l / _Scope) * (pi / 2));
				st_1 += baseLine;
				st.x = _IsVertical ? st_1 : st_2;
				st.y = _IsVertical ? st_2 : st_1;

				half4 col = tex2D(_MainTex, l < _Scope ? st : i.uv);
				//half4 col = tex2D(_MainTex, st);
				//half4 col = tex2D(_MainTex, i.uv.x < 0.6 && i.uv.x > 0.5 ? st : i.uv);

				return col;
			}

			ENDCG
		}
	}
}
