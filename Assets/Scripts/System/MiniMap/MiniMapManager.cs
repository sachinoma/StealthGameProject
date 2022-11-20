using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO : 動的に MiniMapCamera と MiniMap UI を生成する？
public class MiniMapManager : SceneSingleton<MiniMapManager>
{
    [SerializeField] private MiniMapCamera _camera;
    [SerializeField] private GameObject _miniMap;

    [SerializeField] private bool _isMiniMapRotate = true;

    private bool _isOn = false;
    private bool _hasFocus = false;

    private void Start()
    {
        // 初期化
        // _hasFocus == true の場合、すでに SetFocus() から初期化した
        if(!_hasFocus)
        {
            Switch(_isOn);
        }
    }

    public void SetFocus(Transform focus, bool isSwitchOn = true)
    {
        _hasFocus = _camera.SetFocus(focus, _isMiniMapRotate);
        Switch(isSwitchOn && _hasFocus);
    }

    public void SwitchOnOff()
    {
        if(_hasFocus)
        {
            Switch(!_isOn);
        }
        else
        {
            Debug.LogWarning("フォーカスなし。MiniMapの表示がスイッチできない。");
        }
    }

    private void Switch(bool isOn)
    {
        _camera.gameObject.SetActive(isOn);
        _miniMap.SetActive(isOn);

        _isOn = isOn;
    }
}
