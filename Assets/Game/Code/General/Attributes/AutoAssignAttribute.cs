using System;
using UnityEngine;

namespace Game.General
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AutoAssignAttribute : PropertyAttribute
    {
        public readonly SearchScope Scope;
        public readonly bool AutoFillOnEnable;

        public AutoAssignAttribute(
            SearchScope searchScope = SearchScope.SelfThenChildrenThenParents,
            bool autoFillOnEnable = false)
        {
            Scope = searchScope;
            AutoFillOnEnable = autoFillOnEnable;
        }
    }

    public enum SearchScope
    {
        Self,
        SelfThenChildren,
        SelfThenParents,
        SelfThenChildrenThenParents
    }
}