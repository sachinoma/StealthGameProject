using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class IntroSceneDirector : MonoBehaviour
{
    [SerializeField] IntroMenu _introMenu;
    [SerializeField] Camera _cameraForVideo;
    [SerializeField] VideoPlayer _videoWithLogo;

    // Start is called before the first frame update
    private void Start()
    {
        _introMenu.ViewChanged += OnMainMenuViewChanged;
        _introMenu.Initialize();
    }

    private void OnDestroy()
    {
        _introMenu.ViewChanged -= OnMainMenuViewChanged;
    }

    private void OnMainMenuViewChanged(IntroMenu.View view)
    {
        bool isPlayVideoWithLogo = view != IntroMenu.View.Credit;

        SetVideoActive(_videoWithLogo, isPlayVideoWithLogo);
    }

    private void SetVideoActive(VideoPlayer video, bool isActive)
    {
        video.renderMode = isActive ? VideoRenderMode.CameraFarPlane : VideoRenderMode.APIOnly;
        if(isActive)
        {
            video.targetCamera = _cameraForVideo;
        }
        video.SetDirectAudioMute(0, !isActive);
    }
}
