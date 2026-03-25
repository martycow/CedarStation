using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CedarStation.Helpers
{
    public class Logger : ILogger
    {
        private static readonly Dictionary<LogType, (string tagHex, string messageHex)> Colors =
            new Dictionary<LogType, Color>
            {
                { LogType.Default, Color.white },
                { LogType.Container, Color.cyan }, 
                { LogType.Input, Color.yellow }, 
                { LogType.Scene, new Color(0.7f, 0.5f, 1.0f) }, 
                { LogType.Audio, new Color(0.9f, 0.7f, 0.4f) }, 
                { LogType.Inventory, new Color(0.9f, 0.6f, 0.3f) }, 
                { LogType.Dialogue, new Color(0.4f, 0.9f, 0.5f) }, 
                { LogType.Radio, new Color(0.4f, 0.7f, 1.0f) },
            }
            .ToDictionary(
                kvp => kvp.Key,
                kvp => (ToHex(kvp.Value), ToHex(Darken(kvp.Value))));
        
        private static readonly HashSet<LogType> DisabledTypes = new();
        
        public void DisableType(LogType type)
        {
            DisabledTypes.Add(type);
        }

        public void EnableType(LogType type)
        {
            DisabledTypes.Remove(type);
        }
        
        public void Info(string infoMessage, LogType logType = LogType.Default)
        {
#if UNITY_EDITOR || DEBUG_BUILD
            if (DisabledTypes.Contains(logType))
                return;
            
            var str = BuildString(infoMessage, logType);
            Debug.Log(str);
#endif
        }
        
        public void Warn(string warningMessage, LogType logType = LogType.Default)
        {
#if UNITY_EDITOR || DEBUG_BUILD
            if (DisabledTypes.Contains(logType))
                return;
            
            var str = BuildString(warningMessage, logType);
            Debug.LogWarning(str);
#endif
        }
        
        public void Error(string errorMessage, LogType logType = LogType.Default)
        {
            if (DisabledTypes.Contains(logType))
                return;
            
            var str = BuildString(errorMessage, logType);
            Debug.LogError(str);
        }

        private static string BuildString(string message, LogType logType)
        {
            if (!Colors.TryGetValue(logType, out var colors))
                return $"[{logType}] {message}";
            
            var builder = MainThreadBuilder.Get();
            
            builder.AppendFormat("<color=#{0}>[{1}]</color>", colors.tagHex, logType);
            builder.Append(" ");
            builder.AppendFormat("<color=#{0}>{1}</color>", colors.messageHex, message);
            
            return builder.ToString();

        }

        private static Color Darken(Color c, float factor = 0.75f)
        {
            return new Color(c.r * factor, c.g * factor, c.b * factor);
        }

        private static string ToHex(Color c)
        {
            return ColorUtility.ToHtmlStringRGB(c);
        }
    }
}