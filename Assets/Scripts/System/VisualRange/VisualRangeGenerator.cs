using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualRangeGenerator : SceneSingleton<VisualRangeGenerator>
{
    // TODO : Dictionaryで違うテンプレートを格納する？
    // TODO : Resources / addressableで動的にテンプレートを読み込む？
    // TODO : 頻繁に生成すれば、Object Poolを使う？
    [Header("Visual Range")]
    [SerializeField] private SoundVisualRange _soundVisualRangeTemplate;

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
