using System;
using System.Collections.Generic;

namespace DependencyInjection.Runtime
{
    /// <summary>
    /// Provides a dependency injection container for managing service registrations and resolutions.
    /// </summary>
    public static class DIContainer
    {
        private static readonly Dictionary<Type, object> _services = new();
        private static readonly Dictionary<Type, Func<object>> _serviceFactories = new();

        /// <summary>
        /// Registers a singleton instance of a service.
        /// </summary>
        /// <typeparam name="T">The type of service to register.</typeparam>
        /// <param name="instance">The singleton instance to register.</param>
        public static void RegisterSingleton<T>(T instance)
        {
            var type = typeof(T);
            _services.TryAdd(type, instance);
        }

        /// <summary>
        /// Registers a factory method for creating service instances.
        /// </summary>
        /// <typeparam name="T">The type of service to register.</typeparam>
        /// <param name="factory">The factory method for creating service instances.</param>
        public static void Register<T>(Func<T> factory)
            => _serviceFactories[typeof(T)] = () => factory();

        /// <summary>
        /// Resolves a service instance by type.
        /// </summary>
        /// <param name="type">The type of service to resolve.</param>
        /// <returns>The resolved service instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the requested service type is not registered.</exception>
        public static object Resolve(Type type)
        {
            if (_services.TryGetValue(type, out var instance))
                return instance;

            if (_serviceFactories.TryGetValue(type, out var factory) is false)
                throw new InvalidOperationException($"[DIContainer::Resolve] No registration for type {type.Name}");

            var resolvedInstance = factory();
            _services[type] = resolvedInstance;
            return resolvedInstance;
        }

        /// <summary>
        /// Resolves a service instance by type.
        /// </summary>
        /// <typeparam name="T">The type of service to resolve.</typeparam>
        /// <returns>The resolved service instance.</returns>
        public static T Resolve<T>() => (T)Resolve(typeof(T));

        /// <summary>
        /// Clears a singleton dependency from the container.
        /// </summary>
        /// <typeparam name="T">The type of singleton service to clear.</typeparam>
        public static void ClearSingletonDependency<T>()
        {
            _services.Remove(typeof(T));
        }

        /// <summary>
        /// Clears all registered services and factories from the container.
        /// </summary>
        public static void Clear()
        {
            _services.Clear();
            _serviceFactories.Clear();
        }
    }
}