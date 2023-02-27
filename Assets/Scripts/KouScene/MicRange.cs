using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;

public class MicRange : MonoBehaviour
{
    [SerializeField] private bool _isMicMode = true;
    public bool IsMicMode => _isMicMode;

    [SerializeField] private float rollBackTime = 1.0f;
    private float rollBackTimer;
    //マイク機能
    private readonly int _sampleNum = (2 << 9); // サンプリング数は2のN乗(N=5-12)
    [Header("音量に掛ける倍率")]
    [SerializeField, Range(0f, 3000f)] float m_gain = 1500f; // 倍率
    AudioSource m_source;
    float[] _currentValues;
    [SerializeField] private float _volumeRate;

    [Header("音量チェックできる最小値")]
    [SerializeField, Range(0f, 1.0f)] private float _minRate = 0.2f;
    [Header("黒いFogが戻る速さ")]
    [SerializeField, Range(0f, 2.0f)] private float _rollBackSpeed = 0.5f;


    [Header("Fogの値(0.01~0.5) 少ないの方が遠い")]
    //[SerializeField, Range(0.01f, 0.5f)] private float _FogRange = 0.05f;
    [SerializeField] private float[] _fogRange;
    private float _fogRangeChoose;
    [Header("Fogの最大範囲(0.5~10.0)")]
    [SerializeField, Range(0.5f, 10.0f)] private float _maxRange = 1.0f;
    [Header("Fogの最小範囲(0.5~2.0)")]
    [SerializeField, Range(0.5f, 2.0f)] private float _minRange = 1.0f;

    //黒いFog
    private float _fogMax = 0.18f;
    private float _fogMin = 0.01f;

    //黒い丸コライダーとメッシュ
    [Header("SoundFogRangeの拡大倍率(1.5~3.0)")]
    [SerializeField, Range(1.5f, 3.0f)] private float _circleMaxRate = 2.0f;
    private float _circleMin = 1.0f;

    //丸コライダー（Enemyから検出用）
    [Header("Enemyから検出用コライダーの最大範囲(0.5~10.0)")]
    [SerializeField, Range(0.5f, 10.0f)] private float _maxRangeForEnemy = 3.0f;
    [SerializeField] private GameObject _circleForEnemyMain;
    [SerializeField] private GameObject _circleForEnemy;
    [Header("敵から検知できるコライダーの大きさ(3.0~8.0)")]
    [SerializeField, Range(0.5f, 8.0f)] private float _circleForEnemyScaleValue = 5.0f;
    private Vector3 _circleForEnemyScale;


    [SerializeField] private GameObject _circleBlack;
    private Vector3 _circleScaleBasic = new Vector3(1, 1, 1);
    private float _circleValue = 1.0f;
    private float _circleBlackValue = 1.0f;

    //ポストエフェクト
    [SerializeField] private GameObject[] _rangeParticle;
    [SerializeField] private GameObject _frontRangeParticlePos;
    [SerializeField] private GameObject _centerRangeParticlePos;

    void Start()
    {
        MicStart();
        RenderSettings.fogDensity = _fogMax / _minRange;
        _circleForEnemyScale = new Vector3(_circleForEnemyScaleValue, _circleForEnemyScaleValue, _circleForEnemyScaleValue);
    }

    void Update()
    {
        if(_isMicMode)
        {
            MicUpdate();
        }
        else
        {
            ButtonUpdate();
        }   

        if(VolumeCheck())
        {
            //FogSetting(_fogMax / _minRange, _fogMin / _maxRange);
            FogSetting(_fogRangeChoose);
            CircleBlackSetting(_circleMaxRate * _maxRange, _circleMin * _minRange);
            CircleSetting(_circleMaxRate * _maxRangeForEnemy, _circleMin * _minRange);
            FogRollBack(_fogMax / _minRange, _fogMin / _maxRange);
            CircleBlackRollBack(_circleBlack, _circleMaxRate * _maxRange, _circleMin * _minRange);
            CircleRollBack(_circleForEnemyMain, _circleMaxRate * _maxRangeForEnemy, _circleMin * _minRange);
            rollBackTimer = rollBackTime;
        }
        if(rollBackTimer <= 0)
        {
            FogRollBack(_fogMax / _minRange, _fogMin / _maxRange);
            CircleBlackRollBack(_circleBlack, _circleMaxRate * _maxRange, _circleMin * _minRange);
        }
        
        CircleRollBack(_circleForEnemyMain, _circleMaxRate * _maxRangeForEnemy, _circleMin * _minRange);

        rollBackTimer -= 1.0f * Time.deltaTime;
    }

    void FixedUpdate()
    {
        
    }

    public void SwitchMicMode()
    {
        _isMicMode = !_isMicMode;
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
    }

    //音量チェック
    private bool VolumeCheck()
    {
        return _volumeRate > _minRate;
    }

    #region 黒いFog  
    public void FogSetting(float num)
    {
        //RenderSettings.fogDensity = FogValue(_volumeRate, max, min);   
        RenderSettings.fogDensity = FogValueNumSet(num);
    }
    /*
    public void FogSetting(float max, float min)
    {
        RenderSettings.fogDensity = FogValue(_volumeRate, max, min);   
    }
    */

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

    float FogValueNumSet(float num)
    {
        float _tmpValue = num;
        if(RenderSettings.fogDensity > _tmpValue)
        {
            return _tmpValue;
        }
        return RenderSettings.fogDensity;
    }

    public void FogRollBack(float max, float min)
    {
        const float _fogSpeedSet = 1.5f;  

        if(RenderSettings.fogDensity < max)
        {
            RenderSettings.fogDensity += ((max - min)/ _fogSpeedSet) * _rollBackSpeed * Time.deltaTime;            
        }
        else
        {
            RenderSettings.fogDensity = max;           
        }
    }
    #endregion

    #region 丸コライダーとメッシュ
    public void CircleBlackSetting(float max, float min)
    {
        _circleBlackValue = CircleBlackValue(_volumeRate, max, min);
    }
    public void CircleSetting(float max, float min)
    {
        _circleValue = CircleValue(_volumeRate, max, min);
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
    float CircleValue(float num, float max, float min)
    {
        float _tmpValue = min + (num * (max - min));
        if(_circleValue < _tmpValue)
        {
            return _tmpValue;
        }
        return _circleValue;
    }

    public void CircleBlackRollBack(GameObject circle, float max, float min)
    {
        const float _circleSpeedSet = 1.4f;

        if(_circleBlackValue > min)
        {
            _circleBlackValue -= ((max - min) / _circleSpeedSet) * _rollBackSpeed * Time.deltaTime;
        }
        else
        {
            _circleBlackValue = min;
        }
        circle.transform.localScale = _circleBlackValue * _circleScaleBasic;
    }

    public void CircleRollBack(GameObject circle, float max, float min)
    {
        const float _circleSpeedSet = 1.4f;

        if(_circleValue > min)
        {
            _circleValue -= ((max - min) / _circleSpeedSet) * _rollBackSpeed * Time.deltaTime;
            _circleForEnemy.transform.localScale = _circleForEnemyScale;
            _circleForEnemy.SetActive(true);
        }
        else
        {
            _circleValue = min;
            _circleForEnemy.SetActive(false);
        }
        circle.transform.localScale = _circleValue * _circleScaleBasic;
    }
    #endregion

    #region ボタン型音量調整
    public void ButtonUpdate()
    {
        _volumeRate -= 0.05f;
    }

    public void SetVolumeRate(float rate)
    {
        _volumeRate = rate;
    }



    #endregion

    #region ボタン型音量調整

    public void AnimSetVolumeRate(float rate)
    {
        _volumeRate = rate;
        if(rate >= 1.0f)
        {
            Instantiate(_rangeParticle[1], _frontRangeParticlePos.transform.position, transform.rotation);
            _fogRangeChoose = _fogRange[1];
        }
        else if(rate >= 0.7f)
        {
            Instantiate(_rangeParticle[0], _frontRangeParticlePos.transform.position, transform.rotation);
            _fogRangeChoose = _fogRange[0];
        }
        else
        {
            Instantiate(_rangeParticle[2], _centerRangeParticlePos.transform.position, transform.rotation);
            _fogRangeChoose = _fogRange[2];
        }       
    }
    #endregion
}
