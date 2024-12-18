namespace dataset_code_autoupdater;

class DatasetFinding
{
#pragma warning disable CS0649
    public long audited_project_id;
    public long issue_index;
    public string repository;
    public string audited_commit;
    public string? reported_remediated_commit;
#pragma warning restore CS0649

    public IEnumerable<Finding> ToFindings()
    {
        yield return new Finding
        {
            Repo = repository,
            Commit = audited_commit,
            ProjectId = audited_project_id,
            IssueIndex = issue_index,
            Remediated = false,
        };
        if (reported_remediated_commit != null)
        {
            yield return new Finding
            {
                Repo = repository,
                Commit = reported_remediated_commit,
                ProjectId = audited_project_id,
                IssueIndex = issue_index,
                Remediated = true,
            };
        }
    }
}
