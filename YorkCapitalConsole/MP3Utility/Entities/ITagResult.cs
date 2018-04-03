
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagUtility.Entities
{
    public interface ITagResult
    {
        string FilePath { get; set; }

        void Action(TagOptions options);
    }
}
