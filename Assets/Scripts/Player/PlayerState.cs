public enum PlayerState
{
    Moving,     // Idle も Moving とみなし
    Acting,     // 例えば、アイテムを拾っている、隠している、現している
    Hiding,
    TakingDamage,
    Died,
}
