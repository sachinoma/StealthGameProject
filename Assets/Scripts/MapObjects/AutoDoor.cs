using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDoor : Door
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(Tag.Player))
        {
            Open();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag(Tag.Player))
        {
            Close();
        }
    }
}
