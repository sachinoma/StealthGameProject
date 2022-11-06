using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawArc : MonoBehaviour
{
    // 放物線の描画ON/OFF
    private bool _drawArc = true;

    // 放物線を構成する線分の数
    [SerializeField, Range(10, 100),Tooltip("放物線を構成する線分の数")]
    private int _segmentCnt = 60;

    // 放物線を何秒分計算するか
    [SerializeField, Range(0.5f, 6.0f), Tooltip("放物線を何秒分計算するか")]
    private float _predictionTime = 6.0f;

    // 放物線のMaterial
    [SerializeField, Tooltip("放物線のマテリアル")]
    private Material _arcMaterial;

    // 放物線の幅
    [SerializeField, Tooltip("放物線の幅")]
    private float _arcWidth = 0.02f;

    // 放物線を構成するLineRenderer
    private LineRenderer[] _lineRenderers;

    // 弾の初速度や生成座標を持つコンポーネント
    private ThrowController _shootBullet;

    // 弾の初速度
    private Vector3 _initialVelocity;

    // 放物線の開始座標
    private Vector3 _arcStartPosition;

    void Start()
    {
        // 放物線のLineRendererオブジェクトを用意
        CreateLineRendererObjects();

        // 弾の初速度や生成座標を持つスクリプト
        _shootBullet = gameObject.GetComponent<ThrowController>();
    }

    void Update()
    {
        // 初速度と放物線の開始座標を更新
        _initialVelocity = _shootBullet.copyShootVelocity;
        _arcStartPosition = _shootBullet.copyInstantiatePosition;

        if(_drawArc)
        {
            // 放物線を表示
            float timeStep = _predictionTime / _segmentCnt;
            bool draw = false;
            float hitTime = float.MaxValue;
            for(int i = 0; i < _segmentCnt; i++)
            {
                // 線の座標を更新
                float startTime = timeStep * i;
                float endTime = startTime + timeStep;
                SetLineRendererPosition(i, startTime, endTime, !draw);

                // 衝突判定
                if(!draw)
                {
                    hitTime = GetArcHitTime(startTime, endTime);
                    if(hitTime != float.MaxValue)
                    {
                        draw = true; // 衝突したらその先の放物線は表示しない
                    }
                }
            }
        }
        else
        {
            // 放物線とマーカーを表示しない
            for(int i = 0; i < _lineRenderers.Length; i++)
            {
                _lineRenderers[i].enabled = false;
            }
        }
    }

    // 指定時間に対するアーチの放物線上の座標を返す
    private Vector3 GetArcPositionAtTime(float time)
    {
        return (_arcStartPosition + ((_initialVelocity * time) + (0.5f * time * time) * Physics.gravity));
    }

    // LineRendererの座標を更新
    private void SetLineRendererPosition(int index, float startTime, float endTime, bool draw = true)
    {
        _lineRenderers[index].SetPosition(0, GetArcPositionAtTime(startTime));
        _lineRenderers[index].SetPosition(1, GetArcPositionAtTime(endTime));
        _lineRenderers[index].enabled = draw;
    }

    // LineRendererオブジェクトを作成
    private void CreateLineRendererObjects()
    {
        // 親オブジェクトを作り、LineRendererを持つ子オブジェクトを作る
        GameObject arcObjectsParent = new GameObject("ArcObject");

        _lineRenderers = new LineRenderer[_segmentCnt];
        for(int i = 0; i < _segmentCnt; i++)
        {
            GameObject newObject = new GameObject("LineRenderer_" + i);
            newObject.transform.SetParent(arcObjectsParent.transform);
            _lineRenderers[i] = newObject.AddComponent<LineRenderer>();

            // 光源関連を使用しない
            _lineRenderers[i].receiveShadows = false;
            _lineRenderers[i].reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            _lineRenderers[i].lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            _lineRenderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            // 線の幅とマテリアル
            _lineRenderers[i].material = _arcMaterial;
            _lineRenderers[i].startWidth = _arcWidth;
            _lineRenderers[i].endWidth = _arcWidth;
            _lineRenderers[i].numCapVertices = 5;
            _lineRenderers[i].enabled = false;
        }
    }

    // 2点間の線分で衝突判定し、衝突する時間を返す
    private float GetArcHitTime(float startTime, float endTime)
    {
        // Linecastする線分の始終点の座標
        Vector3 startPosition = GetArcPositionAtTime(startTime);
        Vector3 endPosition = GetArcPositionAtTime(endTime);

        // 衝突判定
        RaycastHit hitInfo;
        if(Physics.Linecast(startPosition, endPosition, out hitInfo))
        {
            // 衝突したColliderまでの距離から実際の衝突時間を算出
            float distance = Vector3.Distance(startPosition, endPosition);
            return startTime + (endTime - startTime) * (hitInfo.distance / distance);
        }
        return float.MaxValue;
    }
}
