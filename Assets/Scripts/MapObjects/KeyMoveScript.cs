using UnityEngine;

public class KeyMoveScript : MonoBehaviour
{
    [Header("動作確認時のみ切り替え：移動切り替え")]
    [SerializeField]
    private bool _isMoving = false;

    private Transform _startTransform;
    [Header("移動速度")]
    [SerializeField]
    private float _defMoveSpeed = 0.7f;

    [Header("回転スイッチ")]
    [SerializeField]
    private bool _isRot = false;
    [Header("回転速度")]
    [SerializeField]
    private float _rotSpeed = 20f;

    [SerializeField]
    private float Grav = 0.05f;
    [SerializeField]
    private float _speed;

    public void Moving(bool onoff)
    { _isMoving = onoff; }

    public void Rotate(bool onoff)
    { _isRot = onoff; }

    void Start()
    {
        _startTransform = transform;
        _speed = _defMoveSpeed;
    }


    private void PositionReset()
    {
        transform.position = _startTransform.position;
    }

    private void RotReset()
    {
        transform.rotation = _startTransform.rotation;
    }

    void Update()
    {
        if(_isMoving)
        {
            transform.position += _speed * Vector3.up * Time.deltaTime;

            _speed += Grav * Time.deltaTime;

            if(_defMoveSpeed <= Mathf.Abs(_speed))
            {
                Grav = -Grav;
                _speed = Mathf.Sign(_speed) * _defMoveSpeed;
            }
        }
        else
            PositionReset();

        if(_isRot)
        {
            transform.Rotate(Vector3.up * _rotSpeed * Time.deltaTime);
        }
        else
            RotReset();
    }
}