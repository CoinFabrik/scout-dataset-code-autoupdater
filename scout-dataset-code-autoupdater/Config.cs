namespace dataset_code_autoupdater;

class Config
{
    public string DatasetCodeUrl = "";
    public string RemoteName = "";
    public string DatasetUrl = "";
    public string? DatasetBranch;
    public string CloneLocation = ".";
    public string TemporaryCloneLocation => Path.Join(CloneLocation, "temp");
}
