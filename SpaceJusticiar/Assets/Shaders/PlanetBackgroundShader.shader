Shader "Custom/Planet Background Shader" {
	Properties {

		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
	
		_LightAttenA ("Light Atten. A", Range(0, 1)) = 0.1
		_LightAttenB ("Light Atten. B", Range(0, 0.1)) = 0
		_Albedo ("Albedo", Range(1, 10)) = 5
		_Emission("Emission", Range(0, 1)) = 0
		_TransformScale("Transform Scale", float) = 1

		_SunPos("Sun Position", Vector) = (0, 0, 0)
	}

	SubShader {

		// Render the planet body.
		Pass{
		
			Tags {
				"RenderType"="Opaque"
				"Queue" = "Geometry+10"
			}

			ZWrite Off

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
