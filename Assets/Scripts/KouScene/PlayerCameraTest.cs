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
        // targetの移動量分、自分（カメラ）も移動する
        transform.position += targetObj.transform.position - targetPos;
        targetPos = targetObj.transform.position;

        // targetの位置のY軸を中心に、回転（公転）する
        transform.RotateAround(targetPos, Vector3.up, _inputHorizontal * Time.deltaTime * 150f);
        // カメラの垂直移動（※角度制限なし、必要が無ければコメントアウト）
        transform.RotateAround(targetPos, transform.right, _inputVertical * Time.deltaTime * 150f);
    }

    public void SetMovement(Vector3 direction, float sensitivity)
    {
        _inputHorizontal = direction.x * sensitivity;
        _inputVertical = direction.z * sensitivity;
    }
}
