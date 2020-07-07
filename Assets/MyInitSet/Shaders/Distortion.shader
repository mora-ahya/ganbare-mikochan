Shader "MyShader/Distortion"
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

			v2f vert(appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex.y *= -_ProjectionParams.x;
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target{
				float r = 0.5;
				float2 origin = float2(0.5, 0.5);
				float l = length(i.uv - origin);
				float2 st = i.uv;
				st -= origin;
				st *= 1.5 + 15 * pow(l - r, 2);
				st /= 1;
				st += origin;

				half4 col = (step(st.x, 1.0f) * step(0, st.x) * step(st.y, 1.0f) * step(0, st.y)) * tex2D(_MainTex, st);
				
				return col;
			}

			ENDCG
		}
	}
}