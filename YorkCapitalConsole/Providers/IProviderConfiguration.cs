using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Providers
{
    public interface IProviderConfiguration
    {
        string FullQualifiedTypeName { get; set; }
    }
}
