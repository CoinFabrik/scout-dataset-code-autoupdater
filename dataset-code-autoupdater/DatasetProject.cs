using Newtonsoft.Json;

namespace dataset_code_autoupdater;

class DatasetProject
{
#pragma warning disable CS0649
    [JsonProperty(PropertyName = "audited_project_id")]
    public long AuditedProjectId;
    [JsonProperty(PropertyName = "findings")]
    public List<DatasetFinding> Findings = new();
#pragma warning restore CS0649
}
