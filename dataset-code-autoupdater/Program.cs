using LibGit2Sharp;
using Newtonsoft.Json;

namespace dataset_code_autoupdater
{
    internal class Program
    {
        static List<DatasetFinding> LoadRawFindings()
        {
            using var file = new StreamReader(@"./findings-linear.json");
            return JsonConvert.DeserializeObject<List<DatasetFinding>>(file.ReadToEnd());
        }

        static HashSet<Finding> LoadFindings()
        {
            var ret = new HashSet<Finding>(new Finding.Comparer());
            foreach (var f in LoadRawFindings())
                foreach (var f2 in f.ToFindings())
                    ret.Add(f2);
            return ret;
        }

        static IEnumerable<TaggingAction> ComputeTaggingActions(Config config)
        {
            using var repo = new Repository(config.DatasetCodeLocalDir);
            var tagsByCommit = new Dictionary<string, List<Tag>>();
            var tagsByName = new Dictionary<string, List<Tag>>();
            foreach (var tag in repo.Tags)
            {
                var hash = tag.Target.Sha;
                var name = tag.FriendlyName;
                {
                    if (!tagsByCommit.TryGetValue(hash, out var tags))
                        tags = tagsByCommit[hash] = new List<Tag>();
                    tags.Add(tag);
                }
                {
                    if (!tagsByName.TryGetValue(name, out var tags))
                        tags = tagsByName[name] = new List<Tag>();
                    tags.Add(tag);
                }
            }

            var findings = LoadFindings();
            foreach (var finding in findings)
            {
                if (tagsByCommit.TryGetValue(finding.Commit, out var tagByCommit))
                {
                    if (!tagsByName.ContainsKey(finding.PredictedTagName))
                        throw new Exception($"Error 1 in dataset: {finding.PredictedTagName}");

                    if (tagByCommit.All(x => x.FriendlyName != finding.PredictedTagName))
                        throw new Exception($"Error 2 in dataset: {finding.PredictedTagName}");
                    continue;
                }

                if (tagsByName.ContainsKey(finding.PredictedTagName))
                    throw new Exception($"Error 3 in dataset: {finding.PredictedTagName}");

                yield return new TaggingAction
                {
                    SourceRepository = finding.Repo,
                    Commit = finding.Commit,
                    NewTagName = finding.PredictedTagName,
                };
            }
        }

        static Dictionary<string, List<TaggingAction>> GroupActionsByRepo(IEnumerable<TaggingAction> actions)
        {
            var ret = new Dictionary<string, List<TaggingAction>>();
            foreach (var action in actions)
            {
                if (!ret.TryGetValue(action.SourceRepository, out var actions2))
                    ret[action.SourceRepository] = actions2 = new List<TaggingAction>();
                actions2.Add(action);
            }
            return ret;
        }
        
        static Dictionary<string, Dictionary<string, List<TaggingAction>>> GroupActionsByCommit(Dictionary<string, List<TaggingAction>> actions)
        {
            var ret = new Dictionary<string, Dictionary<string, List<TaggingAction>>>();
            foreach (var (repo, groupedActions) in actions)
            {
                var finalGroupedActions = ret[repo] = new Dictionary<string, List<TaggingAction>>();
                foreach (var action in groupedActions)
                {
                    if (!finalGroupedActions.TryGetValue(action.Commit, out var finalGroup))
                        finalGroupedActions[action.Commit] = finalGroup = new List<TaggingAction>();
                    finalGroup.Add(action);
                }
            }
            return ret;
        }

        static IEnumerable<Action> FlattenActions(Dictionary<string, Dictionary<string, List<TaggingAction>>> groups)
        {
            foreach (var (repo, subgroups) in groups)
            {
                yield return new CloneAction(repo);
                foreach (var (commit, subgroup) in subgroups)
                {
                    yield return new CheckoutAction(commit);
                    foreach (var taggingAction in subgroup)
                        yield return taggingAction;
                }

                yield return new CloseAction();
            }
        }

        static void Main(string[] args)
        {
            try
            {
                var config = new Config
                {
                    RemoteName = "target_remote",
                    RemoteUrl = "http://gogs2.nkt/victor/scout-substrate-dataset-code.git",
                };
                config.DatasetCodeLocalDir = Utility.GetGitDestination(config.RemoteUrl, Environment.CurrentDirectory);
                Utility.RunProcessThrowing(config, "git", "clone", "--bare", config.RemoteUrl, config.DatasetCodeLocalDir);

                var actions = FlattenActions(GroupActionsByCommit(GroupActionsByRepo(ComputeTaggingActions(config))))
                    .ToList();
                foreach (var action in actions)
                    action.Execute(config);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine(e.StackTrace);
            }
        }
    }
}
