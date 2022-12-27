using UnityEngine;

/// <summary>
/// プレイヤーの Action 行動を反応できるオブジェクト
/// </summary>
public abstract class ReactableBase : MonoBehaviour
{
    public abstract ReactableType GetReactableType();
}
