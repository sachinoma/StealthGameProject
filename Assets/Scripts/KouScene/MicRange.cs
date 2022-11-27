using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;

public class MicRange : MonoBehaviour
{
    [Header("音量に掛ける倍率")]
    [SerializeField, Range(0f, 20f)] private float m_gain = 1f; // 音量に掛ける倍率
    private float m_volumeRate; // 音量(0-1)

    [Header("音量チェックできる最小値")]
    [SerializeField, Range(0f, 1.0f)] private float _minRate = 0.2f;
    [Header("黒いFogが戻る速さ")]
    [SerializeField, Range(0f, 2.0f)] private float _rollBackSpeed = 0.5f;

    [Header("Fogの最大範囲(0.5~2.0)")]
    [SerializeField, Range(0.5f, 2.0f)] private float _maxRange = 1.0f;
    [Header("Fogの最小範囲(0.5~2.0)")]
    [SerializeField, Range(0.5f, 2.0f)] private float _minRange = 1.0f;


    //黒いFog
    private float _fogMax = 0.18f;
    private float _fogMin = 0.03f;

    //黒い丸コライダーとメッシュ
    private float _circleBlackMax = 4.5f;
    private float _circleBlackMin = 1.0f;

    [SerializeField] private GameObject _circleBlack;
    private Vector3 _circleBlackBasic = new Vector3(1, 1, 1);
    private float _circleBlackValue = 1.0f;

    void Start()
    {
        AudioSource aud = GetComponent<AudioSource>();
        if((aud != null) && (Microphone.devices.Length > 0)) // オーディオソースとマイクがある
        {
            string devName = Microphone.devices[0]; // 複数見つかってもとりあえず0番目のマイクを使用
            int minFreq, maxFreq;
            Microphone.GetDeviceCaps(devName, out minFreq, out maxFreq); // 最大最小サンプリング数を得る
            aud.clip = Microphone.Start(devName, true, 2, minFreq); // 音の大きさを取るだけなので最小サンプリングで十分
            aud.Play(); //マイクをオーディオソースとして実行(Play)開始
        }
        RenderSettings.fogDensity = _fogMax / _minRange;
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if(VolumeCheck())
        {
            FogSetting(_fogMax/ _minRange, _fogMin/ _maxRange);
            CircleBlackSetting(_circleBlackMax* _maxRange, _circleBlackMin* _minRange);
        }

        FogRollBack(_fogMax / _minRange, _fogMin / _maxRange);
        CircleBlackRollBack(_circleBlackMax * _maxRange, _circleBlackMin * _minRange);
    }

    //音量チェック
    private bool VolumeCheck()
    {
        return m_volumeRate > _minRate;
    }

    #region 黒いFog  
    public void FogSetting(float max, float min)
    {
        RenderSettings.fogDensity = FogValue(m_volumeRate, max, min);
    }

    // 黒いFogの増減、maxは一番黒い,minは一番遠い
    float FogValue(float num , float max , float min)
    {
        float _tmpValue = max + (num * -(max - min));
        if(RenderSettings.fogDensity > _tmpValue)
        {
            return _tmpValue;
        }
        return RenderSettings.fogDensity;
    }

    public void FogRollBack(float max, float min)
    {
        const float _fogSpeedSet = 150.0f;

        if(RenderSettings.fogDensity < max)
        {
            RenderSettings.fogDensity += ((max - min)/ _fogSpeedSet) * _rollBackSpeed;
            //RenderSettings.fogDensity += 0.001f * _rollBackSpeed;
        }
    }
    #endregion

    #region 黒い丸コライダーとメッシュ
    public void CircleBlackSetting(float max, float min)
    {
        _circleBlackValue = CircleBlackValue(m_volumeRate, max, min);
    }
    // 黒い丸コライダーとメッシュの増減、maxは一番遠い,minは一番近い
    // FogValueと真逆
    float CircleBlackValue(float num, float max, float min)
    {
        float _tmpValue = min + (num * (max - min));
        if(_circleBlackValue < _tmpValue)
        {
            return _tmpValue;
        }
        return _circleBlackValue;
    }

    public void CircleBlackRollBack(float max, float min)
    {
        const float _circleBlackSpeedSet = 140.0f;

        if(_circleBlackValue > min)
        {
            _circleBlackValue -= ((max - min) / _circleBlackSpeedSet) * _rollBackSpeed;
            //_circleBlackValue -= 0.025f * _rollBackSpeed;
        }
        _circleBlack.transform.localScale = _circleBlackValue * _circleBlackBasic;
    }
    #endregion

    // オーディオが読まれるたびに実行される
    private void OnAudioFilterRead(float[] data, int channels)
    {
        float sum = 0f;
        for(int i = 0; i < data.Length; ++i)
        {
            sum += Mathf.Abs(data[i]); // データ（波形）の絶対値を足す
        }
        // データ数で割ったものに倍率をかけて音量とする
        m_volumeRate = Mathf.Clamp01(sum * m_gain / (float)data.Length);
        if(m_volumeRate < 0.001f) { m_volumeRate = 0; }
    }
}
