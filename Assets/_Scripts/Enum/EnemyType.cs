/// <summary>
/// Identifies an enemy variant so the pool can spawn the matching prefab.
/// Keep in sync with the prefab entries configured on the EnemyPool.
/// </summary>
public enum EnemyType : byte
{
    Slime,
    Skeleton,
    Archer,
    Boss,
}
