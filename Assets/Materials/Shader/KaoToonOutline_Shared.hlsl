// 複数回のinclude conflict防止(.hlsl)
#pragma once

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
// For Litシェーダー用
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

// 使わない
//#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
// 以下のhlslで代用
#include "KaoOutlineUtil.hlsl"
#include "KaoZOffset.hlsl"
#include "KaoInvLerpRemap.hlsl"

// 頂点シェーダへの入力定義
struct Attributes
{
    float3 positionOS   : POSITION;
    half3 normalOS      : NORMAL;
    half4 tangentOS     : TANGENT;
    float2 uv           : TEXCOORD0;
};

// フラグメントシェーダへ渡すデータ
struct Varyings
{
    float2 uv                       : TEXCOORD0;
    float4 positionWSAndFogFactor   : TEXCOORD1; // xyz: positionWS, w: vertex fog factor
    half3 normalWS                  : TEXCOORD2;
    float4 positionCS               : SV_POSITION;
};

// sampler2DはCBUFFERに入れない
sampler2D _BaseMap;
sampler2D _EmissionMap;
sampler2D _OcclusionMap;
sampler2D _OutlineZOffsetMaskTex;

// SRP batchingを使えるように、必要な物が全部この一つのCBUFFERに入ります(重要)
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

// applyShadowBiasFixToHClipPos()用の特殊な変数、per material uniformではないのでCBUFFERの外で書いても大丈夫。
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
// 頂点シェーダの処理と機能
//-----------------------------

float3 TransformPositionWSToOutlinePositionWS(float3 positionWS, float positionVS_Z, float3 normalWS)
{
    //あなた自身の方法に置き換えることができます！ ここでは、チュートリアルの理由で簡単なワールドスペースメソッドを書きますが、これは最良の方法ではありません！
    float outlineExpandAmount = _OutlineWidth * GetOutlineCameraFovAndDistanceFixMultiplier(positionVS_Z);
    return positionWS + normalWS * outlineExpandAmount;
}

// ToonShaderIsOutline not defined = 通常の座標変換をする。
// ToonShaderIsOutline defined     = 通常の座標変換をする + 頂点をnormalの方向に少し押し出す。
Varyings VertexShaderWork(Attributes input)
{
    Varyings output;

    // VertexPositionInputs は新しい座標変換機能
    // リンク：https://tips.hecomi.com/entry/2019/10/27/152520
    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS);

    // VertexNormalInputs はnormal、tangent、bittangentを含まれています(world space)
    VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

    float3 positionWS = vertexInput.positionWS;

    // アウトラインの為に頂点をnormalの方向に少し押し出す(world space)
#ifdef ToonShaderIsOutline
    positionWS = TransformPositionWSToOutlinePositionWS(vertexInput.positionWS, vertexInput.positionVS.z, vertexNormalInput.normalWS);
#endif

    // 頂点ごとにComputeFogFactorを実行(昔のUNITY_TRANSFER_FOG())
    float fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

    // TRANSFORM_TEXは同じ
    output.uv = TRANSFORM_TEX(input.uv, _BaseMap);

    // positionWS(xyz)とfog(w)を一つのvector4にする。
    output.positionWSAndFogFactor = float4(positionWS, fogFactor);

    // GetVertexNormalInputsの時は既にnormlaized済みです。
    output.normalWS = vertexNormalInput.normalWS;

    output.positionCS = TransformWorldToHClip(positionWS);

#ifdef ToonShaderIsOutline
    // [Read ZOffset mask texture]
    // 頂点シェーダーはtex2D()を使えないので(まだラスタライズしてない、ddxとddyは未知)
    // だからtex2Dlod()を使う。explict mip level 0を四つ目のparam uvに置く
    float outlineZOffsetMaskTexExplictMipLevel = 0;
    float outlineZOffsetMask = tex2Dlod(_OutlineZOffsetMaskTex, float4(input.uv, 0, outlineZOffsetMaskTexExplictMipLevel)).r; //ここは黒白テクスチャ

    // [Remap ZOffset texture value]
    // texture read valueを反転させ(black = apply ZOffset)、通常のoutline mask textureはこのフォーマットを使ってる(black = hide outline)
    outlineZOffsetMask = 1 - outlineZOffsetMask;
    outlineZOffsetMask = invLerpClamp(_OutlineZOffsetMaskRemapStart, _OutlineZOffsetMaskRemapEnd, outlineZOffsetMask);// ユーザーが値を反転させたり、remapすることを許可する

    // [ZOffsetを適用し、リマップされた値をZOffsetマスクとして使用する]
    output.positionCS = KaoGetNewClipPosWithZOffset(output.positionCS, _OutlineZOffset * outlineZOffsetMask + 0.03 * _IsFace);
#endif

    // ShadowCasterパスはpositionCSに特殊な処理が必要、でなければshadow artifactが起こる
    //-----------------------------
#ifdef ToonShaderApplyShadowBiasFix
    // URP/Shaders/ShadowCasterPass.hlslのGetShadowPositionHClip()
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
// フラグメントシェーダの機能 (ステップ1：ライティング計算の為に構造体を準備)
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
    DoClipTestToTargetAlphaValue(output.alpha);//可能であればearly exit

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
    lightingData.normalWS = normalize(input.normalWS); //補間されたnormalは単位ベクトルではない、normalizeする必要がある。

    return lightingData;
}

//-----------------------------
// フラグメントシェーダの機能 (ステップ2：ライティングとfinal colorの計算)
//-----------------------------

// ライティングに関する全部の式はこのhlslに載ってる。
// このhlslを編集すると、大多数のビジュアルの結果を変えることができます。
#include "KaoToonOutline_LightingEquation.hlsl"

// この関数はライティングのロジックが含まれていません。ただビジュアルの結果を色んな所に渡すだけ。
// この関数の仕事は「影マッピングの深度テストpositionWSオフセットをする」
// 影マッピングでは、光源から視点までの距離を表す深度値を、あらかじめ計算しておいて、それを用いて影を表現することができます。
// 深度テストpositionWSオフセットは、この深度値をどのように計算するかを決めるもので、そのために使われるものです。
half3 ShadeAllLights(ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    // 間接照明
    half3 indirectResult = ShadeGI(surfaceData, lightingData);

    //////////////////////////////////////////////////////////////////////////////////
    // URPが抽象的な光の為に提供したライトの構造体は
    // 以下の情報が含まれています。
    // - direction           方向
    // - color               色
    // - distanceAttenuation 距離減衰
    // - shadowAttenuation   影の減衰
    //
    // URPはライトとプラットフォームに応じて、違うシェーディングアプローチをする。
    // シェーディングアプローチ ＝＞ 光や影をどのように表現するかを決める方法を指します。
    // ライト シェーダ変数を直接参照する事はダメです。
    // 代わりに
    // -GetMainLight()
    // -GetLight()
    // の関数を使ってライトの構造体を埋める
    //////////////////////////////////////////////////////////////////////////////////

    //==============================================================================================
    // メインライトは最も明るい方向性の光です。
    // ライトループの外でシェーディングされ、特定の変数セットとシェーディングパスを持っているので、唯一の方向性光がある場合にはできるだけ高速に処理できます。
    // 必要に応じて、shadowCoordを渡すことができます。その場合、shadowAttenuationが計算されます。
    Light mainLight = GetMainLight();

    float3 shadowTestPosWS = lightingData.positionWS + mainLight.direction * (_ReceiveShadowMappingPosOffset + _IsFace);
#ifdef _MAIN_LIGHT_SHADOWS
    // フラグメントシェーダの中にshadowCoordを計算するには、今こういう変わりがあります。
    // https://forum.unity.com/threads/shadow-cascades-weird-since-7-2-0.828453/#post-5516425

    // _ReceiveShadowMappingPosOffsetは、影の比較位置のオフセットを制御します。
    // これは、通常、顔のような影に敏感なエリアで醜いセルフシャドウを隠すために行われます。
    float4 shadowCoord = TransformWorldToShadowCoord(shadowTestPosWS);
    mainLight.shadowAttenuation = MainLightRealtimeShadow(shadowCoord);
#endif 

    // Main light
    half3 mainLightResult = ShadeSingleLight(surfaceData, lightingData, mainLight, false);

    //==============================================================================================
    // 他の追加ライト

    half3 additionalLightSumResult = 0;

#ifdef _ADDITIONAL_LIGHTS
    // レンダリングされているオブジェクトに影響を与えるライトの数値を返します。
    // これらのライトは、URPのフォワードレンダラーでオブジェクトごとにカリングされます。.
    int additionalLightsCount = GetAdditionalLightsCount();
    for (int i = 0; i < additionalLightsCount; ++i)
    {
        // GetMainLight()と似ていますが、forループのインデックスを取ります。
        // これは、オブジェクトごとのライトインデックスを計算し、ライトバッファを適切にサンプリングしてLight構造体を初期化します。
        // ADDITIONAL_LIGHT_CALCULATE_SHADOWSが定義されている場合、影も計算されます。
        int perObjectLightIndex = GetPerObjectLightIndex(i);
        Light light = GetAdditionalPerObjectLight(perObjectLightIndex, lightingData.positionWS); // 元のpositionWSをライティングに使用する。
        // オフセットされたpositionWSを影テストに使用する。
        // 「オフセット」されることで、その位置がずらされることを意味します。
        light.shadowAttenuation = AdditionalLightRealtimeShadow(perObjectLightIndex, shadowTestPosWS); 

        // 追加のライトをシェーディングするために使用される異なる関数
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
    // ピクセルカラーをfogColorと混合します。必要に応じて、MixFogColorを使用してfogColorをカスタムカラーで上書きすることができます。
    color = MixFog(color, fogFactor);

    return color;
}

// フラグメントシェーダ本体
// この関数は.shaderファイルのみ呼び出されます 
// #pragma fragment ShadeFinalColor（.shader中のこれにより）
half4 ShadeFinalColor(Varyings input) : SV_TARGET
{
    //////////////////////////////////////////////////////////////////////////////////////////
    // まずはライティング機能の為に全てのデータを用意する。
    //////////////////////////////////////////////////////////////////////////////////////////

    // ToonSurfaceData構造体を埋める
    ToonSurfaceData surfaceData = InitializeSurfaceData(input);

    // ToonLightingData構造体を埋める
    ToonLightingData lightingData = InitializeLightingData(input);

    // 全てのライティング計算を適用する
    half3 color = ShadeAllLights(surfaceData, lightingData);

#ifdef ToonShaderIsOutline
    color = ConvertSurfaceColorToOutlineColor(color);
#endif

    color = ApplyFog(color, input);

    return half4(color, surfaceData.alpha);
}

//////////////////////////////////////////////////////////////////////////////////////////
// フラグメント共有関数（ShadowCaster pass & DepthOnly passでのみ使用）
//////////////////////////////////////////////////////////////////////////////////////////
void BaseColorAlphaClipTest(Varyings input)
{
    DoClipTestToTargetAlphaValue(GetFinalBaseColor(input).a);
}
