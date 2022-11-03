using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scene の範囲内の Singleton。(DontDestroyOnLoad が付かない)<br />
/// あらかじめ Scene の中に入れる必要がある。つまり、動的生成ではない。<br />
/// Scene を破棄する時、この Instance も破棄される。<br />
/// Scene によって、SceneSingleton.Instance は null になる可能性がある。
/// </summary>
/// <typeparam name="T">派生クラス自体</typeparam>
public class SceneSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Debug.LogWarning("すでに他のInstanceがある");
            return;
        }

        Instance = GetComponent<T>();
    }

    protected void OnDestroy()
    {
        if(Instance != null && Instance == this)
        {
            Instance = null;
        }
    }

}
