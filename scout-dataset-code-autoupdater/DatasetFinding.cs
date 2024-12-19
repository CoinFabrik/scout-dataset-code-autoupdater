using Newtonsoft.Json;

namespace dataset_code_autoupdater;

class DatasetFinding
{
    [JsonIgnore]
    public long AuditedProjectId;
    [JsonIgnore]
    public long IssueIndex;
#pragma warning disable CS0649
    [JsonProperty(PropertyName = "repository")]
    public string Repository = "";
    [JsonProperty(PropertyName = "audited_commit")]
    public string AuditedCommit = "";
    [JsonProperty(PropertyName = "reported_remediated_commit")]
    public string? ReportedRemediatedCommit;
#pragma warning restore CS0649

    public IEnumerable<Finding> ToFindings()
    {
        yield return new Finding
        {
            Repo = Repository,
            Commit = AuditedCommit,
            ProjectId = AuditedProjectId,
            IssueIndex = IssueIndex,
            Remediated = false,
        };
        if (ReportedRemediatedCommit != null)
        {
            yield return new Finding
            {
                Repo = Repository,
                Commit = ReportedRemediatedCommit,
                ProjectId = AuditedProjectId,
                IssueIndex = IssueIndex,
                Remediated = true,
            };
        }
    }
}
