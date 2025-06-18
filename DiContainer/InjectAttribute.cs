using System;
using JetBrains.Annotations;

namespace Whaledevelop.DiContainer
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
    [MeansImplicitUse]
    public class InjectAttribute : Attribute
    {
        public string ID;
        public bool Optional;
    }
}