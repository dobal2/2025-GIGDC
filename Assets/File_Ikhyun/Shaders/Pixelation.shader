Shader "Hidden/FullScreen/Pixelate_URP63"
{
    Properties
    {
        _Intensity ("Effect Intensity", Range(0,1)) = 1

        // 픽셀 블록의 크기. 값이 클수록 더 거칠게(큰 픽셀) 보임.
        _PixelSize ("Pixel Size (Screen Pixels)", Range(1,256)) = 6

        // 픽셀 그리드가 움직이지 않게 중앙 기준으로 정렬할지(1), 좌상단 기준(0)
        _CenterAlign ("Center Align", Range(0,1)) = 1

        // 색을 단계화(레트로 느낌 강화). 0이면 꺼짐.
        _PosterizeSteps ("Posterize Steps (0=off)", Range(0,64)) = 0
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
        ZWrite Off
        ZTest Always
        Cull Off

        Pass
        {
            Name "Pixelate_Fullscreen"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BlitTexture);
            SAMPLER(sampler_BlitTexture);

            CBUFFER_START(UnityPerMaterial)
                float _Intensity;
                float _PixelSize;
                float _CenterAlign;
                float _PosterizeSteps;
            CBUFFER_END

            struct Attributes { uint vertexID : SV_VertexID; };
            struct Varyings { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; };

            Varyings Vert(Attributes input)
            {
                Varyings o;

                float2 pos;
                if (input.vertexID == 0) pos = float2(-1.0, -1.0);
                else if (input.vertexID == 1) pos = float2(-1.0,  3.0);
                else pos = float2( 3.0, -1.0);

                o.positionCS = float4(pos, 0.0, 1.0);

                float2 uv;
                if (input.vertexID == 0) uv = float2(0.0, 0.0);
                else if (input.vertexID == 1) uv = float2(0.0, 2.0);
                else uv = float2(2.0, 0.0);

                o.uv = uv;
                return o;
            }

            float3 Posterize(float3 c, float steps)
            {
                if (steps <= 0.5) return c;
                float s = max(2.0, steps);
                return floor(c * s) / s;
            }

            half4 Frag(Varyings i) : SV_Target
            {
                float2 uv = i.uv;

                // ✅ URP/플랫폼에 따라 화면이 뒤집혀 들어오는 경우 보정
                #if UNITY_UV_STARTS_AT_TOP
                    uv.y = 1.0 - uv.y;
                #endif

                // 스크린 픽셀 좌표(0..width, 0..height)
                float2 screenPx = uv * _ScreenParams.xy;

                // 픽셀 블록 크기 (최소 1px)
                float pxSize = max(1.0, _PixelSize);

                // 중앙 정렬 옵션: 블록 그리드가 화면 중앙 기준으로 안정적으로 보이게
                if (_CenterAlign > 0.5)
                {
                    float2 halfRes = _ScreenParams.xy * 0.5;
                    screenPx -= halfRes;
                    screenPx = floor(screenPx / pxSize) * pxSize;
                    screenPx += halfRes;
                }
                else
                {
                    screenPx = floor(screenPx / pxSize) * pxSize;
                }

                // 블록의 "중앙"을 찍어서 샘플링하면 깔끔하게 보임
                screenPx += pxSize * 0.5;

                float2 uvPix = screenPx / _ScreenParams.xy;

                float4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, uvPix);

                float3 c = col.rgb;
                c = Posterize(c, _PosterizeSteps);

                // 강도 블렌드
                float3 outC = lerp(col.rgb, c, _Intensity);
                return half4(outC, 1);
            }
            ENDHLSL
        }
    }
    Fallback Off
}