using UnityEngine;

public class HintEventTrigger : MapEventTriggerBase
{
    [SerializeField] private HintType hintType;

    private void Start()
    {
        if(!GameProgress.CheckIsHintRemain(hintType))
        {
            Destroy(gameObject);
        }
    }

    protected override void Trigger()
    {
        Hint.TriggerHintIfRemain(this, hintType);
    }
}
