namespace Cedar.Core
{
    public sealed class MockedCedarLogger : ICedarLogger
    {
        public void EnableAll() { }
        public void EnableType(SystemTag tag) { }
        public void DisableType(SystemTag tag) { }
        public void DisableAllExceptOne(SystemTag tag) { }

        public void Info(SystemTag tag, string message) { }
        public void Warn(SystemTag tag, string message) { }
        public void Error(SystemTag tag, string message) { }
        public void Success(SystemTag tag, string message) { }
        public void Fail(SystemTag tag, string message) { }
    }
}