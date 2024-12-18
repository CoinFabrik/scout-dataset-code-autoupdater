namespace dataset_code_autoupdater;

class CloneAction : Action
{
    public string Repository;

    public CloneAction(string repo)
    {
        Repository = repo;
    }

    public override void Execute(Config config)
    {
        config.WorkingDirectory = Environment.CurrentDirectory;
        var dst = Utility.GetGitDestination(Repository, Environment.CurrentDirectory);
        Utility.RunProcessThrowing(config, "git", "clone", Repository, dst);
        config.WorkingDirectory = dst;
        Utility.RunProcessThrowing(config, "git", "remote", "add", config.RemoteName, config.DatasetCodeLocalDir);
    }
}