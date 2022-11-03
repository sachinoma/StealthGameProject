using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerManager : SceneSingleton<MarkerManager>
{
    // TODO : Dictionaryで違うテンプレートを格納する？
    // TODO : Resources / addressableで動的にテンプレートを読み込む？
    // TODO : 頻繁に生成すれば、Object Poolを使う？
    [Header("Marker")]
    [SerializeField] private Marker _markerTemplate;
    [SerializeField] private Transform _markerBaseTransform;

    private Dictionary<Transform, Marker> _markerDict = new Dictionary<Transform, Marker>();

    public void SetMarker(Transform target)
    {
        if(_markerDict.ContainsKey(target))
        {
            Debug.Log($"target : {target.name} すでに Marker が付かせた。");
            _markerDict[target].SetIsOn(true);
            return;
        }

        if(_markerTemplate == null)
        {
            Debug.LogWarning("_markerTemplate なしで、生成できない。");
            return;
        }

        Marker marker = Instantiate(_markerTemplate, _markerBaseTransform);
        marker.SetTarget(target);
        _markerDict.Add(target, marker);
    }

    public void CancelMarker(Transform target)
    {
        if(!_markerDict.ContainsKey(target))
        {
            Debug.Log($"target : {target.name} は Marker が付かせなかった。");
            return;
        }

        _markerDict[target].SetIsOn(false);
    }
}
