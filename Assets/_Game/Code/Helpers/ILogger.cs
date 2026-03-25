namespace CedarStation.Helpers
{
    public interface ILogger
    {
        void Info(string message, LogType type = LogType.Default);
        void Warn(string message, LogType type = LogType.Default);
        void Error(string message, LogType type = LogType.Default);
    }
}