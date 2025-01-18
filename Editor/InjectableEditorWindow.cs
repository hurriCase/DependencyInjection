using DependencyInjection.Runtime.InjectionBase;
using UnityEditor;

namespace DependencyInjection.Editor
{
    /// <summary>
    /// Base class for Unity editor windows that support dependency injection.
    /// Automatically injects dependencies when the window is enabled.
    /// </summary>
    internal abstract class InjectableEditorWindow : EditorWindow
    {
        protected virtual void OnEnable()
        {
            DependencyInjector.InjectDependencies(this);
        }
    }
}