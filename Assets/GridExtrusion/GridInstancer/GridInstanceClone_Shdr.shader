Shader "Custom/GridInstanceClone" {
    Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Map("Map", 2D) = "white" {}
        _MetalMap ("Metal Map", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "white" {}

        [HDR]
        _EmissionColorBottom ("Color Bottom", Color)=(0,0,0)
            [HDR]
        _EmissionColorTop ("Color Top", Color)=(1,1,1)

        _NormalMag("Normal", Range(0,1)) = 1.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM

        #include "UnityPBSLighting.cginc"
    // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _MetalMap;
        sampler2D _NormalMap;

        sampler2D _Map;

        struct Input {
            float2 uv_MainTex;
        };

        half3 _EmissionColorBottom;
        half3 _EmissionColorTop;

        half _NormalMag;
        half _Glossiness;
        half _Metallic;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        //   UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
#define _Color_arr Props
            UNITY_DEFINE_INSTANCED_PROP(float, _Row)
#define _Row_arr Props
            UNITY_DEFINE_INSTANCED_PROP(float, _Col)
#define _Col_arr Props
            //     UNITY_INSTANCING_CBUFFER_END(Props)
            UNITY_INSTANCING_BUFFER_END(Props)

        void vert(inout appdata_full v) 
        {
            // Green = col (u) 0-1 
            // Red = row (v) 0-1
            float2 uv = UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color).gr;

            // Extrude the cube based on height map
            float4 rgba = tex2Dlod(_Map, float4(uv, 0, 0));
            v.vertex.z *= 6.0 * rgba.a;
            //v.vertex.z *= 4.0*((rgba.r * 0.3) + (rgba.g * 0.59) + (rgba.b * 0.11));
        }

        void surf (Input IN, inout SurfaceOutputStandard o) {
           
            fixed4 c = UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color);
            float2 uv = float2(c.g, c.r);

            o.Albedo = tex2D(_Map, uv) * tex2D(_MainTex, IN.uv_MainTex);

            // Metallic and smoothness come from slider variables
           // o.Metalness = float3(1, 1, 1);
            o.Metallic = tex2D(_MetalMap, IN.uv_MainTex).r * _Metallic;
            o.Smoothness = tex2D(_MetalMap, IN.uv_MainTex).a * _Glossiness;

            o.Normal = lerp(o.Normal, UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex)), _NormalMag);

           // o.Emission = pow(tex2D(_Map, uv).a * .4, 6.0f) * lerp(_EmissionColorBottom, _EmissionColorTop, 1.0);

            float v = pow(tex2D(_Map, uv).a * .4, 5.0f);

            o.Emission = lerp(_EmissionColorBottom, _EmissionColorTop, v);

            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
