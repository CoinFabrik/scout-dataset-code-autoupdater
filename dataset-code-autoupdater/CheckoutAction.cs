using LibGit2Sharp;

namespace dataset_code_autoupdater;

class CheckoutAction : Action
{
    public string Commit;

    public CheckoutAction(string commit)
    {
        Commit = commit;
    }

    public override void Execute(Config config)
    {
        Utility.RunProcessThrowing(config.WorkingDirectory, "git", "checkout", Commit);
        Utility.RunProcessThrowing(config.WorkingDirectory, "git", "branch", $"temp-{Commit}");
        Utility.RunProcessThrowing(config.WorkingDirectory, "git", "checkout", $"temp-{Commit}");
    }
}
