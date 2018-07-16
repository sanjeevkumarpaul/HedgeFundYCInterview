using APICalls.Entities.Interfaces;

namespace APICalls.Dependents
{
    /// <summary>
    /// In Parallel task, it will calcualte the Percentage and Report back.
    /// </summary>
    public sealed class APIParallelProgress : IAPIParallelProgress
    {
        public float Percentage { get; set; } = 0.0f;
        public int Tasks { get; set; } = 0;
        public string Url { get; set; } = string.Empty;
    }
}
