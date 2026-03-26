namespace Game.General
{
    public struct LoggerColorScheme
    {
        public readonly string PrimaryColorHex;
        public readonly string SecondaryColorHex;
        public readonly string SuccessColorHex;
        public readonly string FailColorHex;

        public LoggerColorScheme(string primaryColor, string secondaryColor, string successColor, string failColor)
        {
            PrimaryColorHex = primaryColor;
            SecondaryColorHex = secondaryColor;
            SuccessColorHex = successColor;
            FailColorHex = failColor;
        }
    }
}