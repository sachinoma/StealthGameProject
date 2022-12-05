using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;

public class MicRange : MonoBehaviour
{
    [SerializeField] private bool isMicMode = true;

    //マイク機能
    private readonly int _sampleNum = (2 << 9); // サンプリング数は2のN乗(N=5-12)
    [Header("音量に掛ける倍率")]
    [SerializeField, Range(0f, 3000f)] float m_gain = 1500f; // 倍率
    AudioSource m_source;
    float[] _currentValues;
    float _volumeRate;

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
    [Header("SoundFogRangeの拡大倍率(1.5~3.0)")]
    [SerializeField, Range(1.5f, 3.0f)] private float _circleBlackMax = 2.0f;
    private float _circleBlackMin = 1.0f;

    //丸コライダー（Enemyから検出用）
    [SerializeField] private GameObject _circleForEnemy;
    [Header("敵から検知できるコライダーの大きさ(3.0~8.0)")]
    [SerializeField, Range(3.0f, 8.0f)] private float _circleForEnemyScaleValue = 5.0f;
    private Vector3 _circleForEnemyScale;


    [SerializeField] private GameObject _circleBlack;
    private Vector3 _circleBlackBasic = new Vector3(1, 1, 1);
    private float _circleBlackValue = 1.0f;

    //ボタン型音量調整


    void Start()
    {
        MicStart();
        RenderSettings.fogDensity = _fogMax / _minRange;
        _circleForEnemyScale = new Vector3(_circleForEnemyScaleValue, _circleForEnemyScaleValue, _circleForEnemyScaleValue);
    }

    void Update()
    {
        if(isMicMode)
        {
            MicUpdate();
        }
        else
        {
            ButtonUpdate();
        }   

        if(VolumeCheck())
        {
            FogSetting(_fogMax / _minRange, _fogMin / _maxRange);
            CircleBlackSetting(_circleBlackMax * _maxRange, _circleBlackMin * _minRange);
        }
        FogRollBack(_fogMax / _minRange, _fogMin / _maxRange);
        CircleBlackRollBack(_circleBlackMax * _maxRange, _circleBlackMin * _minRange);
    }

    void FixedUpdate()
    {
        
    }

    //マイク設定
    public void MicStart()
    {
        m_source = GetComponent<AudioSource>();
        _currentValues = new float[_sampleNum];
        if((m_source != null) && (Microphone.devices.Length > 0)) // オーディオソースとマイクがある
        {
            string devName = Microphone.devices[0]; // 複数見つかってもとりあえず0番目のマイクを使用
            int minFreq, maxFreq;
            Microphone.GetDeviceCaps(devName, out minFreq, out maxFreq); // 最大最小サンプリング数を得る
            int ms = minFreq / _sampleNum; // サンプリング時間を適切に取る
            m_source.loop = true; // ループにする
            m_source.clip = Microphone.Start(devName, true, ms, minFreq); // clipをマイクに設定
            while(!(Microphone.GetPosition(devName) > 0)) { } // きちんと値をとるために待つ
            Microphone.GetPosition(null);
            m_source.Play();
        }
    }

    public void MicUpdate()
    {
        m_source.GetSpectrumData(_currentValues, 0, FFTWindow.Hamming);
        float sum = 0f;
        for(int i = 0; i < _currentValues.Length; ++i)
        {
            sum += _currentValues[i]; // データ（周波数帯ごとのパワー）を足す
        }
        // データ数で割ったものに倍率をかけて音量とする
        _volumeRate = Mathf.Clamp01(sum * m_gain / (float)_currentValues.Length);
        Debug.Log(_volumeRate);
    }

    //音量チェック
    private bool VolumeCheck()
    {
        return _volumeRate > _minRate;
    }

    #region 黒いFog  
    public void FogSetting(float max, float min)
    {
        RenderSettings.fogDensity = FogValue(_volumeRate, max, min);
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
        const float _fogSpeedSet = 1.5f;

        _circleForEnemy.transform.localScale = new Vector3(0, 0, 0);

        if(RenderSettings.fogDensity < max)
        {
            RenderSettings.fogDensity += ((max - min)/ _fogSpeedSet) * _rollBackSpeed * Time.deltaTime;
            _circleForEnemy.transform.localScale = _circleForEnemyScale;

            //RenderSettings.fogDensity += 0.001f * _rollBackSpeed;
        }
    }
    #endregion

    #region 黒い丸コライダーとメッシュ
    public void CircleBlackSetting(float max, float min)
    {
        _circleBlackValue = CircleBlackValue(_volumeRate, max, min);
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
        const float _circleBlackSpeedSet = 1.4f;

        if(_circleBlackValue > min)
        {
            _circleBlackValue -= ((max - min) / _circleBlackSpeedSet) * _rollBackSpeed * Time.deltaTime;
            //_circleBlackValue -= 0.025f * _rollBackSpeed;
        }
        _circleBlack.transform.localScale = _circleBlackValue * _circleBlackBasic;
    }
    #endregion

    #region ボタン型音量調整
    public void ButtonUpdate()
    {
        _volumeRate = 1.0f;
    }
    #endregion
}
