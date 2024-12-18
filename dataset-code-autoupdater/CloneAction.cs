namespace dataset_code_autoupdater;

class CloneAction : Action
{
    public string Repository;

    public CloneAction(string repo)
    {
        Repository = repo;
    }

    protected override bool ExecuteInternal(State state)
    {
        state.WorkingDirectory = Environment.CurrentDirectory;
        var dst = Utility.GetGitDestination(Repository, Environment.CurrentDirectory);
        state.RunProcess("git", "clone", Repository, dst);
        state.WorkingDirectory = dst;
        state.RunProcess("git", "remote", "add", state.RemoteName, state.DatasetCodeLocalDir);
        return true;
    }
}