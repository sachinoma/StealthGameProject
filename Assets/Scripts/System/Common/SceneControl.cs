using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl
{
    public const string TitleSceneName = "Title";
    public const string MainSceneName = "Main";

    #region UI

    public const string GameOverUISceneName = "GameOver";
    public const string GameClearUISceneName = "GameClear";

    #endregion

    public static void ChangeScene(string targetSceneName)
    {
        // TODO : トランジション
        SceneManager.LoadScene(targetSceneName);
    }

    public static void LoadUI(string uiSceneName)
    {
        SceneManager.LoadScene(uiSceneName, LoadSceneMode.Additive);
    }
}