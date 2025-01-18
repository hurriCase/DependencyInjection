using System;

namespace DependencyInjection.Runtime.InjectionBase
{
    /// <summary>
    /// Interface for registering services in the dependency injection container.
    /// </summary>
    public interface IServiceCollection
    {
        /// <summary>
        /// Registers a singleton instance of a service.
        /// </summary>
        void RegisterSingleton<T>(T instance) where T : class;

        /// <summary>
        /// Registers a factory method for creating service instances.
        /// </summary>
        void Register<T>(Func<T> factory) where T : class;
    }
}