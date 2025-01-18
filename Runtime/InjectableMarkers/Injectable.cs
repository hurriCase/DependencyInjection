using DependencyInjection.Runtime.InjectionBase;

namespace DependencyInjection.Runtime.InjectableMarkers
{
    /// <summary>
    /// Base class for objects that require dependency injection.
    /// Automatically injects dependencies in constructor.
    /// </summary>
    internal abstract class Injectable
    {
        protected Injectable() => DependencyInjector.InjectDependencies(this);
    }
}