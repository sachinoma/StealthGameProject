// https://github.com/ronja-tutorials/ShaderTutorials/blob/master/Assets/047_InverseInterpolationAndRemap/Interpolation.cginc
// 最適化のためにfloatをhalfに編集します。これは色データ（half）を処理するために使用するからです

#ifndef Include_KaoInvLerpRemap
#define Include_KaoInvLerpRemap

// smoothstep()のようですが、線形です、clampedではない。
half invLerp(half from, half to, half value)
{
    return (value - from) / (to - from);
}
half invLerpClamp(half from, half to, half value)
{
    return saturate(invLerp(from, to, value));
}
// リマップを完全に制御できますが、より遅いです
half remap(half origFrom, half origTo, half targetFrom, half targetTo, half value)
{
    half rel = invLerp(origFrom, origTo, value);
    return lerp(targetFrom, targetTo, rel);
}
#endif
