// https://github.com/ronja-tutorials/ShaderTutorials/blob/master/Assets/047_InverseInterpolationAndRemap/Interpolation.cginc
// �œK���̂��߂�float��half�ɕҏW���܂��B����͐F�f�[�^�ihalf�j���������邽�߂Ɏg�p���邩��ł�

#ifndef Include_KaoInvLerpRemap
#define Include_KaoInvLerpRemap

// smoothstep()�̂悤�ł����A���`�ł��Aclamped�ł͂Ȃ��B
half invLerp(half from, half to, half value)
{
    return (value - from) / (to - from);
}
half invLerpClamp(half from, half to, half value)
{
    return saturate(invLerp(from, to, value));
}
// ���}�b�v�����S�ɐ���ł��܂����A���x���ł�
half remap(half origFrom, half origTo, half targetFrom, half targetTo, half value)
{
    half rel = invLerp(origFrom, origTo, value);
    return lerp(targetFrom, targetTo, rel);
}
#endif
