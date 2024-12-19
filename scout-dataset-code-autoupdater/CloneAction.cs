using System.Net;
using LibGit2Sharp;

namespace dataset_code_autoupdater;

class CloneAction : Action
{
    public string Repository;

    public CloneAction(string repo)
    {
        Repository = repo;
    }

    public static bool IsHttpRepositoryPublic(string url)
    {
        using var client = new HttpClient();
        var get = client.GetAsync(new Uri(url));
        get.Wait();
        return get.Result.StatusCode == HttpStatusCode.OK;
    }

    public static bool IsRepositoryPublic(string url)
    {
        if (url.StartsWith("http://") || url.StartsWith("https://"))
            return IsHttpRepositoryPublic(url);
        return false;
    }

    protected override bool ExecuteInternal(State state)
    {
        if (!IsRepositoryPublic(Repository))
        {
            state.Errors.Add($"Repository {Repository} is not public or does not exist.");
            return false;
        }

        state.WorkingDirectory = state.TemporaryCloneLocation;
        var dst = Utility.GetGitDestination(Repository, state.WorkingDirectory);
        state.RunProcess("git", "clone", Repository, dst);
        state.WorkingDirectory = dst;
        state.RunProcess("git", "remote", "add", state.RemoteName, state.DatasetCodeLocalDir);
        return true;
    }

    protected override void ReportNonExecutionInternal(State state, int level)
    {
        state.Errors.Add($"{"  ".Repeat(level)}--> Will not clone repository {Repository}");
    }
}