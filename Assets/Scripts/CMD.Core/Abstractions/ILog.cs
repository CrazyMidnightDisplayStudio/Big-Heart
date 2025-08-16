using System;

namespace CMD.Core
{
    public interface ILog
    {
        void Info(string msg);
        void Warn(string msg);
        void Error(string msg);
        void Exception(Exception ex, string msg = null);
    }
}
