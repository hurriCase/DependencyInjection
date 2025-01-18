using System;

namespace DependencyInjection.Runtime.InjectionBase
{
    /// <summary>
    /// Attribute that marks a field for dependency injection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class InjectAttribute : Attribute { }
}