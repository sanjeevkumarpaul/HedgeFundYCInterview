using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Entities
{
    /// <summary>
    /// In Parallel task, it will calcualte the Percentage and Report back.
    /// </summary>
    public class APIParallelProgress
    {
        public float Percentage { get; internal set; } = 0.0f;
        public int Tasks { get; internal set; } = 0;

        public string Url { get; internal set; } = string.Empty;
    }
}
