﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAniEvent : MonoBehaviour
{
    [SerializeField] PlayerModel _playerModel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FootEvent()
    {
        _playerModel.AnimSoundVolume();
    }
}