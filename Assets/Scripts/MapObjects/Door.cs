using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] protected Animator _animator;

    public void Open()
    {
        _animator.SetBool("isOpen", true);
    }

    public void Close()
    {
        _animator.SetBool("isOpen", false);
    }
}
