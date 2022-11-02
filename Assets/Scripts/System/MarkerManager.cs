using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO : 今Markは一個だけ表示する
public class MarkerManager : MonoBehaviour
{
    // Singletonパターン
    public static MarkerManager Instance { get; private set; }

    // TODO : Dictionaryで違うテンプレートを格納する？
    // TODO : Resources / addressableで動的にテンプレートを読み込む？
    [Header("Marker")]
    [SerializeField] private Marker _markerTemplate;
    [SerializeField] private Transform _markerBaseTransform;

    private Marker _marker = null;

    #region Singleton

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Debug.LogWarning("すでに他のMarkerManager Instanceがある");
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

    #endregion

    public void SetMarker(Transform target)
    {
        if (_marker == null)
        {
            if(_markerTemplate == null)
            {
                Debug.LogWarning("_markerTemplateなしで、生成できない。");
                return;
            }

            _marker = Instantiate(_markerTemplate, _markerBaseTransform);
        }

        _marker.SetTarget(target);
    }

    public void CancelMarker()
    {
        _marker.SetTarget(null);
    }
}
