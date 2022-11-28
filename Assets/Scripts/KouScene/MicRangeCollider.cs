﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicRangeCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == Tag.SoundChecker)
        {
            Debug.Log("Enter");
        }
    }
    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == Tag.SoundChecker)
        {
            Debug.Log("Stay");
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == Tag.SoundChecker)
        {
            Debug.Log("Exit");
        }
    }
}
