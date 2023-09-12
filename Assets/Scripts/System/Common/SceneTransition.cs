using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Video;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private float _transitionTime = 1.0f;

    [Header("AudioMixerSnapshot")]
    [SerializeField] private AudioMixerSnapshot _normalSnapshot;
    [SerializeField] private AudioMixerSnapshot _muteSnapshot;

    [SerializeField] private Image _blackOverlay;
    private float _startTransitionTime;

    private const string ResourcesFilePath = "SceneTransitionCanvas";

    private bool _isFading = false;

    private static SceneTransition _Instance = null;
    public static SceneTransition Instance
    {
        get
        {
            if(_Instance == null)
            {
                _Instance = Instantiate(Resources.Load<SceneTransition>(ResourcesFilePath));
                DontDestroyOnLoad(_Instance.gameObject);
            }

            return _Instance;
        }
    }

    /// <returns>フェードができるか</returns>
    public bool Fade(bool isFadeIn, Action onFinished = null)
    {
        if(_isFading)
        {
            return false;
        }

        StartCoroutine(FadeCoroutine(isFadeIn, onFinished));
        return true;
    }

    private IEnumerator FadeCoroutine(bool isFadeIn, Action onFinished = null)
    {
        if(_isFading)
        {
            yield break;
        }
        _isFading = true;

        // 音声
        AudioMixerSnapshot targetSnapshot = isFadeIn ? _normalSnapshot : _muteSnapshot;
        targetSnapshot.TransitionTo(_transitionTime);

        _startTransitionTime = Time.time;

        float startAlpha = isFadeIn ? 1.0f : 0.0f;
        float endAlpha = isFadeIn ? 0.0f : 1.0f;

        VideoPlayer[] videos = FindObjectsOfType<VideoPlayer>();
        float startVideoVolume = isFadeIn ? 0.0f : 1.0f;
        float endVideoVolume = isFadeIn ? 1.0f : 0.0f;

        Color newColor = _blackOverlay.color;
        while(Time.time - _startTransitionTime < _transitionTime)
        {
            float progress = (Time.time - _startTransitionTime) / _transitionTime;

            // 画面
            newColor.a = Mathf.Lerp(startAlpha, endAlpha, progress);
            _blackOverlay.color = newColor;

            // 動画
            if(videos != null)
            {
                foreach(VideoPlayer video in videos)
                {
                    video.SetDirectAudioVolume(0, Mathf.Lerp(startVideoVolume, endVideoVolume, progress));
                }
            }

            yield return null;
        }

        // 最終の処理
        newColor.a = endAlpha;
        _blackOverlay.color = newColor;
        if(videos != null)
        {
            foreach(VideoPlayer video in videos)
            {
                video.SetDirectAudioVolume(0, endVideoVolume);
            }
        }

        onFinished?.Invoke();

        _isFading = false;
    }
}
