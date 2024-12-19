namespace dataset_code_autoupdater;

class Finding
{
    public string? Repo;
    public string? Commit;
    public bool Remediated;
    public long ProjectId;
    public long IssueIndex;

    private string? _predictedTagName;
    public string PredictedTagName
    {
        get
        {
            if (_predictedTagName == null)
                _predictedTagName = (Remediated ? "remediated" : "audited") + "-" + ProjectId + "-" + IssueIndex;
            return _predictedTagName;
        }
    }

    public class Comparer : IEqualityComparer<Finding>
    {
        public bool Equals(Finding? l, Finding? r)
        {
            if (ReferenceEquals(l, r)) return true;
            if (ReferenceEquals(l, null)) return false;
            if (ReferenceEquals(r, null)) return false;
            if (l.GetType() != r.GetType()) return false;
            return l.Repo == r.Repo
                   && l.Commit == r.Commit
                   && l.Remediated == r.Remediated
                   && l.ProjectId == r.ProjectId
                   && l.IssueIndex == r.IssueIndex;
        }

        public int GetHashCode(Finding obj)
        {
            return HashCode.Combine(
                obj.Repo,
                obj.Commit,
                obj.Remediated,
                obj.ProjectId,
                obj.IssueIndex
            );
        }
    }
}
