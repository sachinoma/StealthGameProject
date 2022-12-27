using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowController : MonoBehaviour
{
    // 弾のPrefab
    [SerializeField, Tooltip("弾のPrefab")]
    private GameObject _bulletPrefab;

    // 砲身のオブジェクト
    [SerializeField, Tooltip("砲身にするオブジェクト")]
    private GameObject _barrelObject;

    // 弾を生成する位置情報
    private Vector3 _instantiatePosition;

    // 弾の生成座標(読み取らせる専用)
    public Vector3 copyInstantiatePosition
    { get { return _instantiatePosition; } }

    // 弾を発射する速度
    [SerializeField, Range(1.0F, 20.0F), Tooltip("弾を発射する速度")]
    private float _throwItemSpeed = 10f;

    // 弾の初速度
    private Vector3 _shootVelocity;

    // 弾の初速度(読み取らせる専用)
    public Vector3 copyShootVelocity
    { get { return _shootVelocity; } }

    void Update()
    {
        //このUpdateStateを消すと投げた瞬間に軌道線が表示される
        //Public関数にして右クリックを長押ししている間だけ表示されるようにした方が良い？かも
        UpdateState();
    }

    private void UpdateState()
    {
        // 弾の初速度を更新
        _shootVelocity = _barrelObject.transform.up * _throwItemSpeed;
        // 弾の生成座標を更新
        _instantiatePosition = _barrelObject.transform.position;
    }

    public void ShootItem()
    {
        UpdateState();

        // 弾を生成して飛ばす
        GameObject obj = Instantiate(_bulletPrefab, _instantiatePosition, Quaternion.identity);
        Rigidbody rid = obj.GetComponent<Rigidbody>();
        rid.AddForce(_shootVelocity * rid.mass, ForceMode.Impulse);

        // 5秒後に消える
        Destroy(obj, 5.0F);
    }
}
