namespace Cedar.Core
{
    public interface ICedarLogger
    {
        void EnableAll();
        void EnableType(SystemTag tag);
        void DisableType(SystemTag tag);
        void DisableAllExceptOne(SystemTag tag);
        
        void Info(SystemTag tag, string message);
        void Warn(SystemTag tag, string message);
        void Error(SystemTag tag, string message);
        void Success(SystemTag tag, string message);
        void Fail(SystemTag tag, string message);
        void Line(int fillWidth = 0, char fill = ' ');
    }
}