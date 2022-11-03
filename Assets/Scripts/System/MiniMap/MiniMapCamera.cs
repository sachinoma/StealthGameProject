using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class MiniMapCamera : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Camera _camera;
    [SerializeField] private PositionConstraint _positionConstraint;
    [SerializeField] private RotationConstraint _rotationConstraint;

    public bool SetFocus(Transform focus, bool isFollowRotation)
    {
        if (focus == null)
        {
            Debug.LogWarning("ターゲットなし。SetFocusできない。");
            return false;
        }

        ConstraintSource source = new ConstraintSource();
        source.sourceTransform = focus;
        source.weight = 1;

        SetPositionConstraint(source);
        SetRotationConstraint(source, isFollowRotation);

        return true;
    }

    private void SetPositionConstraint(ConstraintSource source)
    {
        if(_positionConstraint.sourceCount == 0)
        {
            _positionConstraint.AddSource(source);
        }
        else
        {
            _positionConstraint.SetSource(0, source);
        }

        _positionConstraint.constraintActive = true;
    }

    private void SetRotationConstraint(ConstraintSource source, bool isActive)
    {
        if(isActive)
        {
            if(_rotationConstraint.sourceCount == 0)
            {
                _rotationConstraint.AddSource(source);
            }
            else
            {
                _rotationConstraint.SetSource(0, source);
            }
        }
        else
        {
            transform.rotation = Quaternion.Euler(_rotationConstraint.rotationAtRest);
        }

        _rotationConstraint.constraintActive = isActive;
    }
}
