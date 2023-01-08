using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class TitleButton : MonoBehaviour
{
    [SerializeField]
    private GameObject _hado;

    [SerializeField]
    private GameObject[] _asi;

    private Vector2 _msPos = Vector2.zero;
    private bool _possibleEfect = true;

    private void Update()
    {
        if(_possibleEfect)
        {
            var mouse = Mouse.current;
            if(mouse != null)
                _msPos = mouse.position.ReadValue();

            foreach(var i in _asi)
            {
                Debug.Log(i);
                RectTransform rectT = i.GetComponent<RectTransform>();
                var rectPos = rectT.position;
                var rectSize = rectT.sizeDelta;
                if(_msPos.x > rectPos.x - rectSize.x / 2f && _msPos.x < rectPos.x + rectSize.x / 2f)
                    if(_msPos.y > rectPos.y - rectSize.y / 2f && _msPos.y < rectPos.y + rectSize.y / 2f)
                    {
                        Effect(_msPos);
                        _possibleEfect = false;
                        StartCoroutine("Possible");
                    }
            }
        }
    }

    private IEnumerator Possible()
    {
        yield return new WaitForSeconds(1);
        _possibleEfect = true;
    }

    private void Effect(Vector2 msPos)
    {
        Instantiate(_hado, new Vector3(msPos.x, msPos.y, 3.0f), Quaternion.identity, this.transform);
    }

    public void ClickGameStart()
    {
        StartCoroutine("StartWait");
        PlayableDirector direct = GetComponent<PlayableDirector>();
        direct.Play();
    }

    private IEnumerator StartWait()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Main");
    }

    public void ClickQuit()
    {
        Application.Quit();
    }
}
