using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HadoScript : MonoBehaviour
{
    [SerializeField]
    private float _multipleSize = 1.01f;

    private Image _image;

    private void Start()
    {
        _image = GetComponent<Image>();
        int rand = Random.Range(4, 10);
        transform.localScale = new Vector3(0.1f * rand, 0.1f * rand, 1f);
        StartCoroutine("destroy");
    }

    private void Update()
    {
        gameObject.transform.localScale = gameObject.transform.localScale * _multipleSize;
        Color color = _image.color;
        color.a -= 0.01f;
        _image.color = color;
    }

    private IEnumerator destroy()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
