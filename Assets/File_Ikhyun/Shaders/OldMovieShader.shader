Shader "Hidden/FullScreen/OldMovieFilm_URP63"
{
    Properties
    {
        _Intensity          ("Effect Intensity", Range(0,1)) = 1

        _GateWeave          ("Gate Weave", Range(0,0.01)) = 0.0025
        _GateWeaveSpeed     ("Gate Weave Speed", Range(0,5)) = 1.2

        _FlickerStrength    ("Flicker Strength", Range(0,0.5)) = 0.08
        _FlickerSpeed       ("Flicker Speed", Range(0,20)) = 8

        _GrainStrength      ("Grain Strength", Range(0,1)) = 0.25
        _GrainSize          ("Grain Size (px-ish)", Range(0.5,4)) = 1.5

        _DustStrength       ("Dust Strength", Range(0,1)) = 0.20
        _DustDensity        ("Dust Density", Range(0,1)) = 0.12
        _DustSpeed          ("Dust Speed", Range(0,2)) = 0.6

        _ScratchStrength    ("Scratch Strength", Range(0,1)) = 0.25
        _ScratchDensity     ("Scratch Density", Range(0,1)) = 0.08
        _ScratchSpeed       ("Scratch Speed", Range(0,5)) = 1.5

        _VignetteStrength   ("Vignette Strength", Range(0,1)) = 0.35
        _VignetteRoundness  ("Vignette Roundness", Range(0.5,2)) = 1.2

        _Sepia              ("Sepia Amount", Range(0,1)) = 0.35
        _Contrast           ("Contrast", Range(0.5,1.5)) = 1.05
        _Brightness         ("Brightness", Range(-0.2,0.2)) = 0.02

        _PosterizeSteps     ("Posterize Steps (0 = off)", Range(0,64)) = 0
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
        ZWrite Off
        ZTest Always
        Cull Off

        Pass
        {
            Name "OldMovieFilm_Fullscreen"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BlitTexture);
            SAMPLER(sampler_BlitTexture);

            CBUFFER_START(UnityPerMaterial)
                float _Intensity;

                float _GateWeave;
                float _GateWeaveSpeed;

                float _FlickerStrength;
                float _FlickerSpeed;

                float _GrainStrength;
                float _GrainSize;

                float _DustStrength;
                float _DustDensity;
                float _DustSpeed;

                float _ScratchStrength;
                float _ScratchDensity;
                float _ScratchSpeed;

                float _VignetteStrength;
                float _VignetteRoundness;

                float _Sepia;
                float _Contrast;
                float _Brightness;

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

            float Hash12(float2 p)
            {
                float3 p3 = frac(float3(p.xyx) * 0.1031);
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.x + p3.y) * p3.z);
            }

            float Hash13(float3 p)
            {
                p = frac(p * 0.1031);
                p += dot(p, p.zyx + 31.32);
                return frac((p.x + p.y) * p.z);
            }

            float Luma(float3 c) { return dot(c, float3(0.2126, 0.7152, 0.0722)); }

            float3 ApplySepia(float3 c, float amount)
            {
                float3 sep;
                sep.r = dot(c, float3(0.393, 0.769, 0.189));
                sep.g = dot(c, float3(0.349, 0.686, 0.168));
                sep.b = dot(c, float3(0.272, 0.534, 0.131));
                return lerp(c, sep, amount);
            }

            float2 GateWeaveUV(float2 uv, float t)
            {
                float wobX = sin(t * (_GateWeaveSpeed * 0.87) + 1.3) + sin(t * (_GateWeaveSpeed * 0.31) + 4.7);
                float wobY = sin(t * (_GateWeaveSpeed * 0.73) + 2.1) + sin(t * (_GateWeaveSpeed * 0.27) + 0.2);
                wobX *= 0.5; wobY *= 0.5;

                float jx = (Hash12(float2(t, 1.7)) - 0.5);
                float jy = (Hash12(float2(t, 9.2)) - 0.5);

                float2 offset = float2(wobX + jx * 0.35, wobY + jy * 0.35) * _GateWeave;
                return uv + offset;
            }

            float Flicker(float t)
            {
                float f = sin(t * _FlickerSpeed) * 0.5 + 0.5;
                float r = Hash12(float2(floor(t * _FlickerSpeed), 3.1));
                return 1.0 + (f - 0.5) * _FlickerStrength + (r - 0.5) * _FlickerStrength * 0.6;
            }

            float FilmGrain(float2 uv, float t, float grainSize)
            {
                float denom = max(0.001, grainSize);
                float2 p = uv * (_ScreenParams.xy / denom);
                float n = Hash13(float3(floor(p), floor(t * 24.0)));
                return (n - 0.5) * 2.0;
            }

            float DustMask(float2 uv, float t)
            {
                float2 p = uv;
                p.y += t * _DustSpeed * 0.15;

                float n = Hash13(float3(floor(p * 480.0), floor(t * 12.0)));
                float speck = step(1.0 - _DustDensity * 0.08, n);
                float n2 = Hash12(p * 900.0 + t * 3.7);
                speck *= smoothstep(0.2, 1.0, n2);
                return speck;
            }

            float ScratchMask(float2 uv, float t)
            {
                float timeSlice = floor(t * (2.0 + _ScratchSpeed));
                float baseN = Hash12(float2(timeSlice, 5.4));

                float m = 0.0;
                [unroll]
                for (int i = 0; i < 6; i++)
                {
                    float xi = Hash12(float2(timeSlice, 10.0 + i * 17.0));
                    float w  = lerp(0.0008, 0.0030, Hash12(float2(timeSlice, 22.0 + i)));

                    // ✅ 'line' 변수명 금지 -> lnMask 로 변경
                    float lnMask = smoothstep(w, 0.0, abs(uv.x - xi));

                    float br = Hash12(float2(uv.y * 120.0 + i * 3.1, timeSlice));
                    lnMask *= smoothstep(0.15, 1.0, br);

                    m = max(m, lnMask);
                }

                float gate = step(1.0 - _ScratchDensity * 0.25, baseN);
                m *= gate;

                float flutter = Hash12(float2(uv.y * 30.0, t * (1.5 + _ScratchSpeed)));
                m *= lerp(0.6, 1.2, flutter);
                return m;
            }

            float3 ColorGradeFilm(float3 c)
            {
                c = (c - 0.5) * _Contrast + 0.5;
                c += _Brightness;

                float l = Luma(c);
                c = lerp(c, l.xxx, 0.12);
                c = ApplySepia(c, _Sepia);
                return saturate(c);
            }

            float3 Posterize(float3 c, float steps)
            {
                if (steps <= 0.5) return c;
                float s = max(2.0, steps);
                return floor(c * s) / s;
            }

            float Vignette(float2 uv)
            {
                float2 p = uv - 0.5;
                p.x *= (_ScreenParams.x / _ScreenParams.y);
                float r = length(p);
                r = pow(r, _VignetteRoundness);
                float v = smoothstep(0.35, 0.95, r);
                return 1.0 - v * _VignetteStrength;
            }

            half4 Frag(Varyings i) : SV_Target
            {
                float t = _Time.y;
                float2 uv = i.uv;

                float2 uvWeaved = GateWeaveUV(uv, t);
                uvWeaved.y = 1.0 - uvWeaved.y;

                float4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, uvWeaved);
                float3 c = col.rgb;

                c *= Flicker(t);

                float g = FilmGrain(uv, t, _GrainSize);
                float lum = Luma(c);
                float grainAmt = _GrainStrength * lerp(0.75, 1.15, smoothstep(0.15, 0.85, lum));
                c += g * 0.08 * grainAmt;

                float dust = DustMask(uv, t);
                c += dust * _DustStrength * 0.65;

                float scratch = ScratchMask(uv, t);
                c = lerp(c, saturate(c + scratch * 0.8), _ScratchStrength);

                c *= Vignette(uv);

                c = ColorGradeFilm(c);
                c = Posterize(c, _PosterizeSteps);

                c = lerp(col.rgb, c, _Intensity);
                return half4(c, 1);
            }
            ENDHLSL
        }
    }
    Fallback Off
}