using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl
{
    public const string TitleSceneName = "Title";
    public const string IntroSceneName = "Intro";
    public const string MainSceneName = "Main";

    #region UI

    public const string GameOverUISceneName = "GameOver";
    public const string GameClearUISceneName = "GameClear";

    #endregion

    private static bool HasRegisteredSceneLoadedEvent = false;

    public static void ChangeScene(string targetSceneName)
    {
        if(!HasRegisteredSceneLoadedEvent)
        {
            HasRegisteredSceneLoadedEvent = true;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        Action onFadeOutFinished = () =>
        {
            SceneManager.LoadScene(targetSceneName);
        };

        SceneTransition.Instance.FadeOut(onFadeOutFinished);
    }

    public static void LoadUI(string uiSceneName)
    {
        SceneManager.LoadScene(uiSceneName, LoadSceneMode.Additive);
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(mode == LoadSceneMode.Additive)
        {
            return;
        }

        SceneTransition.Instance.FadeIn();
    }
}
