/// <summary>
/// プレイヤーの Action 行動を反応できるオブジェクト
/// </summary>
public interface IReactable
{
    /// <returns><see langword="true"/>：反応する後まだ反応できる</returns>
    bool React();
    ReactableType GetReactableType();
}
