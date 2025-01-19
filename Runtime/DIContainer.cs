using System;
using System.Collections.Generic;
using DependencyInjection.Runtime.InjectionBase.Service;
using UnityEngine;

namespace DependencyInjection.Runtime
{
    /// <summary>
    /// Provides a dependency injection container with lifetime management.
    /// Supports registration and resolution of both singleton and transient services.
    /// </summary>
    public static class DIContainer
    {
        private static readonly Dictionary<Type, ServiceRegistration> _registrations = new();

        /// <summary>
        /// Registers an existing instance as a singleton service.
        /// </summary>
        /// <typeparam name="TService">The type to register the instance as.</typeparam>
        /// <param name="instance">The singleton instance.</param>
        public static void RegisterSingleton<TService>(TService instance) where TService : class
        {
            var type = typeof(TService);
            var registration = new ServiceRegistration(
                ServiceLifetime.Singleton,
                () => instance
            );
            _registrations[type] = registration;
        }

        /// <summary>
        /// Registers a type with its implementation as a singleton service.
        /// </summary>
        /// <typeparam name="TService">The service interface or base type.</typeparam>
        /// <typeparam name="TImplementation">The concrete implementation type.</typeparam>
        public static void RegisterSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class
        {
            Register<TService, TImplementation>(ServiceLifetime.Singleton);
        }

        /// <summary>
        /// Registers a type with its implementation as a transient service.
        /// </summary>
        /// <typeparam name="TService">The service interface or base type.</typeparam>
        /// <typeparam name="TImplementation">The concrete implementation type.</typeparam>
        public static void RegisterTransient<TService, TImplementation>()
            where TService : class
            where TImplementation : class
        {
            Register<TService, TImplementation>(ServiceLifetime.Transient);
        }

        /// <summary>
        /// Registers a factory method for creating a singleton service.
        /// </summary>
        /// <typeparam name="TService">The type of service to register.</typeparam>
        /// <param name="factory">The factory method to create the instance.</param>
        public static void RegisterSingleton<TService>(Func<TService> factory) where TService : class
        {
            Register(ServiceLifetime.Singleton, factory);
        }

        /// <summary>
        /// Registers a factory method for creating transient services.
        /// </summary>
        /// <typeparam name="TService">The type of service to register.</typeparam>
        /// <param name="factory">The factory method to create instances.</param>
        public static void RegisterTransient<TService>(Func<TService> factory) where TService : class
        {
            Register(ServiceLifetime.Transient, factory);
        }

        /// <summary>
        /// Resolves a service instance by type.
        /// </summary>
        /// <param name="serviceType">The type of service to resolve.</param>
        /// <returns>The resolved service instance.</returns>
        public static object Resolve(Type serviceType)
        {
            if (_registrations.TryGetValue(serviceType, out var registration) is false)
                throw new InvalidOperationException($"[DIContainer::Resolve] No registration for type {serviceType.Name}");

            try
            {
                return registration.GetInstance();
            }
            catch (Exception e)
            {
                Debug.LogError($"[DIContainer::Resolve] Failed to create instance of {serviceType.Name}: {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Resolves a service instance by type.
        /// </summary>
        public static T Resolve<T>() where T : class => (T)Resolve(typeof(T));

        /// <summary>
        /// Clears a singleton dependency from the container.
        /// </summary>
        public static void ClearSingletonDependency<T>() where T : class
        {
            _registrations.Remove(typeof(T));
        }

        /// <summary>
        /// Clears all registered services from the container.
        /// </summary>
        public static void Clear()
        {
            _registrations.Clear();
        }

        private static void Register<TService>(ServiceLifetime lifetime, Func<TService> factory) where TService : class
        {
            var type = typeof(TService);
            var registration = new ServiceRegistration(
                lifetime,
                factory
            );
            _registrations[type] = registration;
        }

        private static void Register<TService, TImplementation>(ServiceLifetime lifetime)
            where TService : class
            where TImplementation : class
        {
            var type = typeof(TService);
            var registration = new ServiceRegistration(
                lifetime,
                () => Activator.CreateInstance<TImplementation>()
            );
            _registrations[type] = registration;
        }
    }
}