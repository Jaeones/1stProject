Shader "Custom/Sowrd_Final_Shader"
{
    Properties
    {
        // 유니티 표준 이름인 _BaseMap을 사용해야 텍스처가 제대로 감지됩니다.
        [MainTexture] _BaseMap("Base Texture", 2D) = "white" {}
        [MainColor]   _BaseColor("Base Color", Color) = (1,1,1,1)
        
        _OutlineWidth("Outline Width", Range(0, 0.05)) = 0.002
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        
        _RimColor("Rim Color", Color) = (1, 1, 1, 1)
        _RimPower("Rim Power", Range(0.5, 8.0)) = 3.0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" }

        // 1. 외곽선 패스
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

            float _OutlineWidth;
            float4 _OutlineColor;

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

        // 2. 캐릭터 본체 (텍스처 출력 강화)
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

            // 텍스처 샘플러 설정
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            float4 _BaseColor, _RimColor;
            float _RimPower;

            Varyings vert(Attributes input) {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.worldNormal = TransformObjectToWorldNormal(input.normalOS);
                output.viewDirWS = GetWorldSpaceViewDir(TransformObjectToWorld(input.positionOS.xyz));
                return output;
            }

            half4 frag(Varyings input) : SV_Target {
                // SAMPLE_TEXTURE2D 매크로를 사용하여 텍스처를 명확히 읽어옵니다.
                half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
                
                // 림 라이트 (빛이 아예 없는 곳에서도 캐릭터가 보이게 함)
                float fresnel = 1.0 - saturate(dot(normalize(input.worldNormal), normalize(input.viewDirWS)));
                float rim = pow(fresnel, _RimPower);
                col.rgb += rim * _RimColor.rgb;
                
                return col;
            }
            ENDHLSL
        }
    }
}