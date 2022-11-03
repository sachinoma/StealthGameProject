using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraTest : MonoBehaviour
{
    [SerializeField]
    GameObject targetObj;
    Vector3 targetPos;
    private float _inputHorizontal;
    private float _inputVertical;

    void Start()
    {
        //targetObj = GameObject.Find("TargetGameObject");
        targetPos = targetObj.transform.position;
    }

    void Update()
    {
        // target�̈ړ��ʕ��A�����i�J�����j���ړ�����
        transform.position += targetObj.transform.position - targetPos;
        targetPos = targetObj.transform.position;

        // target�̈ʒu��Y���𒆐S�ɁA��]�i���]�j����
        transform.RotateAround(targetPos, Vector3.up, _inputHorizontal * Time.deltaTime * 150f);
        // �J�����̐����ړ��i���p�x�����Ȃ��A�K�v��������΃R�����g�A�E�g�j
        transform.RotateAround(targetPos, transform.right, _inputVertical * Time.deltaTime * 150f);
    }

    public void SetMovement(Vector3 direction, float sensitivity)
    {
        _inputHorizontal = direction.x * sensitivity;
        _inputVertical = direction.z * sensitivity;
    }
}
