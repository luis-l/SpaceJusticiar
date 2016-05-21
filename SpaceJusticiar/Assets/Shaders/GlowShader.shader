Shader "Custom/Glow Shader" {
	Properties {

		_MainTex ("Texture", 2D) = "white" {}
	
		_TransformScale("Transform Scale", float) = 1

		_GlowSize("Size", Range(1, 4)) = 1.1
		_Color("Color", Color) = (0, 1, 1, 1)
		_GradientExp("Gradient Exponent", Range(0, 3)) = 1
		_Brightness("Brightness", Range(0, 10)) = 1
	}

	SubShader {

		 // Render the atmosphere
		Pass{

			Tags {

				"Queue"="Transparent" 
				"IgnoreProjector"="True" 
				"RenderType"="Transparent" 
			}

			Blend SrcAlpha One
			ZWrite Off

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform float4 _Color;
			uniform float _GlowSize;
			uniform float _GradientExp;
			uniform float _Brightness;
			uniform float _TransformScale;

			struct v2f{
				float4 vertexPos : SV_POSITION;
				float3 vertexWorldPos : TEXCOORD1;
			};

			// Create an extension of the unit circle around the sun mesh
			v2f vert(appdata_base v){
				v2f o;

				v.vertex.xyz *= _GlowSize;
				o.vertexPos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.vertexWorldPos = mul(_Object2World, v.vertex).xyz;

				return o;
			}

			float4 frag(v2f i) : COLOR {

				float3 circleCenter = mul(_Object2World, float4(0, 0, 0, 1)).xyz;
				float3 posFromCenter = i.vertexWorldPos - circleCenter;

				// In order to get the distance of a fragment to the surface of the circle use,
				// dist = len(posFromCenter - 1 * s), where s is the transform scale.
				// Anything inside the circle, treat as zero.
				float distanceToSurface = max(0, length(posFromCenter) - _TransformScale);

				// Normalization and gradient formulation of the glow color.
				// Closer to the surface the brighter. Distance = 0 and value = 1
				// Farther from the surface the darker. Distance = 1 and value = 0
				// Equation used: f(x) = 1 - (x / ( s * (a-1)) )^b
				// x = distance of fragment from surface.
				// a = glow size
				// b = gradient exponent. If this value = 1 then it is linear gradient, if 2 then it is a quadratic gradient and so on.
				// Subtract 1 from the sun light size since the surface is a distance of 1 away from the unit circle center.
				// s = transformation scale
				float distToSurfaceNormalized = distanceToSurface / (_TransformScale * (_GlowSize - 1));
				float ratio = 1.0 - pow(distToSurfaceNormalized, _GradientExp);

				// Apply gradient ratio.
				_Color *= ratio;

				float4 diffuseReflection = _Color * _Brightness;
				return diffuseReflection;
			}

			ENDCG

		}
	}

	FallBack "Diffuse"
}
