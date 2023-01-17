// ���̃t�@�C���́A�قȂ�Ɩ���������ҏW���Ď������邽�߂ɗp�ӂ���Ă��܂��B
// �����ɍD���ȃR�[�h��ǉ���ҏW���Ă�������

// #pragma once�́A�قƂ�ǂ�.hlsl�ōŗǂ̃v���N�e�B�X�ł���iUnity2020�ȏオ�K�v�ł��j�A
// ������s�����ƂŁA.hlsl�̃��[�U�[�����ł��ǂ��ł�����.hlsl���܂߂邱�Ƃ��ł��A�}���`�C���N���[�h�R���t���N�g�𐶂��邱�ƂȂ��m���ɂȂ�܂�
#pragma once

half3 ShadeGI(ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    // ���ׂĂ̏ڍ�SH�𖳎����āi�萔SH���݂̂��c�����ƂŁj3D�����B���B
    // �������͒P�ɕ��ϓI�Ȋ��ԐڐF�������~����
    half3 averageSH = SampleSH(0);

    // ���C�g�v���[�u��Bake����Ă��Ȃ��ꍇ�ɁA���ʂ����S�ɍ����Ȃ�̂�h�����Ƃ��ł��܂�
    averageSH = max(_IndirectLightMinColor, averageSH);

    // �ǁi�Ԑڌ��ōő�50���Â����āA���ʂ����S�ɍ����Ȃ�̂�h���j
    half indirectOcclusion = lerp(1, surfaceData.occlusion, 0.5);
    return averageSH * indirectOcclusion;
}

// �ł��d�v�ȕ����F���C�e�B���O�������A���g���ɍ��킹�ĕҏW�A�����Ŏ��R�ɏ�������ł��������A�n���I�ɁI
// ���̊֐��́A���ׂĂ̒��ڌ��i������/�_/�X�|�b�g�j�Ŏg�p����܂��B
half3 ShadeSingleLight(ToonSurfaceData surfaceData, ToonLightingData lightingData, Light light, bool isAdditionalLight)
{
    half3 N = lightingData.normalWS;
    half3 L = light.direction;

    half NoL = dot(N, L);

    half lightAttenuation = 1;

    // �_�����ƃX�|�b�g���C�g�̋����Ɗp�x�t�F�[�h�iLighting.hlsl����GetAdditionalPerObjectLight(...)���Q�Ɓj
    // Lighting.hlsl -> https://github.com/Unity-Technologies/Graphics/blob/master/Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl
    half distanceAttenuation = min(4, light.distanceAttenuation); //clamp to prevent light over bright if point/spot light too close to vertex

    // N dot L
    // �ł��P����1�s�Z���V�F�[�h�A��ɂ��̍s��Ǝ��̕��@�Œu�������邱�Ƃ��ł��܂��I
    half litOrShadowArea = smoothstep(_CelShadeMidPoint - _CelShadeSoftness, _CelShadeMidPoint + _CelShadeSoftness, NoL);

    // occlusion
    litOrShadowArea *= surfaceData.occlusion;

    // ��͒ʏ�NoL���\�b�h���g�p����Ɣ��ɏX���̂ŁA�Z���V�F�[�h�𖳎�����
    litOrShadowArea = _IsFace ? lerp(0.5, 1, litOrShadowArea) : litOrShadowArea;

    // light's shadow map
    litOrShadowArea *= lerp(1, light.shadowAttenuation, _ReceiveShadowMappingAmount);

    half3 litOrShadowColor = lerp(_ShadowMapColor, 1, litOrShadowArea);

    half3 lightAttenuationRGB = litOrShadowColor * distanceAttenuation;

    // light.color��saturate()���āA�ߏ�ɖ��邭�Ȃ�Ȃ��悤�ɂ��A�ǉ��̌��͉��Z�����̂ŋ��x�����炷
    return saturate(light.color) * lightAttenuationRGB * (isAdditionalLight ? 0.25 : 1);
}

half3 ShadeEmission(ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    half3 emissionResult = lerp(surfaceData.emission, surfaceData.emission * surfaceData.albedo, _EmissionMulByBaseColor); // optional mul albedo
    return emissionResult;
}

half3 CompositeAllLightResults(half3 indirectResult, half3 mainLightResult, half3 additionalLightSumResult, half3 emissionResult, ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    // [�����ɂ͉��ł������܂��A����͒P���ȃ`���[�g���A�����\�b�h�ł�] 
    // �����ł́A�����ߏ�ɖ��邭�Ȃ�Ȃ��悤�ɂ��܂����A�܂����̐F�̒��F��ۂ��Ƃ�]�݂܂�

    half3 rawLightSum = max(indirectResult, mainLightResult + additionalLightSumResult); // �Ԑڌ��ƒ��ڌ��̂����A�ł��������̂�I������
    return surfaceData.albedo * rawLightSum + emissionResult;
}
