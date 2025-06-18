using System;
using JetBrains.Annotations;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
[MeansImplicitUse]
public class InjectAttribute : Attribute
{
    public string ID;
    public bool Optional;
}