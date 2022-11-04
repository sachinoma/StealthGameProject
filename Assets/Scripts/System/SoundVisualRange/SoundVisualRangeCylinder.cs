using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundVisualRangeCylinder : SoundVisualRangeBase
{
    private float _defaultHeight;

    protected override void Start()
    {
        base.Start();

        _defaultHeight = transform.localScale.y;
    }

    protected override void UpdateRadius(float radius)
    {
        float diameter = radius * 2;
        transform.localScale = new Vector3(diameter, _defaultHeight, diameter);
    }
}
