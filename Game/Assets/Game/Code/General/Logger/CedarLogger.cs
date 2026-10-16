using System.Collections.Generic;
using UnityEngine;

namespace Game.General
{
    public sealed class CedarLogger
    {
        private static readonly HashSet<LogTag> DisabledTypes = new();
        
        private readonly LoggerSettings _settings; 

        public CedarLogger(LoggerSettings settings)
        {
            _settings = settings;
        }

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

            var primaryColor = _settings.GetPrimaryColor(logTag);
            var secondaryColor = Utilities.Colors.Darken(primaryColor);
            
            var str = BuildString(logTag, message, primaryColor, secondaryColor);
            Debug.Log(str);
#endif
        }
        
        public void Warn(LogTag logTag, string warningMessage)
        {
#if UNITY_EDITOR || DEBUG_BUILD
            if (DisabledTypes.Contains(logTag))
                return;

            var primaryColor = _settings.WarnColor;
            var secondaryColor = Utilities.Colors.Darken(primaryColor);
            
            var str = BuildString(logTag, warningMessage, primaryColor, secondaryColor);
            Debug.LogWarning(str);
#endif
        }
        
        public void Error(LogTag logTag, string errorMessage)
        {
            if (DisabledTypes.Contains(logTag))
                return;

            var primaryColor = _settings.ErrorColor;
            var secondaryColor = Utilities.Colors.Darken(primaryColor);
            
            var str = BuildString(logTag, errorMessage, primaryColor, secondaryColor);
            Debug.LogError(str);
        }
        
        public void Success(LogTag logTag, string message)
        {
#if UNITY_EDITOR || DEBUG_BUILD
            if (DisabledTypes.Contains(logTag))
                return;

            var primaryColor = _settings.GetPrimaryColor(logTag);
            var secondaryColor = Utilities.Colors.Darken(primaryColor);
            var accentColor = _settings.SuccessColor;
            
            var str = BuildOperationResultString(logTag, OperationResult.Success, message, primaryColor, secondaryColor, accentColor);
            Debug.Log(str);
#endif
        }
        
        public void Fail(LogTag logTag, string message)
        {
#if UNITY_EDITOR || DEBUG_BUILD
            if (DisabledTypes.Contains(logTag))
                return;
            
            var primaryColor = _settings.GetPrimaryColor(logTag);
            var secondaryColor = Utilities.Colors.Darken(primaryColor);
            var accentColor = _settings.FailColor;

            var str = BuildOperationResultString(logTag, OperationResult.Fail, message, primaryColor, secondaryColor, accentColor);
            Debug.Log(str);
#endif
        }

        public void Line(int fillWidth = 50, char fill = '=')
        {
#if UNITY_EDITOR || DEBUG_BUILD
            var line = new string(fill, fillWidth);
            Debug.Log(line);
#endif
        }

        private static string BuildString(LogTag logTag, string message, Color primaryColor, Color secondaryColor)
        {
            var primaryHex = ColorUtility.ToHtmlStringRGB(primaryColor);
            var secondaryHex = ColorUtility.ToHtmlStringRGB(secondaryColor);
            
            var builder = MainThreadBuilder.Get();
            builder.Append("<b>");
            builder.AppendFormat("<color=#{0}>[{1}]</color>", primaryHex, logTag);
            builder.Append("</b>");
            builder.Append(' ');
            builder.AppendFormat("<color=#{0}>{1}</color>", secondaryHex, message);
            return builder.ToString();
        }

        private static string BuildOperationResultString(LogTag logTag, OperationResult result, string message, Color primaryColor, Color secondaryColor, Color accentColor)
        {
            var primaryHex = ColorUtility.ToHtmlStringRGB(primaryColor);
            var secondaryHex = ColorUtility.ToHtmlStringRGB(secondaryColor);
            var accentHex = ColorUtility.ToHtmlStringRGB(accentColor);
                
            var builder = MainThreadBuilder.Get();
            
            builder.Append("<b>");
            //"[Result]"
            builder.AppendFormat("<color=#{0}>", accentHex);
            builder.AppendFormat("[{0}]</color>", result);
            
            //" [SystemTag]"
            builder.Append(' ');
            builder.AppendFormat("<color=#{0}>", primaryHex);
            builder.AppendFormat("[{0}]</color>", logTag);
            
            // " Message"
            builder.Append(' ');
            builder.AppendFormat("<color=#{0}>", secondaryHex);
            builder.AppendFormat("{0}</color>", message);
            
            builder.Append("</b>");
            
            return builder.ToString();
        }
    }
}