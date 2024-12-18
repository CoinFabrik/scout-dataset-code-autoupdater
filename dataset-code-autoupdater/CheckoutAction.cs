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
        Utility.RunProcessThrowing(config, "git", "checkout", Commit);
        Utility.RunProcessThrowing(config, "git", "branch", $"temp-{Commit}");
        Utility.RunProcessThrowing(config, "git", "checkout", $"temp-{Commit}");
    }
}
