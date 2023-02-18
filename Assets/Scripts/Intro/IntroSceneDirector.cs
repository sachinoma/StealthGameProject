using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class IntroSceneDirector : MonoBehaviour
{
    [SerializeField] IntroMenu _introMenu;
    [SerializeField] Camera _cameraForVideo;
    [SerializeField] VideoPlayer _videoIntro;

    // Start is called before the first frame update
    private void Start()
    {
        _introMenu.ViewChanged += OnIntroViewChanged;
        _introMenu.Initialize();
    }

    private void OnDestroy()
    {
        _introMenu.ViewChanged -= OnIntroViewChanged;
    }

    private void OnIntroViewChanged(IntroMenu.View view)
    {
        bool isPlayVideoWithLogo = view != IntroMenu.View.Credit;

        SetVideoActive(_videoIntro, isPlayVideoWithLogo);
    }

    private void SetVideoActive(VideoPlayer video, bool isActive)
    {
        video.renderMode = isActive ? VideoRenderMode.CameraFarPlane : VideoRenderMode.APIOnly;
        if(isActive)
        {
            video.targetCamera = _cameraForVideo;
        }
        video.SetDirectAudioMute(0, !isActive);
        video.loopPointReached += LoopPointReached;
    }

    public void LoopPointReached(VideoPlayer video)
    {
        // 動画再生完了時の処理
        video.Pause();
        SceneControl.ChangeScene(SceneControl.MainSceneName);
    }
}
