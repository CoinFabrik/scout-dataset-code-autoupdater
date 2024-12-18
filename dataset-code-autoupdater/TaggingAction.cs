using LibGit2Sharp;

namespace dataset_code_autoupdater;

class TaggingAction(string sourceRepository, string commit, string newTagName) : Action
{
    public string SourceRepository = sourceRepository;
    public string Commit = commit;
    public string NewTagName = newTagName;

    protected override bool ExecuteInternal(State state)
    {
        state.RunProcess("git", "tag", NewTagName);
        state.RunProcess("git", "push", state.RemoteName, "tag", NewTagName);
        return true;
    }
}
