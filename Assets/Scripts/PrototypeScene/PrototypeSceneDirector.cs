using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrototypeSceneDirector : MonoBehaviour
{
    private PlayerModel _playerModel;

    private void Start()
    {
        _playerModel = FindObjectOfType<PlayerModel>();
        if(_playerModel != null)
        {
            _playerModel.Died += PlayerDiedHandler;
        }
    }

    private void OnDestroy()
    {
        if(_playerModel != null)
        {
            _playerModel.Died -= PlayerDiedHandler;
        }
    }

    private void PlayerDiedHandler()
    {
        Debug.LogWarning("Game Over!!");
        StartCoroutine(RestartGame(3.0f));
    }

    private IEnumerator RestartGame(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        SceneManager.LoadScene("Prototype");
    }
}
