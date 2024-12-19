using System.Runtime.InteropServices;

namespace dataset_code_autoupdater;

class State : IDisposable
{
    private Config _config;
    private StreamWriter? _logFile = new("log.txt");
    public string WorkingDirectory = ".";
    public string DatasetLocalDirName;
    public string DatasetLocalDir => Path.Join(_config.CloneLocation, DatasetLocalDirName);
    public string DatasetCodeLocalDirName;
    public string DatasetCodeLocalDir => Path.Join(_config.CloneLocation, DatasetCodeLocalDirName);
    public List<string> Errors = new();
    public string DatasetUrl => _config.DatasetUrl;
    public string DatasetBranch => _config.DatasetBranch;
    public string RemoteName => _config.RemoteName;
    public string DatasetCodeUrl => _config.DatasetCodeUrl;
    public string CloneLocation => _config.CloneLocation;
    public string TemporaryCloneLocation => _config.TemporaryCloneLocation;

    public State(Config config)
    {
        _config = config;
        DatasetLocalDirName = Utility.GetGitDestination(config.DatasetUrl);
        DatasetCodeLocalDirName = Utility.GetGitDestination(config.DatasetCodeUrl);
    }

    public void Dispose()
    {
        _logFile?.Dispose();
        _logFile = null;
    }

    public void Log(string message)
    {
        lock (_logFile)
        {
            _logFile.WriteLine(message);
            _logFile.Flush();
        }
    }

    public void RunProcess(string command, params string[] arguments)
    {
        Log($"@{WorkingDirectory}\n{command}\n{arguments.Select(x => $">{x}").Join("\n")}");
        Utility.RunProcessThrowing(WorkingDirectory, command, arguments);
    }

    public void CloneDatasetCodeDir()
    {
        DatasetCodeLocalDirName = Utility.CloneDirectoty(CloneLocation, DatasetCodeLocalDirName);
    }
}
