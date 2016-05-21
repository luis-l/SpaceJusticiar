Shader "Custom/Light Streak Shader" {
	Properties {

		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		_RelVel("Relative Velocity", Vector) = (1, 1, 1, 1)

	}
	SubShader {

		Tags { 
			"RenderType"="Transparent"
			"IgnoreProjector"="True"
			"Queue" = "Transparent"
		}

		
		Pass { 

			Fog { Mode Off } 

			Blend SrcAlpha One

			Lighting Off

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;

			uniform float4 _Color;
			uniform float4 _RelVel;

			struct v2f{

				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;

			};

			v2f vert (appdata_base v){

				v2f o;

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex * _RelVel);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

				return o;
			}

			half4 frag(v2f i) : COLOR{

			    return tex2D(_MainTex, i.uv) * _Color;
			} 

			ENDCG
		}

	}
	FallBack "Diffuse"
}
