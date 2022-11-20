using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundVisualRangeSphere : SoundVisualRangeBase
{
    protected override void UpdateRadius(float radius)
    {
        float diameter = radius * 2;
        transform.localScale = new Vector3(diameter, diameter, diameter);
    }
}
