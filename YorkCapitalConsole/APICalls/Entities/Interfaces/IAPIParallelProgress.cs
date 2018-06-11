using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Entities.Interfaces
{
    public interface IAPIParallelProgress
    {
         float Percentage { get; set; }
         int Tasks { get; set; }
         string Url { get; set; }
    }
}
