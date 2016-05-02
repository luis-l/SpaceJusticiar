Shader "Custom/Planet Shader" {
	Properties {

		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
	
		_LightAttenA ("Light Atten. A", Range(0, 1)) = 0.1
		_LightAttenB ("Light Atten. B", Range(0, 0.1)) = 0
		_Albedo ("Albedo", Range(1, 10)) = 5
		_Emission("Emission", Range(0, 1)) = 0
		_TransformScale("Transform Scale", float) = 1

		_SunPos("Sun Position", Vector) = (0, 0, 0)

		_AtmoSize("Atm. Size", Range(1, 4)) = 1.1
		_AtmoColor("Atm. Color", Color) = (0, 1, 1, 1)
		_AtmoGradientExp("Atm. Gradient Exponent", Range(0, 3)) = 1
		_AtmoBrightness("Atmo. Brightness", Range(0, 10)) = 1
		_AtmoEmission("Atm. Emission", Range(0, 1)) = 0
	}

	SubShader {

		 // Render the atmosphere
		Pass{

			Tags {"Queue"="Transparent" "RenderType"="Transparent" }

			Blend SrcAlpha One
			ZWrite Off
			Offset 0, 1

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform half4 _AtmoColor;
			uniform float _AtmoSize;
			uniform float _AtmoGradientExp;
			uniform float _AtmoBrightness;
			uniform float3 _SunPos;
			uniform float _LightAttenA;
			uniform float _LightAttenB;

			uniform float _AtmoEmission;
			uniform float _TransformScale;

			struct v2f{
				float4 vertexPos : SV_POSITION;
				float3 vertexWorldPos : TEXCOORD1;
			};

			// Create an extension of the circle around the planet to represent the atmosphere mesh.
			v2f vert(appdata_base v){
				v2f o;
				v.vertex.xyz *= _AtmoSize;
				o.vertexPos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.vertexWorldPos = mul(_Object2World, v.vertex).xyz;
				return o;
			}

			float4 frag(v2f i) : COLOR {

				float3 circleCenter = mul(_Object2World, float4(0, 0, 0, 1)).xyz;
				float3 posFromCenter = i.vertexWorldPos - circleCenter;

				// Calculate the fragment position from the surface of the mesh.
				// Subtract 1 since the mesh is a unit circle, which means that the surface is a distance of 1 (times transform scale) away from the center.
				// Anything inside the circle, treat as zero distance.
				float distanceToSurface = max(0, length(posFromCenter) - _TransformScale);

				// Normalization and gradient formulation of the atmosphere color.
				// Closer to the surface the brighter. Distance = 0 and value = 1
				// Farther from the surface the darker. Distance = 1 and value = 0
				// Equation used: f(x) = 1 - (x / (a-1))^b
				// x = distance of fragment from surface.
				// a = atmosphere size
				// b = atmosphere gradient exponent. If this value = 1 then it is linear gradient, if 2 then it is a quadratic gradient and so on.
				// Subtract 1 from atmosphere size since the surface is a distance of 1 away from the circle center.
				float distToSurfaceNormalized = distanceToSurface / (_TransformScale * (_AtmoSize - 1));
				float ratio = 1.0 - pow(distToSurfaceNormalized, _AtmoGradientExp);

				// Apply gradient ratio.
				_AtmoColor *= ratio;

				// Apply light
				float3 lightToVertex = posFromCenter - circleCenter + _SunPos;
				float3 lightDirection = normalize(lightToVertex);

				// Angle between the light ray and vertex on sphere.
				float incidence = max(_AtmoEmission, dot(lightDirection, normalize(posFromCenter)));

				float distanceToLight = length(lightToVertex);
				float attenuation = 1.0 / (1.0 + _LightAttenA * distanceToLight + _LightAttenB * distanceToLight * distanceToLight);

				float4 diffuseReflection = incidence * attenuation * _AtmoColor * _AtmoBrightness;
				return diffuseReflection;
			}

			ENDCG

		}

		// Render the planet body.
		Pass{
		
			Tags {"RenderType"="Opaque"}

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _Color;
			
			uniform float4 _LightColor0;
			uniform float _LightAttenA;
			uniform float _LightAttenB;
			uniform float _Albedo;
			uniform float _Emission;
			uniform float _TransformScale;

			uniform float3 _SunPos;
			
			struct v2f{
				float4 vertexPos : SV_POSITION;
				float3 vertexWorldPos : TEXCOORD2;
				float2 texcoord : TEXCOORD0;
			};
			
			v2f vert(appdata_base v){
				v2f o;
				
				o.vertexPos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.vertexWorldPos = mul(_Object2World, v.vertex).xyz;
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				
				return o;
			}

			float4 frag(v2f i) : COLOR {

				float3 circleCenter = mul(_Object2World, float4(0, 0, 0, 1)).xyz;

				// The distance from the center of the circle to the fragment position.
				float3 posFromCenter = i.vertexWorldPos - circleCenter;

				// Apply light //

				// Project the fragment position onto the sphere edge.
				float3 onSpherePos = posFromCenter;

				float xySquaredSum = pow(onSpherePos.x, 2) + pow(onSpherePos.y, 2); 
				float zSquared = pow(_TransformScale, 2) - xySquaredSum;
				onSpherePos.z = sqrt(abs(zSquared));

				// onSpherePos.z = -sqrt(pow(_TransformScale, 2) - pow(onSpherePos.x, 2) - pow(onSpherePos.y, 2));

				float3 lightToVertexOnSphere = onSpherePos - circleCenter + _SunPos;
				float3 lightdir = normalize(lightToVertexOnSphere);

				float incidence = max(_Emission, dot(lightdir, normalize(onSpherePos)));

				float distanceToLight = length(lightToVertexOnSphere);
				float attenuation = _Albedo / (1.0 + _LightAttenA * distanceToLight + _LightAttenB * distanceToLight * distanceToLight);

				float3 diffuseReflection = incidence * attenuation * _Color.rgb * tex2D(_MainTex, i.texcoord);
				return float4(diffuseReflection, 1.0);
			}

			ENDCG

		}

	} 
	FallBack "Diffuse"
}
