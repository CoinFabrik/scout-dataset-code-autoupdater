namespace dataset_code_autoupdater;

public class CloneOptions
{
    public bool Bare = false;
    public string? Branch;

    public CloneOptions()
    {
    }

    public static CloneOptions DefaultPlusBare()
    {
        return new CloneOptions
        {
            Bare = true,
        };
    }

    public static CloneOptions DefaultPlusBranch(string branch)
    {
        return new CloneOptions
        {
            Branch = branch,
        };
    }
}
