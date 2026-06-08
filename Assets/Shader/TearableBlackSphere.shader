Shader "Custom/TearableBlackSphere"
{
    Properties
    {
        _Color ("Black Color", Color) = (0,0,0,1)
        _TextTex ("Text Texture", 2D) = "white" {}
        _TextColor ("Text Color", Color) = (1,1,1,1)
        _TextTilingOffset ("Text Tiling Offset", Vector) = (4,4,-1.5,-1.5)
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _Cutoff ("Cutoff", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent+100"
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "ForwardUnlit"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_TextTex);
            SAMPLER(sampler_TextTex);

            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _TextColor;
                float4 _TextTilingOffset;
                float _Cutoff;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, input.uv).r;

                if (mask < _Cutoff)
                    discard;

                float2 textUV = input.uv * _TextTilingOffset.xy + _TextTilingOffset.zw;
                half4 textSample = SAMPLE_TEXTURE2D(_TextTex, sampler_TextTex, textUV);

                half textAlpha = textSample.a * _TextColor.a;
                half3 finalColor = lerp(_Color.rgb, _TextColor.rgb, textAlpha);

                return half4(finalColor, _Color.a);
            }

            ENDHLSL
        }
    }
}