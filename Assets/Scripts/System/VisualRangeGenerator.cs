using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualRangeGenerator : MonoBehaviour
{
    // Singletonパターン
    public static VisualRangeGenerator Instance { get; private set; }

    // TODO : Dictionaryで違うテンプレートを格納する？
    // TODO : Resources / addressableで動的にテンプレートを読み込む？
    // TODO : 頻繁に生成すれば、Object Poolを使う？
    [Header("Visual Range")]
    [SerializeField] private SoundVisualRange _soundVisualRangeTemplate;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Debug.LogWarning("すでに他のVisualRangeGenerator Instanceがある");
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if(Instance != null && Instance == this)
        {
            Instance = null;
        }
    }

    public void Generate(Vector3 worldPos, float radius, float time = 1.0f)
    {
        if (_soundVisualRangeTemplate == null)
        {
            Debug.LogWarning("_soundVisualRangeTemplateなしで、生成できない。");
            return;
        }

        SoundVisualRange visualRange = Instantiate(_soundVisualRangeTemplate);
        visualRange.Show(worldPos, radius, time);
    }
}
