Shader "MyShader/GaussianBlur"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

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

			half _Offset;
			half4 _Direction;

			static const int samplingCount = 10;
			half _Weights[samplingCount];

			v2f vert(appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex.y *= -_ProjectionParams.x;
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target{
				half4 col = 0;

				float2 tmp;

				int j;
				[unroll]
				for (j = samplingCount - 1; j > 0; j--) 
				{
					tmp = i.uv - (_Offset * _Direction.xy * j);
					col += (step(tmp.x, 1.0f) * step(0, tmp.x) * step(tmp.y, 1.0f) * step(0, tmp.y)) * tex2D(_MainTex, tmp) * _Weights[j];
				}

				[unroll]
				for (j = 0; j < samplingCount; j++)
				{
					tmp = i.uv + (_Offset * _Direction.xy * j);
					col += (step(tmp.x, 1.0f) * step(0, tmp.x) * step(tmp.y, 1.0f) * step(0, tmp.y)) * tex2D(_MainTex, tmp) * _Weights[j];
				}

				return col;
			}

			ENDCG
		}
	}
}
