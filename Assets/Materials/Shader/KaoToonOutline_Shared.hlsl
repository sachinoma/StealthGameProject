// �������include conflict�h�~(.hlsl)
#pragma once

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
// For Lit�V�F�[�_�[�p
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

// �g��Ȃ�
//#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
// �ȉ���hlsl�ő�p
#include "KaoOutlineUtil.hlsl"
#include "KaoZOffset.hlsl"
#include "KaoInvLerpRemap.hlsl"

// ���_�V�F�[�_�ւ̓��͒�`
struct Attributes
{
    float3 positionOS   : POSITION;
    half3 normalOS      : NORMAL;
    half4 tangentOS     : TANGENT;
    float2 uv           : TEXCOORD0;
};

// �t���O�����g�V�F�[�_�֓n���f�[�^
struct Varyings
{
    float2 uv                       : TEXCOORD0;
    float4 positionWSAndFogFactor   : TEXCOORD1; // xyz: positionWS, w: vertex fog factor
    half3 normalWS                  : TEXCOORD2;
    float4 positionCS               : SV_POSITION;
};

// sampler2D��CBUFFER�ɓ���Ȃ�
sampler2D _BaseMap;
sampler2D _EmissionMap;
sampler2D _OcclusionMap;
sampler2D _OutlineZOffsetMaskTex;

// SRP batching���g����悤�ɁA�K�v�ȕ����S�����̈��CBUFFER�ɓ���܂�(�d�v)
CBUFFER_START(UnityPerMaterial)

// Face settings
float   _IsFace;

// base color
float4  _BaseMap_ST;
half4   _BaseColor;

// alpha
half    _Cutoff;

// emission
float   _UseEmission;
half3   _EmissionColor;
half    _EmissionMulByBaseColor;
half3   _EmissionMapChannelMask;

// occlusion
float   _UseOcclusion;
half    _OcclusionStrength;
half4   _OcclusionMapChannelMask;
half    _OcclusionRemapStart;
half    _OcclusionRemapEnd;

// lighting
half3   _IndirectLightMinColor;
half    _CelShadeMidPoint;
half    _CelShadeSoftness;

// shadow mapping
half    _ReceiveShadowMappingAmount;
float   _ReceiveShadowMappingPosOffset;
half3   _ShadowMapColor;

// outline
float   _OutlineWidth;
half3   _OutlineColor;
float   _OutlineZOffset;
float   _OutlineZOffsetMaskRemapStart;
float   _OutlineZOffsetMaskRemapEnd;

CBUFFER_END

// applyShadowBiasFixToHClipPos()�p�̓���ȕϐ��Aper material uniform�ł͂Ȃ��̂�CBUFFER�̊O�ŏ����Ă����v�B
float3 _LightDirection;

struct ToonSurfaceData
{
    half3   albedo;
    half    alpha;
    half3   emission;
    half    occlusion;
};
struct ToonLightingData
{
    half3   normalWS;
    float3  positionWS;
    half3   viewDirectionWS;
    float4  shadowCoord;
};

//-----------------------------
// ���_�V�F�[�_�̏����Ƌ@�\
//-----------------------------

float3 TransformPositionWSToOutlinePositionWS(float3 positionWS, float positionVS_Z, float3 normalWS)
{
    //���Ȃ����g�̕��@�ɒu�������邱�Ƃ��ł��܂��I �����ł́A�`���[�g���A���̗��R�ŊȒP�ȃ��[���h�X�y�[�X���\�b�h�������܂����A����͍ŗǂ̕��@�ł͂���܂���I
    float outlineExpandAmount = _OutlineWidth * GetOutlineCameraFovAndDistanceFixMultiplier(positionVS_Z);
    return positionWS + normalWS * outlineExpandAmount;
}

// ToonShaderIsOutline not defined = �ʏ�̍��W�ϊ�������B
// ToonShaderIsOutline defined     = �ʏ�̍��W�ϊ������� + ���_��normal�̕����ɏ��������o���B
Varyings VertexShaderWork(Attributes input)
{
    Varyings output;

    // VertexPositionInputs �͐V�������W�ϊ��@�\
    // �����N�Fhttps://tips.hecomi.com/entry/2019/10/27/152520
    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS);

    // VertexNormalInputs ��normal�Atangent�Abittangent���܂܂�Ă��܂�(world space)
    VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

    float3 positionWS = vertexInput.positionWS;

    // �A�E�g���C���ׂ̈ɒ��_��normal�̕����ɏ��������o��(world space)
#ifdef ToonShaderIsOutline
    positionWS = TransformPositionWSToOutlinePositionWS(vertexInput.positionWS, vertexInput.positionVS.z, vertexNormalInput.normalWS);
#endif

    // ���_���Ƃ�ComputeFogFactor�����s(�̂�UNITY_TRANSFER_FOG())
    float fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

    // TRANSFORM_TEX�͓���
    output.uv = TRANSFORM_TEX(input.uv, _BaseMap);

    // positionWS(xyz)��fog(w)�����vector4�ɂ���B
    output.positionWSAndFogFactor = float4(positionWS, fogFactor);

    // GetVertexNormalInputs�̎��͊���normlaized�ς݂ł��B
    output.normalWS = vertexNormalInput.normalWS;

    output.positionCS = TransformWorldToHClip(positionWS);

#ifdef ToonShaderIsOutline
    // [Read ZOffset mask texture]
    // ���_�V�F�[�_�[��tex2D()���g���Ȃ��̂�(�܂����X�^���C�Y���ĂȂ��Addx��ddy�͖��m)
    // ������tex2Dlod()���g���Bexplict mip level 0���l�ڂ�param uv�ɒu��
    float outlineZOffsetMaskTexExplictMipLevel = 0;
    float outlineZOffsetMask = tex2Dlod(_OutlineZOffsetMaskTex, float4(input.uv, 0, outlineZOffsetMaskTexExplictMipLevel)).r; //�����͍����e�N�X�`��

    // [Remap ZOffset texture value]
    // texture read value�𔽓]����(black = apply ZOffset)�A�ʏ��outline mask texture�͂��̃t�H�[�}�b�g���g���Ă�(black = hide outline)
    outlineZOffsetMask = 1 - outlineZOffsetMask;
    outlineZOffsetMask = invLerpClamp(_OutlineZOffsetMaskRemapStart, _OutlineZOffsetMaskRemapEnd, outlineZOffsetMask);// ���[�U�[���l�𔽓]��������Aremap���邱�Ƃ�������

    // [ZOffset��K�p���A���}�b�v���ꂽ�l��ZOffset�}�X�N�Ƃ��Ďg�p����]
    output.positionCS = KaoGetNewClipPosWithZOffset(output.positionCS, _OutlineZOffset * outlineZOffsetMask + 0.03 * _IsFace);
#endif

    // ShadowCaster�p�X��positionCS�ɓ���ȏ������K�v�A�łȂ����shadow artifact���N����
    //-----------------------------
#ifdef ToonShaderApplyShadowBiasFix
    // URP/Shaders/ShadowCasterPass.hlsl��GetShadowPositionHClip()
    // https://github.com/Unity-Technologies/Graphics/blob/master/Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl
    float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, output.normalWS, _LightDirection));

#if UNITY_REVERSED_Z
    positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
#else
    positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
#endif
    output.positionCS = positionCS;
#endif
    //-----------------------------
    return output;
}

//-----------------------------
// �t���O�����g�V�F�[�_�̋@�\ (�X�e�b�v1�F���C�e�B���O�v�Z�ׂ̈ɍ\���̂�����)
//-----------------------------
half4 GetFinalBaseColor(Varyings input)
{
    return tex2D(_BaseMap, input.uv) * _BaseColor;
}
half3 GetFinalEmissionColor(Varyings input)
{
    half3 result = 0;
    if (_UseEmission)
    {
        result = tex2D(_EmissionMap, input.uv).rgb * _EmissionMapChannelMask * _EmissionColor.rgb;
    }

    return result;
}
half GetFinalOcculsion(Varyings input)
{
    half result = 1;
    if (_UseOcclusion)
    {
        half4 texValue = tex2D(_OcclusionMap, input.uv);
        half occlusionValue = dot(texValue, _OcclusionMapChannelMask);
        occlusionValue = lerp(1, occlusionValue, _OcclusionStrength);
        occlusionValue = invLerpClamp(_OcclusionRemapStart, _OcclusionRemapEnd, occlusionValue);
        result = occlusionValue;
    }

    return result;
}
void DoClipTestToTargetAlphaValue(half alpha)
{
#if _UseAlphaClipping
    clip(alpha - _Cutoff);
#endif
}
ToonSurfaceData InitializeSurfaceData(Varyings input)
{
    ToonSurfaceData output;

    // albedo & alpha
    float4 baseColorFinal = GetFinalBaseColor(input);
    output.albedo = baseColorFinal.rgb;
    output.alpha = baseColorFinal.a;
    DoClipTestToTargetAlphaValue(output.alpha);//�\�ł����early exit

    // emission
    output.emission = GetFinalEmissionColor(input);

    // occlusion
    output.occlusion = GetFinalOcculsion(input);

    return output;
}
ToonLightingData InitializeLightingData(Varyings input)
{
    ToonLightingData lightingData;
    lightingData.positionWS = input.positionWSAndFogFactor.xyz;
    lightingData.viewDirectionWS = SafeNormalize(GetCameraPositionWS() - lightingData.positionWS);
    lightingData.normalWS = normalize(input.normalWS); //��Ԃ��ꂽnormal�͒P�ʃx�N�g���ł͂Ȃ��Anormalize����K�v������B

    return lightingData;
}

//-----------------------------
// �t���O�����g�V�F�[�_�̋@�\ (�X�e�b�v2�F���C�e�B���O��final color�̌v�Z)
//-----------------------------

// ���C�e�B���O�Ɋւ���S���̎��͂���hlsl�ɍڂ��Ă�B
// ����hlsl��ҏW����ƁA�命���̃r�W���A���̌��ʂ�ς��邱�Ƃ��ł��܂��B
#include "KaoToonOutline_LightingEquation.hlsl"

// ���̊֐��̓��C�e�B���O�̃��W�b�N���܂܂�Ă��܂���B�����r�W���A���̌��ʂ�F��ȏ��ɓn�������B
// ���̊֐��̎d���́u�e�}�b�s���O�̐[�x�e�X�gpositionWS�I�t�Z�b�g������v
// �e�}�b�s���O�ł́A�������王�_�܂ł̋�����\���[�x�l���A���炩���ߌv�Z���Ă����āA�����p���ĉe��\�����邱�Ƃ��ł��܂��B
// �[�x�e�X�gpositionWS�I�t�Z�b�g�́A���̐[�x�l���ǂ̂悤�Ɍv�Z���邩�����߂���̂ŁA���̂��߂Ɏg������̂ł��B
half3 ShadeAllLights(ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    // �ԐڏƖ�
    half3 indirectResult = ShadeGI(surfaceData, lightingData);

    //////////////////////////////////////////////////////////////////////////////////
    // URP�����ۓI�Ȍ��ׂ̈ɒ񋟂������C�g�̍\���̂�
    // �ȉ��̏�񂪊܂܂�Ă��܂��B
    // - direction           ����
    // - color               �F
    // - distanceAttenuation ��������
    // - shadowAttenuation   �e�̌���
    //
    // URP�̓��C�g�ƃv���b�g�t�H�[���ɉ����āA�Ⴄ�V�F�[�f�B���O�A�v���[�`������B
    // �V�F�[�f�B���O�A�v���[�` ���� ����e���ǂ̂悤�ɕ\�����邩�����߂���@���w���܂��B
    // ���C�g �V�F�[�_�ϐ��𒼐ڎQ�Ƃ��鎖�̓_���ł��B
    // �����
    // -GetMainLight()
    // -GetLight()
    // �̊֐����g���ă��C�g�̍\���̂𖄂߂�
    //////////////////////////////////////////////////////////////////////////////////

    //==============================================================================================
    // ���C�����C�g�͍ł����邢�������̌��ł��B
    // ���C�g���[�v�̊O�ŃV�F�[�f�B���O����A����̕ϐ��Z�b�g�ƃV�F�[�f�B���O�p�X�������Ă���̂ŁA�B��̕�������������ꍇ�ɂ͂ł��邾�������ɏ����ł��܂��B
    // �K�v�ɉ����āAshadowCoord��n�����Ƃ��ł��܂��B���̏ꍇ�AshadowAttenuation���v�Z����܂��B
    Light mainLight = GetMainLight();

    float3 shadowTestPosWS = lightingData.positionWS + mainLight.direction * (_ReceiveShadowMappingPosOffset + _IsFace);
#ifdef _MAIN_LIGHT_SHADOWS
    // �t���O�����g�V�F�[�_�̒���shadowCoord���v�Z����ɂ́A�����������ς�肪����܂��B
    // https://forum.unity.com/threads/shadow-cascades-weird-since-7-2-0.828453/#post-5516425

    // _ReceiveShadowMappingPosOffset�́A�e�̔�r�ʒu�̃I�t�Z�b�g�𐧌䂵�܂��B
    // ����́A�ʏ�A��̂悤�ȉe�ɕq���ȃG���A�ŏX���Z���t�V���h�E���B�����߂ɍs���܂��B
    float4 shadowCoord = TransformWorldToShadowCoord(shadowTestPosWS);
    mainLight.shadowAttenuation = MainLightRealtimeShadow(shadowCoord);
#endif 

    // Main light
    half3 mainLightResult = ShadeSingleLight(surfaceData, lightingData, mainLight, false);

    //==============================================================================================
    // ���̒ǉ����C�g

    half3 additionalLightSumResult = 0;

#ifdef _ADDITIONAL_LIGHTS
    // �����_�����O����Ă���I�u�W�F�N�g�ɉe����^���郉�C�g�̐��l��Ԃ��܂��B
    // �����̃��C�g�́AURP�̃t�H���[�h�����_���[�ŃI�u�W�F�N�g���ƂɃJ�����O����܂��B.
    int additionalLightsCount = GetAdditionalLightsCount();
    for (int i = 0; i < additionalLightsCount; ++i)
    {
        // GetMainLight()�Ǝ��Ă��܂����Afor���[�v�̃C���f�b�N�X�����܂��B
        // ����́A�I�u�W�F�N�g���Ƃ̃��C�g�C���f�b�N�X���v�Z���A���C�g�o�b�t�@��K�؂ɃT���v�����O����Light�\���̂����������܂��B
        // ADDITIONAL_LIGHT_CALCULATE_SHADOWS����`����Ă���ꍇ�A�e���v�Z����܂��B
        int perObjectLightIndex = GetPerObjectLightIndex(i);
        Light light = GetAdditionalPerObjectLight(perObjectLightIndex, lightingData.positionWS); // ����positionWS�����C�e�B���O�Ɏg�p����B
        // �I�t�Z�b�g���ꂽpositionWS���e�e�X�g�Ɏg�p����B
        // �u�I�t�Z�b�g�v����邱�ƂŁA���̈ʒu�����炳��邱�Ƃ��Ӗ����܂��B
        light.shadowAttenuation = AdditionalLightRealtimeShadow(perObjectLightIndex, shadowTestPosWS); 

        // �ǉ��̃��C�g���V�F�[�f�B���O���邽�߂Ɏg�p�����قȂ�֐�
        additionalLightSumResult += ShadeSingleLight(surfaceData, lightingData, light, true);
    }
#endif
    //==============================================================================================

    // emission
    half3 emissionResult = ShadeEmission(surfaceData, lightingData);

    return CompositeAllLightResults(indirectResult, mainLightResult, additionalLightSumResult, emissionResult, surfaceData, lightingData);
}

half3 ConvertSurfaceColorToOutlineColor(half3 originalSurfaceColor)
{
    return originalSurfaceColor * _OutlineColor;
}
half3 ApplyFog(half3 color, Varyings input)
{
    half fogFactor = input.positionWSAndFogFactor.w;
    // �s�N�Z���J���[��fogColor�ƍ������܂��B�K�v�ɉ����āAMixFogColor���g�p����fogColor���J�X�^���J���[�ŏ㏑�����邱�Ƃ��ł��܂��B
    color = MixFog(color, fogFactor);

    return color;
}

// �t���O�����g�V�F�[�_�{��
// ���̊֐���.shader�t�@�C���̂݌Ăяo����܂� 
// #pragma fragment ShadeFinalColor�i.shader���̂���ɂ��j
half4 ShadeFinalColor(Varyings input) : SV_TARGET
{
    //////////////////////////////////////////////////////////////////////////////////////////
    // �܂��̓��C�e�B���O�@�\�ׂ̈ɑS�Ẵf�[�^��p�ӂ���B
    //////////////////////////////////////////////////////////////////////////////////////////

    // ToonSurfaceData�\���̂𖄂߂�
    ToonSurfaceData surfaceData = InitializeSurfaceData(input);

    // ToonLightingData�\���̂𖄂߂�
    ToonLightingData lightingData = InitializeLightingData(input);

    // �S�Ẵ��C�e�B���O�v�Z��K�p����
    half3 color = ShadeAllLights(surfaceData, lightingData);

#ifdef ToonShaderIsOutline
    color = ConvertSurfaceColorToOutlineColor(color);
#endif

    color = ApplyFog(color, input);

    return half4(color, surfaceData.alpha);
}

//////////////////////////////////////////////////////////////////////////////////////////
// �t���O�����g���L�֐��iShadowCaster pass & DepthOnly pass�ł̂ݎg�p�j
//////////////////////////////////////////////////////////////////////////////////////////
void BaseColorAlphaClipTest(Varyings input)
{
    DoClipTestToTargetAlphaValue(GetFinalBaseColor(input).a);
}
