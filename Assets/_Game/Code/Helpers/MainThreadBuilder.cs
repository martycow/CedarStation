using System.Text;

namespace CedarStation.Helpers
{
    public static class MainThreadBuilder
    {
        private static readonly StringBuilder Builder = new();

        public static StringBuilder Get()
        {
            Builder.Clear();
            return Builder;
        }
    }
}