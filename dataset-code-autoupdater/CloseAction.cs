using System.Runtime.InteropServices;

namespace dataset_code_autoupdater;

class CloseAction : Action
{
    protected override bool ExecuteInternal(State state)
    {
        return true;
    }

    protected override void ReportNonExecutionInternal(State state, int level)
    {
    }
}
