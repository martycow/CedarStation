using System.Collections.Generic;
using System.Linq;
using Cedar.Core;
using UnityEngine;

namespace Game.General
{
    public sealed class CedarLogger : ICedarLogger
    {
        private static readonly Dictionary<SystemTag, Color> LogColors = new()
        {
            { SystemTag.Application, Color.white },
            { SystemTag.Container, Color.bisque },
            { SystemTag.Input, Color.chocolate },
            { SystemTag.EventBus, Color.dodgerBlue },
            { SystemTag.Gameplay, Color.lawnGreen },
            { SystemTag.Audio, Color.aquamarine },
            { SystemTag.Inventory, Color.deepPink },
            { SystemTag.Dialogue, Color.darkSalmon },
            { SystemTag.UI, Color.cyan },
            { SystemTag.AI, Color.magenta },
        };

        private static readonly Dictionary<SystemTag, LoggerColorScheme> ColorSchemes =
            LogColors.ToDictionary(
                kvp => kvp.Key,
                kvp =>
                {
                    var primaryColor = ColorUtility.ToHtmlStringRGB(kvp.Value);
                    var secondaryColor = ColorUtility.ToHtmlStringRGB(Darken(kvp.Value));
                    var successColor = ColorUtility.ToHtmlStringRGB(Color.chartreuse);
                    var failColor = ColorUtility.ToHtmlStringRGB(Color.orangeRed);
                    return new LoggerColorScheme(primaryColor, secondaryColor, successColor, failColor);
                });
        
        private static readonly HashSet<SystemTag> DisabledTypes = new();
        
        public void EnableAll()
        {
            DisabledTypes.Clear();
        }
        
        public void EnableType(SystemTag tag)
        {
            DisabledTypes.Remove(tag);
        }
        
        public void DisableType(SystemTag tag)
        {
            DisabledTypes.Add(tag);
        }
        
        public void DisableAllExceptOne(SystemTag tag)
        {
            DisabledTypes.Clear();
            foreach (SystemTag t in System.Enum.GetValues(typeof(SystemTag)))
            {
                if (t == tag)
                    continue;
                
                DisabledTypes.Add(t);
            }
        }
        
        public void Info(SystemTag systemTag, string message)
        {
#if UNITY_EDITOR || DEBUG_BUILD
            if (DisabledTypes.Contains(systemTag))
                return;
            
            var str = BuildString(systemTag, message);
            Debug.Log(str);
#endif
        }
        
        public void Warn(SystemTag systemTag, string warningMessage)
        {
#if UNITY_EDITOR || DEBUG_BUILD
            if (DisabledTypes.Contains(systemTag))
                return;
            
            var str = BuildString(systemTag, warningMessage);
            Debug.LogWarning(str);
#endif
        }
        
        public void Error(SystemTag systemTag, string errorMessage)
        {
            if (DisabledTypes.Contains(systemTag))
                return;
            
            var str = BuildString(systemTag, errorMessage);
            Debug.LogError(str);
        }
        
        public void Success(SystemTag systemTag, string message)
        {
#if UNITY_EDITOR || DEBUG_BUILD
            if (DisabledTypes.Contains(systemTag))
                return;

            var str = BuildOperationResultString(systemTag, OperationResult.Success, message);
            Debug.Log(str);
#endif
        }
        
        public void Fail(SystemTag systemTag, string message)
        {
#if UNITY_EDITOR || DEBUG_BUILD
            if (DisabledTypes.Contains(systemTag))
                return;

            var str = BuildOperationResultString(systemTag, OperationResult.Fail, message);
            Debug.Log(str);
#endif
        }

        private static string BuildString(SystemTag primaryTag, string message)
        {
            if (ColorSchemes.TryGetValue(primaryTag, out var colorScheme) == false)
                return $"[{primaryTag}] {message}";
            
            var builder = MainThreadBuilder.Get();
            builder.Append("<b>");
            builder.AppendFormat("<color=#{0}>[{1}]</color> ", colorScheme.PrimaryColorHex, primaryTag);
            builder.AppendFormat("<color=#{0}>{1}</color>", colorScheme.SecondaryColorHex, message);
            builder.Append("</b>");
            return builder.ToString();
        }

        private static string BuildOperationResultString(SystemTag tag, OperationResult result, string message)
        {
            if (ColorSchemes.TryGetValue(tag, out var colorScheme) == false)
                return $"[{tag}] [{result}] {message}";

            var builder = MainThreadBuilder.Get();
            
            builder.Append("<b>");
            
            //"[Tag]"
            builder.AppendFormat("<color=#{0}>", colorScheme.PrimaryColorHex);
            builder.AppendFormat("[{0}]</color>", tag);
            
            //" [Result]"
            builder.Append(' ');
            builder.AppendFormat("<color=#{0}>", result == OperationResult.Success ? colorScheme.SuccessColorHex : colorScheme.FailColorHex);
            builder.AppendFormat("[{0}]", result);
            
            // " Message"
            builder.Append(' ');
            builder.AppendFormat("{0}</color>", message);
            
            builder.Append("</b>");
            
            return builder.ToString();
        }

        private static Color Darken(Color c, float factor = 0.75f)
        {
            return new Color(c.r * factor, c.g * factor, c.b * factor);
        }
    }
}