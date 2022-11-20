using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinSceneDummyPlayer2 : PlayerModel, ISoundSource
{
    public float GetCurrentSoundRadius()
    {
        return GetComponent<Rigidbody>().velocity.magnitude / 5;
    }
}
