using System.Collections.Generic;
using UnityEngine;

public class RangeVisualChanger: MonoBehaviour
{
    private GameObject _player;

    [Header("透過開始、透過解除の距離設定")]
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

    [Header("サウンド近いほうから")]
    [SerializeField] private List<AudioClip> audioClips = new List<AudioClip>();
    [SerializeField] private float minDistance = 3f;
    [SerializeField] private float middleDistance = 5f;
    [SerializeField] private float maxDistance = 7f;

    private AudioSource audioSource;

    private float _seconds;

    private void OnEnable()
    {
        _player = GameObject.FindWithTag(Tag.Player);
        if(_player == null)
        {
            gameObject.SetActive(false);
        }

        //オーディオのキャッシュ
        audioSource = this.GetComponent<AudioSource>();
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
            
        //距離に応じてオーディオの再生速度を変化
        AudioPitchChenge(distance);

        //変更した色を実際に入れる
        this.GetComponent<Renderer>().material.color = color;
    }

    private void AudioPitchChenge(float dist)
    {
        int num = 0;
        var currentClip = audioSource.clip;
        if(dist < minDistance)
        {
            num = 0;
        }
        else if(dist < middleDistance)
        {
            num = 1;
        }
        else if (dist < maxDistance)
        {
            num = 2;
        }
        else
        {
            return;
        }
        audioSource.clip = audioClips[num];
        if(currentClip != audioSource.clip) { audioSource.Play(); }
    }

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

    private float RangeChangeAlpha(float dist, float color)
    {
        if(dist > _startDistance)
            color = 0f;
        else if(dist > _endDistance)
            color = (_startDistance - dist) / (_startDistance - _endDistance);
        else
            color = 1f;
        
        return color;
    }

}
