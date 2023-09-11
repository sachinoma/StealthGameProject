using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDoor : Door
{
    private int count = 0;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(Tag.Player)|| other.CompareTag(Tag.Enemy))
        {
            if(isEmpty())
            {
                Open();
            }              
            ++count;            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag(Tag.Player)|| other.CompareTag(Tag.Enemy))
        {
            --count;
            if(isEmpty())
            {
                Close();
            }
        }
    }

    bool isEmpty()
    {
        return count == 0;
    }
}
