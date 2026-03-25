namespace CedarStation.Helpers
{
    public class NullLogger : ILogger
    {
        public void Info(string message, LogType type = LogType.Default) { }
        public void Warn(string message, LogType type = LogType.Default) { }
        public virtual void Error(string message, LogType type = LogType.Default) { }
    }
}