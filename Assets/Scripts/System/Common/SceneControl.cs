using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SceneControl
{
    public const string TitleSceneName = "Title";
    public const string IntroSceneName = "Intro";
    public const string MainSceneName = "Main";
    public const string EndingSceneName = "Ending";

    #region UI

    public const string GameOverUISceneName = "GameOver";
    public const string GameClearUISceneName = "GameClear";

    #endregion

    private static bool HasRegisteredSceneLoadedEvent = false;
    public static bool IsChangingScene { get; private set; } = false;

    /// <returns>シーンの遷移ができるか</returns>
    public static bool ChangeScene(string targetSceneName, bool isDisableUIControl = true)
    {
        if(IsChangingScene)
        {
            return false;
        }

        if (!HasRegisteredSceneLoadedEvent)
        {
            HasRegisteredSceneLoadedEvent = true;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        Action onFadeOutFinished = () =>
        {
            SceneManager.LoadScene(targetSceneName);
        };

        IsChangingScene = SceneTransition.Instance.Fade(false, onFadeOutFinished);

        if(IsChangingScene && isDisableUIControl)
        {
            EventSystem eventSystem = EventSystem.current;
            if(eventSystem != null)
            {
                eventSystem.enabled = false;
            }
        }

        return IsChangingScene;
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

        if(!IsChangingScene){
            return;
        }

        Action onFadeInFinished = () =>
        {
            IsChangingScene = false;
        };

        SceneTransition.Instance.Fade(true, onFadeInFinished);
    }
}
