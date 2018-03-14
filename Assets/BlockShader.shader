// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/BlockShader" {
    Properties{
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Emission("Emission", Range(0,4)) = 1.0
        _RGBShift("RGB Shift", float) = .1

    }

    SubShader{

        Tags{ "RenderType" = "Opaque" }
        LOD 200
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert
        // Use Shader model 3.0 target
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _WallMap;

        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
        };
        half _Glossiness;
        half _Metallic;

      //  #define _Color_arr Props
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

        float _Emission;
        float _Rows;
        float _Cols;
        float _RGBShift;
        float _Spectrum[256];
        float _SpectrumMag;

        void vert(inout appdata_full v) {
            // Green = col (u)  Red = row (v)  in normalized 0-1 values
            float2 uv = UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color).gr;

            // Extrude the cube based on height map
            v.vertex.z *= tex2Dlod(_WallMap, float4(uv, 0, 0)).a * 66.0f;
        }

        void surf(Input IN, inout SurfaceOutputStandard o) {
            // float2 uv = IN.uv_MainTex;

            // TODO: 2017.1.3 way of instancing
          //  fixed4 c = UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color);
            fixed4 c = UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color);


            // UV along the grid (not the uv of individual cube)
            float2 uv = float2(c.g, c.r);

            // normalized value going up the grid
          //  float phase = UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Row) / _Rows;
          // float cx = fmod(row, 2) * (cos(phase*6.28*13.0f + _Time.y)*.5 + .5);

            float3 emission = float3(0,0,0);
            float row = UNITY_ACCESS_INSTANCED_PROP(_Row_arr, _Row);//  round(UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color).g * _Cols);
            float col = UNITY_ACCESS_INSTANCED_PROP(_Col_arr, _Col);

          //  emission = lerp(_Color1, _Color2, cx);

            // Grab color from the wall map
            emission = tex2D(_WallMap, uv).rgb;

            // Color shift
            float GShift = lerp(0.0f, (cos(_Time.y)*.5 + .5) * 1, _RGBShift);
            float BShift = lerp(0.0f, (cos(_Time.y*2.5)*.5 + .5) * 1, _RGBShift);

            emission.r = tex2D(_WallMap, uv).r;
            emission.g = tex2D(_WallMap, float2(uv.x + _RGBShift, uv.y + GShift)).g;
            emission.b = tex2D(_WallMap, float2(uv.x + _RGBShift, uv.y + BShift)).b;


            // Spectrum
            int index = (int)saturate(round((round( (IN.worldPos.x/5.0f) * 50.) / 50.)) * 255);
            float spectrum = pow(_Spectrum[index], .55) * 30;
          //  emission.rgb *= spectrum;

            emission = clamp(emission, 0.0f, 4.00f);

            o.Albedo = emission;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
          //  o.Alpha = 1;
            o.Emission = emission * _Emission;
        }
    ENDCG
    }
        FallBack "Diffuse"
}
