public class EnemyStateContext
{
    public IEnemyState CurrentState
    {
        get; set;
    }

    private readonly EnemyController _enemyController;

    public EnemyStateContext(EnemyController enemyController)
    {
        _enemyController = enemyController;
    }

    public void Transition()
    {
        CurrentState.Handle(_enemyController);
    }

    public void Transition(IEnemyState state)
    {
        CurrentState = state;
        CurrentState.Handle(_enemyController);
    }
}