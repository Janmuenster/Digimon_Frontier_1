public interface IAction
{
    string Name { get; }
    void Execute(CharacterStats user, CharacterStats target);
    void Execute(EnemyStats enemyStats, PlayerStats playerStats);
}
