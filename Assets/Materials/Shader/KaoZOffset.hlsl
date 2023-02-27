#ifndef Include_KaoZOffset
#define Include_KaoZOffset

// ビュースペース（リニア、ビュースペース単位）でカメラに向かって仮想頂点をプッシュし、
// その後に、仮想頂点の結果のpositionCS.z値を使用して、元のpositionCS.zをのみ上書きします。
// これは、頂点シェーダのZTest ZWriteの深度値にのみ影響します。

// 用途:
// -顔/目に見た目が悪いアウトラインを隠す
// -眉毛を髪の上に描画する
// -ジオメトリを移動せずにZFighting問題を解決する
float4 KaoGetNewClipPosWithZOffset(float4 originalPositionCS, float viewSpaceZOffsetAmount)
{
    if (unity_OrthoParams.w == 0)
    {
        ////////////////////////////////
        //Perspective camera case
        ////////////////////////////////
        float2 ProjM_ZRow_ZW = UNITY_MATRIX_P[2].zw;
        float modifiedPositionVS_Z = -originalPositionCS.w + -viewSpaceZOffsetAmount; // 仮想頂点をプッシュする
        float modifiedPositionCS_Z = modifiedPositionVS_Z * ProjM_ZRow_ZW[0] + ProjM_ZRow_ZW[1];
        originalPositionCS.z = modifiedPositionCS_Z * originalPositionCS.w / (-modifiedPositionVS_Z); // positionCS.zを上書きする
        return originalPositionCS;
    }
    else
    {
        ////////////////////////////////
        //Orthographic camera case
        ////////////////////////////////
        originalPositionCS.z += -viewSpaceZOffsetAmount / _ProjectionParams.z; // 仮想頂点をプッシュする,そしてpositionCS.zを上書きする
        return originalPositionCS;
    }
}

#endif

