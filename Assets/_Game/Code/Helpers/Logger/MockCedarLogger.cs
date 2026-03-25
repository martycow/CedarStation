namespace CedarStation.Helpers
{
    public sealed class MockCedarLogger : ICedarLogger
    {
        public void EnableAll() { }
        public void EnableType(LogTag tag) { }
        public void DisableType(LogTag tag) { }
        public void DisableAllExceptOne(LogTag tag) { }

        public void Info(LogTag tag, string message) { }
        public void Warn(LogTag tag, string message) { }
        public void Error(LogTag tag, string message) { }
        public void Success(LogTag tag, string message) { }
        public void Fail(LogTag tag, string message) { }
    }
}