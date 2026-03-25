using System;

namespace CedarStation.Core.DI
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class Inject : Attribute { }
}