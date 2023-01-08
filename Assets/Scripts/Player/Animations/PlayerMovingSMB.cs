using UnityEngine;

public class PlayerMovingSMB : StateMachineBehaviour
{
    private PlayerModel _playerModel;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        if(_playerModel == null)
        {
            _playerModel = animator.GetComponentInParent<PlayerModel>();
        }

        _playerModel.SetMovingState();
    }
}
