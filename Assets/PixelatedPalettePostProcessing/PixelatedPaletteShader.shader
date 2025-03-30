// This shader draws a texture on the mesh.
Shader "Custom/URPPixelatedPaletteShader"
{
    // The _BaseMap variable is visible in the Material's Inspector, as a field
    // called Base Map.
    Properties
    {
        [HideInInspector]_MainTex("Base Map", 2D) = "white"
        _LUT("LUT", 2D) = "white" {}
        _EffectIntensity("Effect Intensity", Range(0, 1)) = 1
        _PaletteCellSize ("Palette Cell Size", Int) = 2
        _Position2D("Position", Vector) = (0, 0, 0, 0)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Cull Back
            Blend One Zero
            ZTest LEqual
            ZWrite On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            
            struct Attributes
            {
                float4 positionOS   : POSITION;
                // The uv variable contains the UV coordinate on the texture for the
                // given vertex.
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                // The uv variable contains the UV coordinate on the texture for the
                // given vertex.
                float2 uv           : TEXCOORD0;
                float depth         : DEPTH;
            };

            // This macro declares _MainTex as a Texture2D object.
            TEXTURE2D(_MainTex);
            SamplerState sampler_point_clamp;
            // This macro declares the sampler for the _MainTex texture.
            //SAMPLER(sampler_MainTex);

            // This macro declares _LUT as a Texture2D object.
            TEXTURE2D(_LUT);
            // This macro declares the sampler for the _LUT texture.
            SAMPLER(sampler_LUT);

            
            float4 _LUT_TexelSize;
            float _EffectIntensity;
            int _PaletteCellSize;

            uniform float2 _BlockCount = (128.0f, 128.0f);
            uniform float2 _BlockSize;
            uniform float2 _HalfBlockSize;
            

            CBUFFER_START(UnityPerMaterial)
                // The following line declares the _BaseMap_ST variable, so that you
                // can use the _BaseMap variable in the fragment shader. The _ST
                // suffix is necessary for the tiling and offset function to work.
                float4 _MainTex_ST;
            CBUFFER_END


            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                // The TRANSFORM_TEX macro performs the tiling and offset
                // transformation.
                OUT.depth = IN.positionOS.z;
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                //Pixelate   
                //float2 blockPos = floor(IN.uv * _BlockCount);
                //float2 blockCenter = blockPos * _BlockSize + _HalfBlockSize;
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_point_clamp, IN.uv);            		        
                
                float yPos = _LUT_TexelSize.w / _PaletteCellSize * _LUT_TexelSize.y;                
                float4 gradedCol = (1,1,1,1);

                //Sample color from LUT palette
	   			float dist = 10000000.0;
                for (int i = _PaletteCellSize * 0.5; i < _LUT_TexelSize.z; i += _PaletteCellSize) 
                {
                    float2 palettePos = float2(i * _LUT_TexelSize.x, yPos);
                    float4 paletteCol = SAMPLE_TEXTURE2D(_LUT, sampler_point_clamp, palettePos);

                    float d = distance(color, paletteCol);
	   				if (d < dist) {
	   					dist = d;
	   					gradedCol = paletteCol;                        
	   				}
                }
                return lerp(color, gradedCol, _EffectIntensity);                
            }
            ENDHLSL
        }
    }
} //float4