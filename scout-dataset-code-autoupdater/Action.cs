namespace dataset_code_autoupdater;

abstract class Action
{
    protected abstract bool ExecuteInternal(State state);
    private readonly List<Action> _subactions = new();

    public void Execute(State state)
    {
        if (!ExecuteInternal(state))
        {
            foreach (var subaction in _subactions)
                subaction.ReportNonExecution(state, 1);
            return;
        }

        foreach (var subaction in _subactions)
            subaction.Execute(state);
    }

    protected abstract void ReportNonExecutionInternal(State state, int level);

    private void ReportNonExecution(State state, int level)
    {
        ReportNonExecutionInternal(state, level);
        foreach (var subaction in _subactions)
            subaction.ReportNonExecution(state, level + 1);
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
