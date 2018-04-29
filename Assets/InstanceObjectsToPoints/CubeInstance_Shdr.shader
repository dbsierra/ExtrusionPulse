Shader "Custom/GridInstanceClone2" {
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

        _HeightMult("Height Multiplier", Range(0,1)) = 1.0
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
        half _HeightMult;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        //   UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float3, _uv)
#define _Uv_arr Props
            //     UNITY_INSTANCING_CBUFFER_END(Props)
            UNITY_INSTANCING_BUFFER_END(Props)

        void vert(inout appdata_full v) 
        {
            // The uv of the texture that this cube maps to
            float3 uv = UNITY_ACCESS_INSTANCED_PROP(_Uv_arr, _uv);

            // Extrude the cube based on the luminance of the height map
            float4 map = tex2Dlod(_Map, float4(uv.xy, 0, 0));

            map.r = saturate(map.r - .1);
            float va = step(map.r, uv.z);

            

            v.vertex.xyz *= va *saturate(1.3*pow(1.0 - uv.z, .4));

          //  v.vertex.z *= 6.0 * map.a * _HeightMult;
       //     v.vertex.z *= 4.0*((map.r * 0.3) + (map.g * 0.59) + (map.b * 0.11));
        }

        void surf (Input IN, inout SurfaceOutputStandard o) {
           
            float3 uv = UNITY_ACCESS_INSTANCED_PROP(_Uv_arr, _uv);

            o.Albedo = tex2D(_Map, uv.xy) * tex2D(_MainTex, IN.uv_MainTex);

            // Metallic and smoothness come from slider variables
           // o.Metalness = float3(1, 1, 1);
            o.Metallic = tex2D(_MetalMap, IN.uv_MainTex).r * _Metallic;
            o.Smoothness = tex2D(_MetalMap, IN.uv_MainTex).a * _Glossiness;

            o.Normal = lerp(o.Normal, UnpackNormal( tex2D(_NormalMap, IN.uv_MainTex) ), _NormalMag);

      //      o.Emission = pow(tex2D(_Map, uv).a * .4, 6.0f) * lerp(_EmissionColorBottom, _EmissionColorTop, 1.0);

            float v = pow(tex2D(_Map, uv.xy).a * .4, 5.0f);

          o.Emission = lerp(_EmissionColorBottom, _EmissionColorTop, pow(1.0-uv.z,2.0));


            o.Albedo =  tex2D(_Map, uv.xy).xyz;



        }
        ENDCG
    }
    FallBack "Diffuse"
}
