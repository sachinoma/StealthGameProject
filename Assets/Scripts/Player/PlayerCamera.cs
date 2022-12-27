using System.Collections;
using Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook _virtualCamera;
    [SerializeField] private Camera _cam;
    public Camera Cam => _cam;

    private void Awake()
    {
        // Cinemachineのために、このGameObjectをヒエラルキーのトップレベルオブジェクトになる
        // また、transformをデフォルトになる
        transform.SetParent(null, false);

        // Sceneの中の全てのメインカメラを探してから破棄する(自分が持っているメインカメラ除外)
        GameObject[] mainCameras = GameObject.FindGameObjectsWithTag(Tag.MainCamera);
        if (mainCameras != null)
        {
            foreach (GameObject mainCam in mainCameras)
            {
                if (mainCam != _cam.gameObject)
                {
                    Destroy(mainCam);
                }
            }
        }
    }

    private void Start()
    {
        ResetRotation();
    }

    public void ResetRotation()
    {
        // target の後ろにいる
        _virtualCamera.m_XAxis.Value = _virtualCamera.Follow.rotation.eulerAngles.y;
    }
}
