using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Entities
{
    /// <summary>
    /// All API Results will be alloted with this interface type.
    /// </summary>
    public interface IAPIProspect
    {        
    }

    /// <summary>
    /// API result with ByteArray/Stream/StringArray should be interfaced with upgrade to get the results other wise
    /// after execution the data will be consumed by calle and will not be emited back to caller with right output.
    /// </summary>
    public interface IAPIProspectUpgrade : IAPIProspect
    {
        dynamic OtherResponses { get; set; }  //Can contain STREAM/ByteArray/String
    }
}
