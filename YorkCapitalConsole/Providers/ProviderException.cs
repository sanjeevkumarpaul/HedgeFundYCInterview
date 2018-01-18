using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Providers
{
    public class ProviderException : Exception
    {
        public ProviderException(string message) : base(message) { }
    }
}
