using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapAnimChanger : MonoBehaviour
{
    Animator _trapAnim;

    void Awake()
    {
        _trapAnim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _trapAnim.SetBool("IsOn", true);
    }

    private void OnTriggerExit(Collider other)
    {
        _trapAnim.SetBool("IsOn", false);
    }
}
