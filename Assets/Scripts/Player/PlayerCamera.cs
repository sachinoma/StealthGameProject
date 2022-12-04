using Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    public Camera Cam { get; private set; }

    private CinemachinePOV _pov;

    private void Awake()
    {
        // メインカメラを探すまたは作成
        Cam = Camera.main;

        if(Cam == null)
        {
            GameObject go = new GameObject();
            go.name = "Main Camera";
            Cam = go.AddComponent<Camera>();
            Cam.tag = Tag.MainCamera;
            go.AddComponent<AudioListener>();
        }

        Cam.gameObject.AddComponent<CinemachineBrain>();

        Cam.transform.SetParent(transform);  // 用途なし。Unity Editorで見やすいようにのみ。

        _pov = _virtualCamera.GetCinemachineComponent<CinemachinePOV>();
    }

    public void Init(Transform target)
    {
        _virtualCamera.Follow = target;

        ResetRotation();
    }

    public void ResetRotation()
    {
        // target の後ろにいる
        _pov.m_HorizontalAxis.Value = _virtualCamera.Follow.rotation.eulerAngles.y;
    }
}
