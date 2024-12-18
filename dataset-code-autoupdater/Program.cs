﻿using LibGit2Sharp;
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

        static IEnumerable<TaggingAction> ComputeTaggingActions(State state)
        {
            using var repo = new Repository(state.DatasetCodeLocalDir);
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

                yield return new TaggingAction(finding.Repo, finding.Commit, finding.PredictedTagName);
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

        static IEnumerable<Action> FinalizeActions(Dictionary<string, Dictionary<string, List<TaggingAction>>> groups)
        {
            foreach (var (repo, subgroups) in groups)
            {
                var clone = new CloneAction(repo);
                foreach (var (commit, subgroup) in subgroups)
                {
                    var checkout = new CheckoutAction(repo, commit);
                    checkout.AddRange(subgroup);
                    clone.Add(checkout);
                }
                clone.Add(new CloseAction());
                yield return clone;
            }
        }

        static void Main(string[] args)
        {
            try
            {
                var config = new Config("http://gogs2.nkt/victor/scout-substrate-dataset-code.git", "target_remote");
                using var state = new State(config);
                state.RunProcess("git", "clone", "--bare", config.RemoteUrl, state.DatasetCodeLocalDir);

                var actions = FinalizeActions(GroupActionsByCommit(GroupActionsByRepo(ComputeTaggingActions(state))))
                    .ToList();
                foreach (var action in actions)
                    action.Execute(state);

                Console.Write(Environment.NewLine.Repeat(10));

                if (state.Errors.Count == 0)
                {
                    Console.WriteLine("All done, no errors!");
                }
                else
                {
                    Console.Error.WriteLine($"Completed with {state.Errors.Count} error(s)");
                    foreach (var error in state.Errors)
                        Console.Error.WriteLine(error);
                }
                Console.WriteLine($"Now push {state.DatasetCodeLocalDir}");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine(e.StackTrace);
            }
        }
    }
}
