using System;

namespace DependencyInjection.Runtime.InjectionBase.Service
{
    /// <summary>
    /// Registration metadata for a service
    /// </summary>
    internal sealed class ServiceRegistration
    {
        private readonly ServiceLifetime _lifetime;
        private readonly Func<object> _factory;
        private object _singletonInstance;

        internal ServiceRegistration(ServiceLifetime lifetime, Func<object> factory)
        {
            _lifetime = lifetime;
            _factory = factory;
        }

        internal object GetInstance()
        {
            if (_lifetime != ServiceLifetime.Singleton)
                return _factory();

            return _singletonInstance ?? (_singletonInstance = _factory());
        }
    }
}