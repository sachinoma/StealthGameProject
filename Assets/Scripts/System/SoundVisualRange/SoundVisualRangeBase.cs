using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO : VisualRangeをオンオフ制御？
public abstract class SoundVisualRangeBase : MonoBehaviour
{
    protected ISoundSource _soundSource;

    protected virtual void Start()
    {
        _soundSource = GetComponentInParent<ISoundSource>();
        if(_soundSource == null)
        {
            Debug.LogWarning("ISoundSourceをParentから探せない。このGameObjectをinactiveになる。");
            gameObject.SetActive(false);
        }
    }

    protected void Update()
    {
        if(_soundSource != null)
        {
            UpdateRadius(_soundSource.GetCurrentSoundRadius());
        }
    }

    protected abstract void UpdateRadius(float radius);
}
