Shader "Custom/PSXRetro_URP"
{
    Properties
    {
        [MainTexture] _MainTex ("Base Map", 2D) = "white" {}
        [MainColor] _Color ("Tint", Color) = (1,1,1,1)

        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Render Face", Float) = 0

        // Normal Map
        [Normal] _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpStrength ("Normal Strength", Range(0, 2)) = 1.0

        // Emission Map
        [HDR] _EmissionColor ("Emission Color", Color) = (0,0,0,1)
        _EmissionMap ("Emission Map", 2D) = "black" {}

        // Vertex Jitter
        _VertexSnap ("Snap Resolution (higher = subtler)", Range(10, 640)) = 160

        // Affine Texture Mapping
        _AffineStrength ("Affine Strength (1 = full PS1 warp)", Range(0,1)) = 1

        // Color Quantization
        _ColorLevels ("Color Levels per Channel", Range(2, 64)) = 16
        _DitherStrength ("Dither Strength", Range(0,1)) = 0.5

        // Lighting
        _AmbientBoost ("Ambient Boost", Range(0,1)) = 0.25
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Geometry"
        }
        LOD 300
        Cull [_Cull]

        Pass
        {
            Name "ForwardLit"
            Tags
            {
                "LightMode"="UniversalForward"
            }

            HLSLPROGRAM
            #pragma target 3.0

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _LIGHT_LAYERS
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile _ _CLUSTER_LIGHT_LOOP

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uvPersp : TEXCOORD0;
                noperspective float2 uvAffine : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float3 normalWS : TEXCOORD3;
                float4 tangentWS : TEXCOORD4;
                float4 screenPos : TEXCOORD5;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_BumpMap);
            SAMPLER(sampler_BumpMap);
            TEXTURE2D(_EmissionMap);
            SAMPLER(sampler_EmissionMap);
            float4 _MainTex_ST;

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _EmissionColor;
                float _BumpStrength;
                float _VertexSnap;
                float _AffineStrength;
                float _ColorLevels;
                float _DitherStrength;
                float _AmbientBoost;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 posWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionWS = posWS;

                VertexNormalInputs normalInputs = GetVertexNormalInputs(IN.normalOS, IN.tangentOS);
                OUT.normalWS = normalInputs.normalWS;
                OUT.tangentWS = float4(normalInputs.tangentWS, IN.tangentOS.w * GetOddNegativeScale());

                float4 posCS = TransformWorldToHClip(posWS);
                float4 snapped = posCS;
                snapped.xyz = posCS.xyz / posCS.w;
                snapped.x = floor(snapped.x * _VertexSnap) / _VertexSnap;
                snapped.y = floor(snapped.y * _VertexSnap) / _VertexSnap;
                snapped.xyz *= posCS.w;
                OUT.positionCS = snapped;

                float2 uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.uvPersp = uv;
                OUT.uvAffine = uv;
                OUT.screenPos = ComputeScreenPos(snapped);

                return OUT;
            }

            static const float bayer4x4[16] =
            {
                0.0 / 16.0, 8.0 / 16.0, 2.0 / 16.0, 10.0 / 16.0,
                12.0 / 16.0, 4.0 / 16.0, 14.0 / 16.0, 6.0 / 16.0,
                3.0 / 16.0, 11.0 / 16.0, 1.0 / 16.0, 9.0 / 16.0,
                15.0 / 16.0, 7.0 / 16.0, 13.0 / 16.0, 5.0 / 16.0
            };

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = lerp(IN.uvPersp, IN.uvAffine, _AffineStrength);
                half4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * _Color;

                float3 normalWS = normalize(IN.normalWS);
                float3 tangentWS = IN.tangentWS.xyz;

                if (length(tangentWS) > 0.1 && _BumpStrength > 0.01)
                {
                    tangentWS = normalize(tangentWS);
                    float3 bitangentWS = cross(normalWS, tangentWS) * IN.tangentWS.w;
                    half4 normalSample = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, uv);
                    float3 normalTS = UnpackNormalScale(normalSample, _BumpStrength);
                    normalWS = normalize(TransformTangentToWorld(normalTS, half3x3(tangentWS, bitangentWS, normalWS)));
                }
                float3 N = normalWS;

                Light mainLight = GetMainLight();
                float mainDiff = saturate(dot(N, mainLight.direction) * 0.5 + 0.5);
                float3 totalLight = mainLight.color * (mainDiff * mainLight.distanceAttenuation);

                InputData inputData = (InputData)0;
                inputData.positionWS = IN.positionWS;
                inputData.positionCS = IN.positionCS;
                inputData.normalWS = normalWS;
                inputData.normalizedScreenSpaceUV = IN.screenPos.xy / max(IN.screenPos.w, 0.0001);

                #if defined(_ADDITIONAL_LIGHTS)
                uint pixelLightCount = GetAdditionalLightsCount();
                LIGHT_LOOP_BEGIN(pixelLightCount)
                    Light addLight = GetAdditionalLight(lightIndex, IN.positionWS, half4(1, 1, 1, 1));
                    float addDiff = saturate(dot(N, normalize(addLight.direction)) * 0.5 + 0.5);
                    totalLight += addLight.color * (addDiff * addLight.distanceAttenuation);
                LIGHT_LOOP_END
                #endif

                totalLight += SampleSH(N) + _AmbientBoost;
                albedo.rgb *= totalLight;

                half3 emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, uv).rgb * _EmissionColor.rgb;
                albedo.rgb += emission;

                float camDist = distance(_WorldSpaceCameraPos, IN.positionWS);
                float fade = saturate((camDist - 5) / max(15 - 5, 0.0001));
                albedo.rgb *= (1.0 - fade);

                float2 screenUV = inputData.normalizedScreenSpaceUV;
                float2 pixelCoord = screenUV * _ScreenParams.xy;
                uint px = (uint)pixelCoord.x % 4;
                uint py = (uint)pixelCoord.y % 4;
                float threshold = bayer4x4[py * 4 + px] - 0.5;

                float levels = max(_ColorLevels, 2.0);
                float3 ditheredColor = albedo.rgb + threshold * (_DitherStrength / levels);
                float3 quantized = floor(ditheredColor * levels) / levels;

                return half4(quantized, albedo.a);
            }
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode"="ShadowCaster"
            }
            ZWrite On
            ColorMask 0

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }
    }
    CustomEditor "PSXRetroShaderGUI"
    FallBack "Universal Render Pipeline/Lit"
}