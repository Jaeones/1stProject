Shader "Custom/FireCharacter_Shadow_Shader"
{
    Properties
    {
        [MainTexture] _BaseMap ("Character Texture", 2D) = "white" {} 
        [MainColor] _BaseColor ("Base Tint", Color) = (1,1,1,1)
        
        [HDR] _EmissionColor ("Fire Glow Color", Color) = (1,0.4,0,1)
        _EmissionPower ("Glow Strength", Range(0, 10)) = 2.0
        
        _RimColor ("Edge Light", Color) = (1,0.9,0.5,1)
        _RimPower ("Edge Sharpness", Range(0.5, 8.0)) = 3.0
        
        _WobbleSpeed ("Wobble Speed", Range(0, 10)) = 4.0
        _WobbleAmount ("Wobble Amount", Range(0, 0.1)) = 0.02
    }

    SubShader
    {
        // 불꽃 캐릭터 특성에 맞춰 투명도 및 URP 태그 설정
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" "Queue"="Geometry" }

        // ------------------------------------------------------------------
        // 1. 캐릭터 본체 패스 (Glow + Wobble + Rim)
        // ------------------------------------------------------------------
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; float3 normalOS : NORMAL; };
            struct Varyings { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; float3 worldNormal : TEXCOORD1; float3 viewDirWS : TEXCOORD3; };

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            float4 _BaseColor, _EmissionColor, _RimColor;
            float _EmissionPower, _RimPower, _WobbleSpeed, _WobbleAmount;

            Varyings vert (Attributes input) {
                Varyings output;
                
                // [Wobble] 불꽃이 위로 갈수록 더 많이 흔들리게 계산
                float wave = sin(_Time.y * _WobbleSpeed + input.positionOS.y * 10.0) * _WobbleAmount * input.uv.y;
                input.positionOS.x += wave;

                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.worldNormal = TransformObjectToWorldNormal(input.normalOS);
                output.viewDirWS = GetWorldSpaceViewDir(TransformObjectToWorld(input.positionOS.xyz));
                return output;
            }

            half4 frag (Varyings input) : SV_Target {
                half4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                half4 col = tex * _BaseColor;

                // 1. 림라이트 (외곽광)
                float fresnel = 1.0 - saturate(dot(normalize(input.viewDirWS), normalize(input.worldNormal)));
                float rim = pow(fresnel, _RimPower);
                
                // 2. 발광(Emission) - 텍스처 밝기에 따라 적용
                float bright = dot(tex.rgb, float3(0.3, 0.59, 0.11)); 
                float3 emission = _EmissionColor.rgb * _EmissionPower * bright;

                col.rgb += emission; 
                col.rgb += _RimColor.rgb * rim; 
                
                return col;
            }
            ENDHLSL
        }

        // ------------------------------------------------------------------
        // 2. 그림자 생성 패스 (ShadowCaster) - **흔들림 동기화 포함**
        // ------------------------------------------------------------------
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

            struct Attributes { float4 positionOS : POSITION; float3 normalOS : NORMAL; float2 uv : TEXCOORD0; };
            struct Varyings { float4 positionCS : SV_POSITION; };

            float _WobbleSpeed, _WobbleAmount;

            Varyings vert (Attributes input) {
                Varyings output;
                
                // **중요**: 그림자 메쉬도 본체와 똑같은 로직으로 흔들려야 합니다.
                float wave = sin(_Time.y * _WobbleSpeed + input.positionOS.y * 10.0) * _WobbleAmount * input.uv.y;
                input.positionOS.x += wave;

                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                
                output.positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _MainLightPosition.xyz));
                return output;
            }

            half4 frag () : SV_Target { return 0; }
            ENDHLSL
        }
    }
}