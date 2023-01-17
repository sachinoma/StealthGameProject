#ifndef Include_KaoOutlineUtil
#define Include_KaoOutlineUtil

// �����v���W�F�N�g�ɁA�V�F�[�_���ŃJ������fov���擾�����荂���ȕ��@������ꍇ�A���̒x���֐���u�������邱�Ƃ��ł��܂��B
// �Ⴆ�΁A�V����RendererFeature���g�p���āAC����cmd.SetGlobalFloat�i"_CurrentCameraFOV"�AcameraFOV�j���������Ƃ��ł��܂��B
// ���͒x�����֗��ȕ��@���g�p���āA�J������fov���擾���邱�ƂŁA�����̂��Ƃ�P�������܂��B
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
    //�J�����̎��E���ŃL�����N�^�[������������ꍇ�A�A�E�g���C�����u�t�F�[�h�A�E�g�v������
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

        // ���ׂẴJ���������ɓn���āA��ʏ�ŃA�E�g���C���̕��𓯂��������܂܂ɂ���    
        cameraMulFix = abs(positionVS_Z);

        // �X���[�Y�Ȓ�~���K�v�ȏꍇ�́A�g�[���}�b�v�֐��ɒu�������邱�Ƃ��ł��܂�
        cameraMulFix = ApplyOutlineDistanceFadeOut(cameraMulFix);

        // ���ׂẴJ����fov�ɓn���āA��ʏ�ŃA�E�g���C���̕��𓯂��������܂܂ɂ���
        cameraMulFix *= GetCameraFOV();
    }
    else
    {
        ////////////////////////////////
        // Orthographic camera case
        ////////////////////////////////
        float orthoSize = abs(unity_OrthoParams.y);
        orthoSize = ApplyOutlineDistanceFadeOut(orthoSize);
        cameraMulFix = orthoSize * 50; // 50�́A�������e�J�����̃A�E�g���C��������v�����邽�߂̃}�W�b�N�i���o�[�ł�
    }

    return cameraMulFix * 0.00005; // �萔����Z���āA���ʂ��f�t�H���g�̖@���W�J��WS�ɂ���
}
#endif

