using DependencyInjection.Runtime.InjectionBase;
using UnityEngine;

namespace DependencyInjection.Runtime.InjectableMarkers
{
    /// <summary>
    /// Base MonoBehaviour class that supports dependency injection.
    /// Automatically injects dependencies in Awake.
    /// </summary>
    internal abstract class InjectableBehaviour : MonoBehaviour
    {
        protected virtual void Awake()
        {
            DependencyInjector.InjectDependencies(this);
        }
    }
}