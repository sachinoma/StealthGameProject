using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeVisualChanger: MonoBehaviour
{
    [SerializeField]
    private GameObject _player;
    [Header("透過開始、透過完全解除の距離設定")]
    [SerializeField, Range(1f, 20f), Tooltip("透過解除を開始する距離")]
    private float _startDistance = 7f;
    [SerializeField, Range(1f, 5f), Tooltip("透過が完全に解除される距離")]
    private float _endDistance = 2f;

    void Update()
    {
        //プレイヤーに設定したものとこのアイテムとの距離をキャッシュ
        var distance = Vector3.Distance(_player.transform.position, transform.position);

        //
        var color = this.GetComponent<Renderer>().material.color;

        if (distance > _startDistance)
            color.a = 0f;
        else if (distance > _endDistance)
            color.a = (_startDistance - distance) / (_startDistance - _endDistance);
        else
            color.a = 1f;
        this.GetComponent<Renderer>().material.color = color;
    }
}
