using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawArc : MonoBehaviour
{
    // �������̕`��ON/OFF
    private bool _drawArc = true;

    // ���������\����������̐�
    [SerializeField, Range(10, 100),Tooltip("���������\����������̐�")]
    private int _segmentCnt = 60;

    // �����������b���v�Z���邩
    [SerializeField, Range(0.5f, 6.0f), Tooltip("�����������b���v�Z���邩")]
    private float _predictionTime = 6.0f;

    // ��������Material
    [SerializeField, Tooltip("�������̃}�e���A��")]
    private Material _arcMaterial;

    // �������̕�
    [SerializeField, Tooltip("�������̕�")]
    private float _arcWidth = 0.02f;

    // ���������\������LineRenderer
    private LineRenderer[] _lineRenderers;

    // �e�̏����x�␶�����W�����R���|�[�l���g
    private ThrowController _shootBullet;

    // �e�̏����x
    private Vector3 _initialVelocity;

    // �������̊J�n���W
    private Vector3 _arcStartPosition;

    void Start()
    {
        // ��������LineRenderer�I�u�W�F�N�g��p��
        CreateLineRendererObjects();

        // �e�̏����x�␶�����W�����X�N���v�g
        _shootBullet = gameObject.GetComponent<ThrowController>();
    }

    void Update()
    {
        // �����x�ƕ������̊J�n���W���X�V
        _initialVelocity = _shootBullet.copyShootVelocity;
        _arcStartPosition = _shootBullet.copyInstantiatePosition;

        if(_drawArc)
        {
            // ��������\��
            float timeStep = _predictionTime / _segmentCnt;
            bool draw = false;
            float hitTime = float.MaxValue;
            for(int i = 0; i < _segmentCnt; i++)
            {
                // ���̍��W���X�V
                float startTime = timeStep * i;
                float endTime = startTime + timeStep;
                SetLineRendererPosition(i, startTime, endTime, !draw);

                // �Փ˔���
                if(!draw)
                {
                    hitTime = GetArcHitTime(startTime, endTime);
                    if(hitTime != float.MaxValue)
                    {
                        draw = true; // �Փ˂����炻�̐�̕������͕\�����Ȃ�
                    }
                }
            }
        }
        else
        {
            // �������ƃ}�[�J�[��\�����Ȃ�
            for(int i = 0; i < _lineRenderers.Length; i++)
            {
                _lineRenderers[i].enabled = false;
            }
        }
    }

    // �w�莞�Ԃɑ΂���A�[�`�̕�������̍��W��Ԃ�
    private Vector3 GetArcPositionAtTime(float time)
    {
        return (_arcStartPosition + ((_initialVelocity * time) + (0.5f * time * time) * Physics.gravity));
    }

    // LineRenderer�̍��W���X�V
    private void SetLineRendererPosition(int index, float startTime, float endTime, bool draw = true)
    {
        _lineRenderers[index].SetPosition(0, GetArcPositionAtTime(startTime));
        _lineRenderers[index].SetPosition(1, GetArcPositionAtTime(endTime));
        _lineRenderers[index].enabled = draw;
    }

    // LineRenderer�I�u�W�F�N�g���쐬
    private void CreateLineRendererObjects()
    {
        // �e�I�u�W�F�N�g�����ALineRenderer�����q�I�u�W�F�N�g�����
        GameObject arcObjectsParent = new GameObject("ArcObject");

        _lineRenderers = new LineRenderer[_segmentCnt];
        for(int i = 0; i < _segmentCnt; i++)
        {
            GameObject newObject = new GameObject("LineRenderer_" + i);
            newObject.transform.SetParent(arcObjectsParent.transform);
            _lineRenderers[i] = newObject.AddComponent<LineRenderer>();

            // �����֘A���g�p���Ȃ�
            _lineRenderers[i].receiveShadows = false;
            _lineRenderers[i].reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            _lineRenderers[i].lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            _lineRenderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            // ���̕��ƃ}�e���A��
            _lineRenderers[i].material = _arcMaterial;
            _lineRenderers[i].startWidth = _arcWidth;
            _lineRenderers[i].endWidth = _arcWidth;
            _lineRenderers[i].numCapVertices = 5;
            _lineRenderers[i].enabled = false;
        }
    }

    // 2�_�Ԃ̐����ŏՓ˔��肵�A�Փ˂��鎞�Ԃ�Ԃ�
    private float GetArcHitTime(float startTime, float endTime)
    {
        // Linecast��������̎n�I�_�̍��W
        Vector3 startPosition = GetArcPositionAtTime(startTime);
        Vector3 endPosition = GetArcPositionAtTime(endTime);

        // �Փ˔���
        RaycastHit hitInfo;
        if(Physics.Linecast(startPosition, endPosition, out hitInfo))
        {
            // �Փ˂���Collider�܂ł̋���������ۂ̏Փˎ��Ԃ��Z�o
            float distance = Vector3.Distance(startPosition, endPosition);
            return startTime + (endTime - startTime) * (hitInfo.distance / distance);
        }
        return float.MaxValue;
    }
}
