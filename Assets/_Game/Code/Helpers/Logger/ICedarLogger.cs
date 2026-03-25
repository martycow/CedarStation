namespace CedarStation.Helpers
{
    public interface ICedarLogger
    {
        void EnableAll();
        void EnableType(LogTag tag);
        void DisableType(LogTag tag);
        void DisableAllExceptOne(LogTag tag);
        
        void Info(LogTag tag, string message);
        void Warn(LogTag tag, string message);
        void Error(LogTag tag, string message);
        void Success(LogTag tag, string message);
        void Fail(LogTag tag, string message);
    }
}