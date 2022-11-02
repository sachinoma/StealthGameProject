using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundVisualRange : MonoBehaviour
{
    public void Show(Vector3 worldPos, float radius, float time)
    {
        transform.position = worldPos;
        float diameter = radius * 2;
        transform.localScale = new Vector3(diameter, diameter, diameter);

        StartCoroutine(DelayDestroy(time));
    }


    private IEnumerator DelayDestroy(float time)
    {
        yield return new WaitForSeconds(time);

        Destroy(gameObject);
    }
}
