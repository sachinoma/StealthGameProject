using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinSceneDummyPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Step());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator Step()
    {
        while(true)
        {
            yield return new WaitForSeconds(3.0f);
            VisualRangeGenerator.Instance?.Generate(transform.position, 1);
        }
    }
}
