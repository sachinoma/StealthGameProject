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
        SetVideoActive(_videoIntro);
        _introMenu.Initialize();
    }

    private void OnDestroy()
    {

    }

    private void SetVideoActive(VideoPlayer video)
    {
        video.renderMode = VideoRenderMode.CameraFarPlane ;
        video.targetCamera = _cameraForVideo;
        video.loopPointReached += LoopPointReached;
    }

    public void LoopPointReached(VideoPlayer video)
    {
        // 動画再生完了時の処理
        video.Stop();
        SceneControl.ChangeScene(SceneControl.MainSceneName);
    }
}
