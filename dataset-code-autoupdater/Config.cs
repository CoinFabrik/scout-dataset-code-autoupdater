namespace dataset_code_autoupdater;

class Config : IDisposable
{
    public string WorkingDirectory = ".";
    public string RemoteUrl;
    public string RemoteName;
    public string DatasetCodeLocalDir;
    private StreamWriter _logFile = new("log.txt");

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
}
