using System;

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
