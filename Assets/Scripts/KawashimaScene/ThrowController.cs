using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowController : MonoBehaviour
{
    // �e��Prefab
    [SerializeField, Tooltip("�e��Prefab")]
    private GameObject _bulletPrefab;

    // �C�g�̃I�u�W�F�N�g
    [SerializeField, Tooltip("�C�g�ɂ���I�u�W�F�N�g")]
    private GameObject _barrelObject;

    // �e�𐶐�����ʒu���
    private Vector3 _instantiatePosition;

    // �e�̐������W(�ǂݎ�点���p)
    public Vector3 copyInstantiatePosition
    { get { return _instantiatePosition; } }

    // �e�𔭎˂��鑬�x
    [SerializeField, Range(1.0F, 20.0F), Tooltip("�e�𔭎˂��鑬�x")]
    private float _throwItemSpeed = 10f;

    // �e�̏����x
    private Vector3 _shootVelocity;

    // �e�̏����x(�ǂݎ�点���p)
    public Vector3 copyShootVelocity
    { get { return _shootVelocity; } }

    void Update()
    {
        //����UpdateState�������Ɠ������u�ԂɋO�������\�������
        //Public�֐��ɂ��ĉE�N���b�N�𒷉������Ă���Ԃ����\�������悤�ɂ��������ǂ��H����
        UpdateState();
    }

    private void UpdateState()
    {
        // �e�̏����x���X�V
        _shootVelocity = _barrelObject.transform.up * _throwItemSpeed;
        // �e�̐������W���X�V
        _instantiatePosition = _barrelObject.transform.position;
    }

    public void ShootItem()
    {
        UpdateState();

        // �e�𐶐����Ĕ�΂�
        GameObject obj = Instantiate(_bulletPrefab, _instantiatePosition, Quaternion.identity);
        Rigidbody rid = obj.GetComponent<Rigidbody>();
        rid.AddForce(_shootVelocity * rid.mass, ForceMode.Impulse);

        // 5�b��ɏ�����
        Destroy(obj, 5.0F);
    }
}
