using UnityEngine;

/// <summary>
/// プレイヤーの Action 行動を反応できるオブジェクト
/// </summary>
public abstract class ReactableBase : MonoBehaviour
{
    protected ReactableIndicator _billboardUIController;

    protected virtual void Awake()
    {
        _billboardUIController = GetComponentInChildren<ReactableIndicator>();
    }

    public abstract ReactableType GetReactableType();

    public void SetInReactableRange(bool isInRange)
    {
        if (_billboardUIController != null) {
            if (isInRange) {
                _billboardUIController.Show();
            } else {
                _billboardUIController.Hide();
            }
        }
    }
}
