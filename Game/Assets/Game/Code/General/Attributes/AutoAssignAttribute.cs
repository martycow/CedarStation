using System;
using UnityEngine;

namespace Game.General
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AutoAssignAttribute : PropertyAttribute
    {
        public readonly string NamePart;
        public readonly bool AutoFillOnEnable;

        public AutoAssignAttribute(
            string namePart = "",
            bool autoFillOnEnable = false)
        {
            NamePart = namePart;
            AutoFillOnEnable = autoFillOnEnable;
        }
    }
}