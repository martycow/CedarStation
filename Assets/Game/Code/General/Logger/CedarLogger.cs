using System.Collections.Generic;
using Cedar.Core;
using UnityEngine;

namespace Game.General
{
    public sealed class CedarLogger : ICedarLogger
    {
        private static readonly HashSet<SystemTag> DisabledTypes = new();
        
        private readonly LoggerSettings _settings; 

        public CedarLogger(LoggerSettings settings)
        {
            _settings = settings;
        }

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

            var primaryColor = _settings.GetPrimaryColor(systemTag);
            var secondaryColor = Utilities.Colors.Darken(primaryColor);
            
            var str = BuildString(systemTag, message, primaryColor, secondaryColor);
            Debug.Log(str);
#endif
        }
        
        public void Warn(SystemTag systemTag, string warningMessage)
        {
#if UNITY_EDITOR || DEBUG_BUILD
            if (DisabledTypes.Contains(systemTag))
                return;

            var primaryColor = _settings.WarnColor;
            var secondaryColor = Utilities.Colors.Darken(primaryColor);
            
            var str = BuildString(systemTag, warningMessage, primaryColor, secondaryColor);
            Debug.LogWarning(str);
#endif
        }
        
        public void Error(SystemTag systemTag, string errorMessage)
        {
            if (DisabledTypes.Contains(systemTag))
                return;

            var primaryColor = _settings.ErrorColor;
            var secondaryColor = Utilities.Colors.Darken(primaryColor);
            
            var str = BuildString(systemTag, errorMessage, primaryColor, secondaryColor);
            Debug.LogError(str);
        }
        
        public void Success(SystemTag systemTag, string message)
        {
#if UNITY_EDITOR || DEBUG_BUILD
            if (DisabledTypes.Contains(systemTag))
                return;

            var primaryColor = _settings.GetPrimaryColor(systemTag);
            var secondaryColor = Utilities.Colors.Darken(primaryColor);
            var accentColor = _settings.SuccessColor;
            
            var str = BuildOperationResultString(systemTag, OperationResult.Success, message, primaryColor, secondaryColor, accentColor);
            Debug.Log(str);
#endif
        }
        
        public void Fail(SystemTag systemTag, string message)
        {
#if UNITY_EDITOR || DEBUG_BUILD
            if (DisabledTypes.Contains(systemTag))
                return;
            
            var primaryColor = _settings.GetPrimaryColor(systemTag);
            var secondaryColor = Utilities.Colors.Darken(primaryColor);
            var accentColor = _settings.FailColor;

            var str = BuildOperationResultString(systemTag, OperationResult.Fail, message, primaryColor, secondaryColor, accentColor);
            Debug.Log(str);
#endif
        }

        public void Line(int fillWidth = 0, char fill = ' ')
        {
#if UNITY_EDITOR || DEBUG_BUILD
            var line = new string(fill, fillWidth);
            Debug.Log(line);
#endif
        }

        private static string BuildString(SystemTag systemTag, string message, Color primaryColor, Color secondaryColor)
        {
            var primaryHex = ColorUtility.ToHtmlStringRGB(primaryColor);
            var secondaryHex = ColorUtility.ToHtmlStringRGB(secondaryColor);
            
            var builder = MainThreadBuilder.Get();
            builder.Append("<b>");
            builder.AppendFormat("<color=#{0}>[{1}]</color>", primaryHex, systemTag);
            builder.Append("</b>");
            builder.Append(' ');
            builder.AppendFormat("<color=#{0}>{1}</color>", secondaryHex, message);
            return builder.ToString();
        }

        private static string BuildOperationResultString(SystemTag systemTag, OperationResult result, string message, Color primaryColor, Color secondaryColor, Color accentColor)
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
            builder.AppendFormat("[{0}]</color>", systemTag);
            
            // " Message"
            builder.Append(' ');
            builder.AppendFormat("<color=#{0}>", secondaryHex);
            builder.AppendFormat("{0}</color>", message);
            
            builder.Append("</b>");
            
            return builder.ToString();
        }
    }
}