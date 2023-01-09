using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TitleSceneDirector : MonoBehaviour
{
    [SerializeField] MainMenu _mainMenu;
    [SerializeField] Camera _cameraForVideo;
    [SerializeField] VideoPlayer _videoWithLogo;
    [SerializeField] VideoPlayer _videoWithoutLogo;

    // Start is called before the first frame update
    private void Start()
    {
        _mainMenu.ViewChanged += OnMainMenuViewChanged;
        _mainMenu.Initialize();
    }

    private void OnDestroy()
    {
        _mainMenu.ViewChanged -= OnMainMenuViewChanged;
    }

    private void OnMainMenuViewChanged(MainMenu.View view)
    {
        bool isPlayVideoWithLogo = view != MainMenu.View.Credit;

        SetVideoActive(_videoWithLogo, isPlayVideoWithLogo);
        SetVideoActive(_videoWithoutLogo, !isPlayVideoWithLogo);
    }

    private void SetVideoActive(VideoPlayer video, bool isActive)
    {
        video.renderMode = isActive ? VideoRenderMode.CameraFarPlane : VideoRenderMode.APIOnly;
        if (isActive) {
            video.targetCamera = _cameraForVideo;
        }
        video.SetDirectAudioMute(0, !isActive);
    }
}
