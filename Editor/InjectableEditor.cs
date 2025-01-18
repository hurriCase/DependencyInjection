using DependencyInjection.Runtime.InjectionBase;

namespace DependencyInjection.Editor
{
    /// <summary>
    /// Base class for Unity custom editors that support dependency injection.
    /// Automatically injects dependencies when the editor is enabled.
    /// </summary>
    internal abstract class InjectableEditor : UnityEditor.Editor
    {
        protected virtual void OnEnable()
        {
            DependencyInjector.InjectDependencies(this);
        }
    }
}