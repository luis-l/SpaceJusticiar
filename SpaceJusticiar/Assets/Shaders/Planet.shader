Shader "Custom/Planet"
{
    Properties
    {
        _MainTex("Texture (RGB)", 2D) = "black" {}
        _Color("Color", Color) = (0, 0, 0, 1)
        _AtmoColor("Atmosphere Color", Color) = (0.5, 0.5, 1.0, 1)
        _Size("Size", Range(0, 1)) = 0.1
        _Falloff("Falloff", Range(0, 15)) = 5
        _Transparency("Transparency", Range(0, 20)) = 15
    }
 
	SubShader
    {
		Tags {"LightMode" = "ForwardBase"}
        Pass
        {
            Name "PlanetBase"
            Cull Back
 
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
 
                #pragma fragmentoption ARB_fog_exp2
                #pragma fragmentoption ARB_precision_hint_fastest
 
                #include "UnityCG.cginc"
 
                uniform sampler2D _MainTex;
                uniform float4 _MainTex_ST;
                uniform float4 _Color;
                uniform float4 _AtmoColor;
 
                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float3 normal : TEXCOORD0;
                    float3 worldvertpos : TEXCOORD1;
                    float2 texcoord : TEXCOORD2;
                };
 
                v2f vert(appdata_base v)
                {
                    v2f o;
 
                    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
                    o.normal = mul((float3x3)_Object2World, v.normal);
                    o.worldvertpos = mul(_Object2World, v.vertex).xyz;
                    o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
 
                    return o;
                }
 
                float4 frag(v2f i) : COLOR
                {
                    i.normal = normalize(i.normal);
 
                    float4 color = tex2D(_MainTex, i.texcoord)*_Color;
 
                    return color*dot(_WorldSpaceLightPos0, i.normal);
                }
            ENDCG
        }
 
 		// Light side atmosphere - very radiant
		Tags {"LightMode" = "ForwardAdd" "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Pass
        {
            Name "FORWARD"
            Cull Front
            Blend SrcAlpha One
			ZWrite Off
 
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
 
                #pragma fragmentoption ARB_fog_exp2
                #pragma fragmentoption ARB_precision_hint_fastest
 
                #include "UnityCG.cginc"
 
                uniform float4 _Color;
                uniform float4 _AtmoColor;
                uniform float _Size;
                uniform float _Falloff;
                uniform float _Transparency;
 
                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float3 normal : TEXCOORD0;
                    float3 worldvertpos : TEXCOORD1;
                };
 
                v2f vert(appdata_base v)
                {
                    v2f o;
 
                    v.vertex.xyz += v.normal*_Size;
                    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
                    o.normal = mul((float3x3)_Object2World, v.normal);
                    o.worldvertpos = mul(_Object2World, v.vertex);
 
                    return o;
                }
 
                float4 frag(v2f i) : COLOR
                {
                    i.normal = normalize(i.normal);
                    float3 viewdir = normalize(i.worldvertpos-_WorldSpaceCameraPos);
 
                    float4 color = _AtmoColor;

					// 2D game uses the z axis as the camera forward
					float4 zAxis;
					zAxis.x = 0;
					zAxis.y = 0;
					zAxis.z = 1;

					// Illuminate only the edge of the mesh. The plane perpendicular to the z axis
					color.a = dot(zAxis, i.normal);
				
					// Amplify the color
					color.a = pow(color.a, _Falloff);

					// Use transparency to blend from bright to dark
					color.a *= _Transparency;

					// Have the light vector only illuminate the bright side of the mesh.
					color.a *= dot(i.normal, _WorldSpaceLightPos0);
					color.a = saturate(color.a) * saturate(color.a);
						
                    return color;
                }
            ENDCG
        }
        
        
        // Dark side atmosphere - much dimmer
        Tags {"LightMode" = "ForwardAdd" "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Pass
        {
            Name "FORWARD"
            Cull Front
            Blend SrcAlpha One
			ZWrite Off
 
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
 
                #pragma fragmentoption ARB_fog_exp2
                #pragma fragmentoption ARB_precision_hint_fastest
 
                #include "UnityCG.cginc"
 
                uniform float4 _Color;
                uniform float4 _AtmoColor;
                uniform float _Size;
                uniform float _Falloff;
                uniform float _Transparency;
 
                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float3 normal : TEXCOORD0;
                    float3 worldvertpos : TEXCOORD1;
                };
 
                v2f vert(appdata_base v)
                {
                    v2f o;
 
                    v.vertex.xyz += v.normal*_Size / 3;
                    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
                    o.normal = mul((float3x3)_Object2World, v.normal);
                    o.worldvertpos = mul(_Object2World, v.vertex);
 
                    return o;
                }
 
 				// Make a rim glow around the mesh. The plane perpendicular to the z axis
                float4 frag(v2f i) : COLOR
                {
                    i.normal = normalize(i.normal);
                    float3 viewdir = normalize(i.worldvertpos-_WorldSpaceCameraPos);
 
                    float4 color = _AtmoColor;

					float4 zAxis;
					zAxis.x = 0;
					zAxis.y = 0;
					zAxis.z = 1;

					color.a = dot(zAxis, i.normal);
					color.a = pow(color.a, _Falloff);
					color.a *= _Transparency / 2;
					color.a = saturate(color.a);

                    return color;
                }
            ENDCG
        }
    }
 
    FallBack "Diffuse"
}