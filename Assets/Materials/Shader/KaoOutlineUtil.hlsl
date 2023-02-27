#ifndef Include_KaoOutlineUtil
#define Include_KaoOutlineUtil

// もしプロジェクトに、シェーダ内でカメラのfovを取得するより高速な方法がある場合、この遅い関数を置き換えることができます。
// 例えば、新しいRendererFeatureを使用して、C＃でcmd.SetGlobalFloat（"_CurrentCameraFOV"、cameraFOV）を書くことができます。
// 今は遅いが便利な方法を使用して、カメラのfovを取得することで、これらのことを単純化します。
float GetCameraFOV()
{
    //https://answers.unity.com/questions/770838/how-can-i-extract-the-fov-information-from-the-pro.html
    float t = unity_CameraProjection._m11;
    float Rad2Deg = 180 / 3.1415;
    float fov = atan(1.0f / t) * 2.0 * Rad2Deg;
    return fov;
}
float ApplyOutlineDistanceFadeOut(float inputMulFix)
{
    //カメラの視界内でキャラクターが小さすぎる場合、アウトラインを「フェードアウト」させる
    return saturate(inputMulFix);
}
float GetOutlineCameraFovAndDistanceFixMultiplier(float positionVS_Z)
{
    float cameraMulFix;
    if (unity_OrthoParams.w == 0)
    {
        ////////////////////////////////
        // Perspective camera case
        ////////////////////////////////

        // すべてのカメラ距離に渡って、画面上でアウトラインの幅を同じくしたままにする    
        cameraMulFix = abs(positionVS_Z);

        // スムーズな停止が必要な場合は、トーンマップ関数に置き換えることができます
        cameraMulFix = ApplyOutlineDistanceFadeOut(cameraMulFix);

        // すべてのカメラfovに渡って、画面上でアウトラインの幅を同じくしたままにする
        cameraMulFix *= GetCameraFOV();
    }
    else
    {
        ////////////////////////////////
        // Orthographic camera case
        ////////////////////////////////
        float orthoSize = abs(unity_OrthoParams.y);
        orthoSize = ApplyOutlineDistanceFadeOut(orthoSize);
        cameraMulFix = orthoSize * 50; // 50は、透視投影カメラのアウトライン幅を一致させるためのマジックナンバーです
    }

    return cameraMulFix * 0.00005; // 定数を乗算して、結果をデフォルトの法線展開量WSにする
}
#endif

