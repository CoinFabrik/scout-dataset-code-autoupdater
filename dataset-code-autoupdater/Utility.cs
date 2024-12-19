using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace dataset_code_autoupdater
{
    static class Utility
    {
        public static int RunProcess(string workingDirectory, string command, params string[] arguments)
        {
            var psi = new ProcessStartInfo(command, arguments)
            {
                WorkingDirectory = workingDirectory,
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

        public static void RunProcessThrowing(string workingDirectory, string command, params string[] arguments)
        {
            var code = RunProcess(workingDirectory, command, arguments);
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
            return PickNonexistentFileName(parentPath, ret);
        }

        public static string PickNonexistentFileName(string parent, string baseName)
        {
            var basePath = Path.Join(parent, baseName);
            var ret = basePath;
            for (int i = 2; Directory.Exists(ret); i++)
                ret = $"{basePath}-{i}";
            return ret;
        }

        public static string Repeat(this string s, int n)
        {
            var ret = new StringBuilder();
            for (int i = n; i-- > 0; )
                ret.Append(s);
            return ret.ToString();
        }

        public static string CloneDirectoty(string parent, string directory)
        {
            var ret = PickNonexistentFileName(parent, directory);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                RunProcess(parent, "robocopy", "/mir", directory, ret);
            else
                RunProcessThrowing(parent, "cp", "-r", directory, ret);
            return ret;
        }

        public static void CloneOrUpdate(string repoUrl, string cloneDir, string destination, CloneOptions? options = null)
        {
            options ??= new CloneOptions();

            var path = Path.Join(cloneDir, destination);
            if (Directory.Exists(path))
            {
                if (options.Bare)
                    RunProcessThrowing(path, "git", "fetch", repoUrl);
                else
                {
                    RunProcessThrowing(path, "git", "pull");
                    if (options.Branch != null)
                    {
                        using var repo = new Repository(path);
                        if (repo.Head.FriendlyName != options.Branch)
                        {
                            RunProcessThrowing(path, "git", "checkout", options.Branch);
                            RunProcessThrowing(path, "git", "pull");
                        }
                    }
                }
            }
            else
            {
                var args = new List<string>();
                args.Add("clone");
                if (options.Bare)
                    args.Add("--bare");
                args.Add(repoUrl);
                if (options.Branch != null)
                {
                    args.Add("-b");
                    args.Add(options.Branch);
                }
                args.Add(destination);
                RunProcessThrowing(cloneDir, "git", args.ToArray());
            }
        }

    }
}
