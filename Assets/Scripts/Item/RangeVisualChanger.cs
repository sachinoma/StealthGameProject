using UnityEngine;

public class RangeVisualChanger: MonoBehaviour
{
    private GameObject _player;

    /*
     * Rangeの下限は0又は0.001fになった方がいいかもしれない
     * 例えば、_startDisance = 0.5fをしたい場合もある
     */
    [Header ("透過開始、透過解除の距離設定")]
    [SerializeField, Range(1f, 20f), Tooltip("透過解除を開始する距離")]
    private float _startDistance = 7f;
    [SerializeField, Range(1f, 5f), Tooltip("透過が完全に解除される距離")]
    private float _endDistance = 2f;

    [Header("距離で透過、距離＆時間経過で透過を切り替え")]
    [SerializeField]
    private bool _isTimeChange = false;
    [Header("透過解除にかかる時間")]
    [SerializeField, Range(0.1f, 20f)]
    private float _releaseTime = 3f;

    private float _seconds;

    private void OnEnable()
    {
        _player = GameObject.FindWithTag(Tag.Player);
        if(_player == null)
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        //プレイヤーに設定したものとこのアイテムとの距離をキャッシュ
        var distance = Vector3.Distance(_player.transform.position, transform.position);

        //現在の色(α値をとるために)をキャッシュ
        var color = this.GetComponent<Renderer>().material.color;

        if (_isTimeChange)
            color.a = TimeChangeAlpha(distance, color.a);
        else
            color.a = RangeChangeAlpha(distance, color.a);
            
        //変更した色を実際に入れる
        this.GetComponent<Renderer>().material.color = color;
    }

    /*
     * 関数の引数colorは役に立たない
     * private float TimeChangeAlpha(float dist)
     * {
     *     float alpha = 0f;
     *     ...
     *     
     *     return alpha;
     * }
     */
    private float TimeChangeAlpha(float dist, float color)
    {
        if(dist > _startDistance)
        {
            _seconds = 0f;
            color = 0f;
        }
        else if(_seconds < _releaseTime)
        {
            _seconds += Time.deltaTime;
            color = _seconds / _releaseTime;
        }
        else
            color = 1f;

        return color;
    }

    /*
     * 関数の引数colorは役に立たない
     */
    private float RangeChangeAlpha(float dist, float color)
    {
        /*
         * UnityはInverseLerp関数を用意した。
         * でも、Mathfだけみたいだ。
         * Vector2とかVector3とかQuaternionとかInverseLerp関数がない
        if(dist > _startDistance)
            color = 0f;
        else if(dist > _endDistance)
            color = (_startDistance - dist) / (_startDistance - _endDistance);
        else
            color = 1f;
        */
        color = Mathf.InverseLerp(_startDistance, _endDistance, dist);

        return color;
    }

}
