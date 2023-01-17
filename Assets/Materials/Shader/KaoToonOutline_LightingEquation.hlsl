// このファイルは、異なる照明方程式を編集して実験するために用意されています。
// ここに好きなコードを追加や編集してください

// #pragma onceは、ほとんどの.hlslで最良のプラクティスであり（Unity2020以上が必要です）、
// これを行うことで、.hlslのユーザーがいつでもどこでもこの.hlslを含めることができ、マルチインクルードコンフリクトを生じることなく確実になります
#pragma once

half3 ShadeGI(ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    // すべての詳細SHを無視して（定数SH項のみを残すことで）3D感を隠す。
    // 私たちは単に平均的な環境間接色だけが欲しい
    half3 averageSH = SampleSH(0);

    // ライトプローブがBakeされていない場合に、結果が完全に黒くなるのを防ぐことができます
    averageSH = max(_IndirectLightMinColor, averageSH);

    // 閉塞（間接光で最大50％暗くして、結果が完全に黒くなるのを防ぐ）
    half indirectOcclusion = lerp(1, surfaceData.occlusion, 0.5);
    return averageSH * indirectOcclusion;
}

// 最も重要な部分：ライティング方程式、お使いに合わせて編集、ここで自由に書き込んでください、創造的に！
// この関数は、すべての直接光（方向性/点/スポット）で使用されます。
half3 ShadeSingleLight(ToonSurfaceData surfaceData, ToonLightingData lightingData, Light light, bool isAdditionalLight)
{
    half3 N = lightingData.normalWS;
    half3 L = light.direction;

    half NoL = dot(N, L);

    half lightAttenuation = 1;

    // 点光源とスポットライトの距離と角度フェード（Lighting.hlsl内のGetAdditionalPerObjectLight(...)を参照）
    // Lighting.hlsl -> https://github.com/Unity-Technologies/Graphics/blob/master/Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl
    half distanceAttenuation = min(4, light.distanceAttenuation); //clamp to prevent light over bright if point/spot light too close to vertex

    // N dot L
    // 最も単純な1行セルシェード、常にこの行を独自の方法で置き換えることができます！
    half litOrShadowArea = smoothstep(_CelShadeMidPoint - _CelShadeSoftness, _CelShadeMidPoint + _CelShadeSoftness, NoL);

    // occlusion
    litOrShadowArea *= surfaceData.occlusion;

    // 顔は通常NoLメソッドを使用すると非常に醜いので、セルシェードを無視する
    litOrShadowArea = _IsFace ? lerp(0.5, 1, litOrShadowArea) : litOrShadowArea;

    // light's shadow map
    litOrShadowArea *= lerp(1, light.shadowAttenuation, _ReceiveShadowMappingAmount);

    half3 litOrShadowColor = lerp(_ShadowMapColor, 1, litOrShadowArea);

    half3 lightAttenuationRGB = litOrShadowColor * distanceAttenuation;

    // light.colorをsaturate()して、過剰に明るくならないようにし、追加の光は加算されるので強度を減らす
    return saturate(light.color) * lightAttenuationRGB * (isAdditionalLight ? 0.25 : 1);
}

half3 ShadeEmission(ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    half3 emissionResult = lerp(surfaceData.emission, surfaceData.emission * surfaceData.albedo, _EmissionMulByBaseColor); // optional mul albedo
    return emissionResult;
}

half3 CompositeAllLightResults(half3 indirectResult, half3 mainLightResult, half3 additionalLightSumResult, half3 emissionResult, ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    // [ここには何でも書けます、これは単純なチュートリアルメソッドです] 
    // ここでは、光が過剰に明るくならないようにしますが、まだ光の色の着色を保つことを望みます

    half3 rawLightSum = max(indirectResult, mainLightResult + additionalLightSumResult); // 間接光と直接光のうち、最も高いものを選択する
    return surfaceData.albedo * rawLightSum + emissionResult;
}
