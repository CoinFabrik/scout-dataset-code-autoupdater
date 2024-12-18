using LibGit2Sharp;

namespace dataset_code_autoupdater;

class TaggingAction : Action
{
    public string SourceRepository;
    public string Commit;
    public string NewTagName;

    public override void Execute(Config config)
    {
        Utility.RunProcessThrowing(config, "git", "tag", NewTagName);
        Utility.RunProcessThrowing(config, "git", "push", config.RemoteName, "tag", NewTagName);
    }
}
