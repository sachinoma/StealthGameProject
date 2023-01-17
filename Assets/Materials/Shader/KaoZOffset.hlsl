#ifndef Include_KaoZOffset
#define Include_KaoZOffset

// �r���[�X�y�[�X�i���j�A�A�r���[�X�y�[�X�P�ʁj�ŃJ�����Ɍ������ĉ��z���_���v�b�V�����A
// ���̌�ɁA���z���_�̌��ʂ�positionCS.z�l���g�p���āA����positionCS.z���̂ݏ㏑�����܂��B
// ����́A���_�V�F�[�_��ZTest ZWrite�̐[�x�l�ɂ̂݉e�����܂��B

// �p�r:
// -��/�ڂɌ����ڂ������A�E�g���C�����B��
// -���т𔯂̏�ɕ`�悷��
// -�W�I���g�����ړ�������ZFighting������������
float4 KaoGetNewClipPosWithZOffset(float4 originalPositionCS, float viewSpaceZOffsetAmount)
{
    if (unity_OrthoParams.w == 0)
    {
        ////////////////////////////////
        //Perspective camera case
        ////////////////////////////////
        float2 ProjM_ZRow_ZW = UNITY_MATRIX_P[2].zw;
        float modifiedPositionVS_Z = -originalPositionCS.w + -viewSpaceZOffsetAmount; // ���z���_���v�b�V������
        float modifiedPositionCS_Z = modifiedPositionVS_Z * ProjM_ZRow_ZW[0] + ProjM_ZRow_ZW[1];
        originalPositionCS.z = modifiedPositionCS_Z * originalPositionCS.w / (-modifiedPositionVS_Z); // positionCS.z���㏑������
        return originalPositionCS;
    }
    else
    {
        ////////////////////////////////
        //Orthographic camera case
        ////////////////////////////////
        originalPositionCS.z += -viewSpaceZOffsetAmount / _ProjectionParams.z; // ���z���_���v�b�V������,������positionCS.z���㏑������
        return originalPositionCS;
    }
}

#endif

