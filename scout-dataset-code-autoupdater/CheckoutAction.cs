using LibGit2Sharp;

namespace dataset_code_autoupdater;

class CheckoutAction : Action
{
    public string Repository;
    public string Commit;

    public CheckoutAction(string repo, string commit)
    {
        Repository = repo;
        Commit = commit;
    }

    private bool CheckValid(State state)
    {
        using var repository = new Repository(state.WorkingDirectory);
        var commit = repository.Lookup(Commit);
        return (commit as Commit) != null;
    }

    protected override bool ExecuteInternal(State state)
    {
        if (!CheckValid(state))
        {
            state.Errors.Add($"Commit {Commit} does not exist in {Repository}");
            return false;
        }

        state.RunProcess("git", "checkout", Commit);
        state.RunProcess("git", "branch", $"temp-{Commit}");
        state.RunProcess("git", "checkout", $"temp-{Commit}");
        return true;
    }

    protected override void ReportNonExecutionInternal(State state, int level)
    {
        state.Errors.Add($"{"  ".Repeat(level)}--> Will not checkout commit {Commit}");
    }
}
