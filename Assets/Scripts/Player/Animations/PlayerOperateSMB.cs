using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOperateSMB : StateMachineBehaviour
{
    [SerializeField] private float _moveToOperateRefTime = 0.5f;

    private PlayerModel _playerModel;
    private Transform _operateRefTransform;
    private float _startMoveTime;
    private Vector3 _originalPos;
    private Quaternion _originalRot;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        if(_playerModel == null)
        {
            _playerModel = animator.GetComponentInParent<PlayerModel>();
        }

        _operateRefTransform = _playerModel.GetOperateRefTransform();
        _startMoveTime = Time.time;
        _originalPos = _playerModel.transform.position;
        _originalRot = _playerModel.transform.rotation;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        float moveTime = Time.time - _startMoveTime;
        if(moveTime > _moveToOperateRefTime)
        {
            _playerModel.transform.position = _operateRefTransform.position;
            _playerModel.transform.rotation = _operateRefTransform.rotation;
        }
        else
        {
            float progress = moveTime / _moveToOperateRefTime;
            _playerModel.transform.position = Vector3.Lerp(_originalPos, _operateRefTransform.position, progress);
            _playerModel.transform.rotation = Quaternion.Lerp(_originalRot, _operateRefTransform.rotation, progress);
        }
    }
}
