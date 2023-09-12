using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class DemoSceneDirector : MonoBehaviour
{
    [SerializeField] VideoPlayer _videoDemo;

    void Start()
    {
        _videoDemo.loopPointReached += LoopPointReached;
    }

    public void LoopPointReached(VideoPlayer video)
    {
        // 動画再生完了時の処理
        video.Stop();
        SceneControl.ChangeScene(SceneControl.TitleSceneName);
    }
}
