using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceArchitecture
{
    public interface ILogger
    {
        void Info(string text);
        void Warning(string text);
        void Exception(Exception e);
        void All(string text, Exception e = null);
    }
}
