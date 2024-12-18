namespace dataset_code_autoupdater;

abstract class Action
{
    protected abstract bool ExecuteInternal(State state);
    private readonly List<Action> _subactions = new();

    public void Execute(State state)
    {
        if (!ExecuteInternal(state))
            return;
        foreach (var subaction in _subactions)
            subaction.Execute(state);
    }

    public void Add(Action action)
    {
        _subactions.Add(action);
    }

    public void AddRange(IEnumerable<Action> actions)
    {
        _subactions.AddRange(actions);
    }
}
