using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataset_code_autoupdater
{
    static class Utility
    {
        public static int RunProcess(Config config, string command, params string[] arguments)
        {
            config.Log($"@{config.WorkingDirectory}\n{command}\n{arguments.Select(x => $">{x}").Join("\n")}");
            var psi = new ProcessStartInfo(command, arguments)
            {
                WorkingDirectory = config.WorkingDirectory,
                //CreateNoWindow = true,
            };

            using var process = new Process()
            {
                StartInfo = psi
            };

            process.Start();
            process.WaitForExit();

            return process.ExitCode;
        }

        public static void RunProcessThrowing(Config config, string command, params string[] arguments)
        {
            var code = RunProcess(config, command, arguments);
            if (code != 0)
                throw new Exception($"{command} {arguments.Join(" ")} failed with code {code}");
        }

        public static string Join(this IEnumerable<string> ss, string? separator)
        {
            var ret = new StringBuilder();
            bool first = true;
            foreach (var s in ss)
            {
                if (first)
                    first = false;
                else if (separator != null)
                    ret.Append(separator);
                ret.Append(s);
            }
            return ret.ToString();
        }

        public static string GetGitDestination(string repositoryUrl, string? parentPath = null)
        {
            var ret = Path.GetFileName(new Uri(repositoryUrl).LocalPath);
            if (parentPath == null)
                return ret;
            var basePath = ret = Path.Join(parentPath, ret);
            for (int i = 1; Directory.Exists(ret); i++)
                ret = $"{basePath}-{i}";
            return ret;
        }
    }
}
