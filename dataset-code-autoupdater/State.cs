namespace dataset_code_autoupdater;

class State : Config, IDisposable
{
    private StreamWriter? _logFile = new("log.txt");
    public string WorkingDirectory = ".";
    public string DatasetCodeLocalDir;
    public List<string> Errors = new();

    public State(Config config): base(config.RemoteUrl, config.RemoteName)
    {
        DatasetCodeLocalDir = Utility.GetGitDestination(config.RemoteUrl, Environment.CurrentDirectory);
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
}
