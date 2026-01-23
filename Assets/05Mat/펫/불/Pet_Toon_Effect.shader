Shader "Custom/Pet_Toon_Effect"
{
    Properties
    {
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        [MainColor] _BaseColor("Base Color", Color) = (1,1,1,1)
        
        // 림 라이트 (외곽 빛) 설정
        _RimColor("Rim Color", Color) = (1, 0.2, 0, 1) // 불꽃 색상 추천
        _RimPower("Rim Power", Range(0.5, 8.0)) = 3.0
        
        // 외곽선 설정
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth("Outline Width", Range(0, 0.1)) = 0.02
        
        // 에미션 설정 (기존 FirePet_Mat 설정 유지용)
        [HDR] _EmissionColor("Emission Color", Color) = (0,0,0,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }

        // --- 1번 패스: 외곽선 그리기 (Hull Outline) ---
        Pass
        {
            Name "Outline"
            Cull Front // 앞면을 깎아내어 뒷면만 그림

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; float3 normalOS : NORMAL; };
            struct Varyings { float4 positionCS : SV_POSITION; };

            float _OutlineWidth;
            float4 _OutlineColor;

            Varyings vert(Attributes input) {
                Varyings output;
                // 노멀 방향으로 모델을 살짝 부풀림
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                float4 posWS = float4(TransformObjectToWorld(input.positionOS.xyz), 1.0);
                posWS.xyz += normalWS * _OutlineWidth;
                output.positionCS = TransformWorldToHClip(posWS.xyz);
                return output;
            }

            half4 frag(Varyings input) : SV_Target { return _OutlineColor; }
            ENDHLSL
        }

        // --- 2번 패스: 메인 색상 및 림 라이트 ---
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; float3 normalOS : NORMAL; };
            struct Varyings { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; float3 normalWS : TEXCOORD1; float3 viewDirWS : TEXCOORD2; };

            sampler2D _BaseMap;
            float4 _BaseColor;
            float4 _RimColor;
            float _RimPower;
            float4 _EmissionColor;

            Varyings vert(Attributes input) {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.viewDirWS = GetWorldSpaceViewDir(TransformObjectToWorld(input.positionOS.xyz));
                return output;
            }

            half4 frag(Varyings input) : SV_Target {
                half4 col = tex2D(_BaseMap, input.uv) * _BaseColor;
                
                // 림 라이트 계산 (외곽 빛)
                float3 normal = normalize(input.normalWS);
                float3 viewDir = normalize(input.viewDirWS);
                float rim = 1.0 - saturate(dot(normal, viewDir));
                rim = pow(rim, _RimPower);
                
                half3 finalRGB = col.rgb + (rim * _RimColor.rgb) + _EmissionColor.rgb;
                return half4(finalRGB, col.a);
            }
            ENDHLSL
        }
    }
}