Shader "Custom/Jiggle_Shadow_Shader"
{
    Properties
    {
        [MainTexture] _BaseMap("Base Texture", 2D) = "white" {}
        [MainColor]   _BaseColor("Base Color", Color) = (1,1,1,1)

        // 젤리 효과 (Jiggle)
        _WaveSpeed ("Jiggle Speed", Float) = 2.0
        _WaveHeight ("Jiggle Height", Float) = 0.05

        // 외곽선 및 림 라이트 (기본 기능 유지)
        _OutlineWidth("Outline Width", Range(0, 0.05)) = 0.002
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _RimColor("Rim Color", Color) = (1, 1, 1, 1)
        _RimPower("Rim Power", Range(0.5, 8.0)) = 3.0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" }

        // 1. 외곽선 패스 (Outline)
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "SRPDefaultUnlit" }
            Cull Front
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; float3 normalOS : NORMAL; };
            struct Varyings { float4 positionCS : SV_POSITION; };
            float _OutlineWidth; float4 _OutlineColor;

            Varyings vert(Attributes input) {
                Varyings output;
                float3 worldNormal = TransformObjectToWorldNormal(input.normalOS);
                float3 worldPos = TransformObjectToWorld(input.positionOS.xyz) + (worldNormal * _OutlineWidth);
                output.positionCS = TransformWorldToHClip(worldPos);
                return output;
            }
            half4 frag() : SV_Target { return _OutlineColor; }
            ENDHLSL
        }

        // 2. 캐릭터 본체 (젤리 효과 + 림 라이트)
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; float3 normalOS : NORMAL; };
            struct Varyings { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; float3 worldNormal : TEXCOORD1; float3 viewDirWS : TEXCOORD3; };

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            float4 _BaseColor, _RimColor;
            float _WaveSpeed, _WaveHeight, _RimPower;

            Varyings vert(Attributes input) {
                Varyings output;
                // 젤리 효과: 상하 출렁임 계산
                float wave = sin(_Time.y * _WaveSpeed + input.positionOS.y) * _WaveHeight;
                input.positionOS.xyz += input.positionOS.xyz * wave;

                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.worldNormal = TransformObjectToWorldNormal(input.normalOS);
                output.viewDirWS = GetWorldSpaceViewDir(TransformObjectToWorld(input.positionOS.xyz));
                return output;
            }

            half4 frag(Varyings input) : SV_Target {
                half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
                
                // 림 라이트
                float fresnel = 1.0 - saturate(dot(normalize(input.worldNormal), normalize(input.viewDirWS)));
                float rim = pow(fresnel, _RimPower);
                col.rgb += rim * _RimColor.rgb;
                
                return col;
            }
            ENDHLSL
        }

        // 3. 그림자 생성 패스 (ShadowCaster)
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            ZWrite On ZTest LEqual ColorMask 0

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            struct Attributes { float4 positionOS : POSITION; float3 normalOS : NORMAL; };
            struct Varyings { float4 positionCS : SV_POSITION; };
            float _WaveSpeed, _WaveHeight; 

            Varyings vert(Attributes input) {
                Varyings output;
                // 그림자도 본체와 똑같이 출렁이게 처리
                float wave = sin(_Time.y * _WaveSpeed + input.positionOS.y) * _WaveHeight;
                input.positionOS.xyz += input.positionOS.xyz * wave;

                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _MainLightPosition.xyz));
                return output;
            }
            half4 frag() : SV_Target { return 0; }
            ENDHLSL
        }
    }
}