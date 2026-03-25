using System;

namespace CedarStation.Helpers
{
    public class ThrowingLogger : NullLogger
    {
        public override void Error(string message, LogType type = LogType.Default)
        {
            throw new Exception($"[{type}] [FATAL ERROR] {message}");
        }
    }
}