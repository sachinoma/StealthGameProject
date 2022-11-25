using Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    private Camera _controllingCamera;

    private void Awake()
    {
        // メインカメラを探すまたは作成
        _controllingCamera = Camera.main;

        if(_controllingCamera == null)
        {
            GameObject go = new GameObject();
            go.name = "Main Camera";
            _controllingCamera = go.AddComponent<Camera>();
            _controllingCamera.tag = "MainCamera";
            go.AddComponent<AudioListener>();
        }

        _controllingCamera.gameObject.AddComponent<CinemachineBrain>();

        _controllingCamera.transform.SetParent(transform);  // 用途なし。Unity Editorで見やすいようにのみ。
    }

    public void Init(Transform target)
    {
        _virtualCamera.Follow = target;

        // target の後ろにいる
        CinemachinePOV pov = _virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        pov.m_HorizontalAxis.Value = target.rotation.eulerAngles.y;
    }
}
