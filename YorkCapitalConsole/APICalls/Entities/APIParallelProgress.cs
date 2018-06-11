using APICalls.Entities.Interfaces;
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
    public sealed class APIParallelProgress : IAPIParallelProgress
    {
        public float Percentage { get; set; } = 0.0f;
        public int Tasks { get; set; } = 0;
        public string Url { get; set; } = string.Empty;
    }
}
