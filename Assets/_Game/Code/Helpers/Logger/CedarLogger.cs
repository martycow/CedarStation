using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CedarStation.Helpers
{
    public sealed class CedarLogger : ICedarLogger
    {
        private enum OperationResult
        {
            Success = 0,
            Fail = 1,
        }
        
        private struct ColorScheme
        {
            public readonly string PrimaryColorHex;
            public readonly string SecondaryColorHex;
            public readonly string SuccessColorHex;
            public readonly string FailColorHex;

            public ColorScheme(string primaryColor, string secondaryColor, string successColor, string failColor)
            {
                PrimaryColorHex = primaryColor;
                SecondaryColorHex = secondaryColor;
                SuccessColorHex = successColor;
                FailColorHex = failColor;
            }
        }
        
        private static readonly Dictionary<LogTag, Color> LogColors = new()
        {
            { LogTag.Default, Color.white },
            { LogTag.Container, Color.cyan },
            { LogTag.Input, Color.yellow },
            { LogTag.Scene, new Color(0.7f, 0.5f, 1.0f) },
            { LogTag.Audio, new Color(0.9f, 0.7f, 0.4f) },
            { LogTag.Inventory, new Color(0.9f, 0.6f, 0.3f) },
            { LogTag.Dialogue, new Color(0.4f, 0.9f, 0.5f) },
            { LogTag.Radio, new Color(0.4f, 0.7f, 1.0f) },
        };

        private static readonly Dictionary<LogTag, ColorScheme> ColorSchemes =
            LogColors.ToDictionary(
                kvp => kvp.Key,
                kvp =>
                {
                    var primaryColor = ColorUtility.ToHtmlStringRGB(kvp.Value);
                    var secondaryColor = ColorUtility.ToHtmlStringRGB(Darken(kvp.Value));
                    var successColor = ColorUtility.ToHtmlStringRGB(Color.lawnGreen);
                    var failColor = ColorUtility.ToHtmlStringRGB(Color.indianRed);
                    return new ColorScheme(primaryColor, secondaryColor, successColor, failColor);
                });
        
        private static readonly HashSet<LogTag> DisabledTypes = new();
        
        public void EnableAll()
        {
            DisabledTypes.Clear();
        }
        
        public void EnableType(LogTag tag)
        {
            DisabledTypes.Remove(tag);
        }
        
        public void DisableType(LogTag tag)
        {
            DisabledTypes.Add(tag);
        }
        
        public void DisableAllExceptOne(LogTag tag)
        {
            DisabledTypes.Clear();
            foreach (LogTag t in System.Enum.GetValues(typeof(LogTag)))
            {
                if (t == tag)
                    continue;
                
                DisabledTypes.Add(t);
            }
        }
        
        public void Info(LogTag logTag, string message)
        {
#if UNITY_EDITOR || DEBUG_BUILD
            if (DisabledTypes.Contains(logTag))
                return;
            
            var str = BuildString(logTag, message);
            Debug.Log(str);
#endif
        }
        
        public void Warn(LogTag logTag, string warningMessage)
        {
#if UNITY_EDITOR || DEBUG_BUILD
            if (DisabledTypes.Contains(logTag))
                return;
            
            var str = BuildString(logTag, warningMessage);
            Debug.LogWarning(str);
#endif
        }
        
        public void Error(LogTag logTag, string errorMessage)
        {
            if (DisabledTypes.Contains(logTag))
                return;
            
            var str = BuildString(logTag, errorMessage);
            Debug.LogError(str);
        }
        
        public void Success(LogTag logTag, string message)
        {
#if UNITY_EDITOR || DEBUG_BUILD
            if (DisabledTypes.Contains(logTag))
                return;

            var str = BuildOperationResultString(logTag, OperationResult.Success, message);
            Debug.Log(str);
#endif
        }
        
        public void Fail(LogTag logTag, string message)
        {
#if UNITY_EDITOR || DEBUG_BUILD
            if (DisabledTypes.Contains(logTag))
                return;

            var str = BuildOperationResultString(logTag, OperationResult.Fail, message);
            Debug.Log(str);
#endif
        }

        private static string BuildString(LogTag primaryTag, string message)
        {
            if (ColorSchemes.TryGetValue(primaryTag, out var colorScheme) == false)
                return $"[{primaryTag}] {message}";
            
            var builder = MainThreadBuilder.Get();
            builder.AppendFormat("<color=#{0}>[{1}]</color> ", colorScheme.PrimaryColorHex, primaryTag);
            builder.AppendFormat("<color=#{0}>{1}</color>", colorScheme.SecondaryColorHex, message);
            return builder.ToString();
        }

        private static string BuildOperationResultString(LogTag tag, OperationResult result, string message)
        {
            if (ColorSchemes.TryGetValue(tag, out var colorScheme) == false)
                return $"[{tag}] [{result}] {message}";

            var builder = MainThreadBuilder.Get();
            
            //"[Tag]"
            builder.AppendFormat("<color=#{0}><b>", colorScheme.PrimaryColorHex);
            builder.AppendFormat("[{0}]</b></color>", tag);
            
            //" [Result]"
            builder.Append(' ');
            builder.AppendFormat("<color=#{0}>", result == OperationResult.Success ? colorScheme.SuccessColorHex : colorScheme.FailColorHex);
            builder.AppendFormat("[{0}]", result);
            
            // " Message"
            builder.Append(' ');
            builder.AppendFormat("{0}</color>", message);
            return builder.ToString();
        }

        private static Color Darken(Color c, float factor = 0.75f)
        {
            return new Color(c.r * factor, c.g * factor, c.b * factor);
        }
    }
}