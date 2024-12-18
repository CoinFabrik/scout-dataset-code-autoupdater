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
        var url = new Uri(Repository);
        var basePath = config.WorkingDirectory = $"./{Path.GetFileName(url.LocalPath)}";
        for (int i = 1; Directory.Exists(config.WorkingDirectory); i++)
            config.WorkingDirectory = $"{basePath}-{i}";
        Utility.RunProcessThrowing(".", "git", "clone", Repository, config.WorkingDirectory);
        Utility.RunProcessThrowing(config.WorkingDirectory, "git", "remote", "add", config.RemoteName, config.RemoteUrl);
    }
}