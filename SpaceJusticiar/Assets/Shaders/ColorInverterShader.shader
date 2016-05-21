
Shader "Custom/ColorInverterShader" {

	Properties { 

		_MainTex ("Alpha (A) only", 2D) = "white" {}

		 // Scales the final color output after inversion. 
		_ColorFactors ("Color Multipliers", Vector) = (1, 1, 1, 1)
	} 

	SubShader { 

		Tags {
		 
			"RenderType"="Transparent" 
			"Queue" = "Overlay" 
			"IgnoreProjector"="True" 
		} 

		// Grab the screen behind the object into _GrabTexture
        GrabPass {"_GrabTexture" }

		Pass { 

			Fog { Mode Off } 

			Blend SrcAlpha OneMinusSrcAlpha

			Lighting Off
			ZWrite Off
			ZTest Always

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform sampler2D _GrabTexture;

			uniform float4 _ColorFactors;

			struct v2f{

				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 uvgrab : TEXCOORD1;
			};

			v2f vert (appdata_base v){

				v2f o;

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uvgrab = ComputeGrabScreenPos(o.pos);

				return o;
			}

			half4 frag(v2f i) : COLOR{

				// Original color of the texture
			    half4 orig = tex2D(_MainTex, i.uv);

			    // The destination color of the screen.
			    half4 passCol = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvgrab));

			    // Set the texture color as the inverse destination color
			    // But perserve the original opacity.
			    orig.rgb = 1 - passCol;

			    // Bias the color from the factors.
			    orig.r *= _ColorFactors.r;
			    orig.g *= _ColorFactors.g;
			    orig.b *= _ColorFactors.b;

				return orig;
			} 

			ENDCG
		}

	}

	FallBack "Diffuse"
}
