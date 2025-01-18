using System;

namespace DependencyInjection.Runtime.InjectionBase
{
    public sealed class ServiceCollection : IServiceCollection
    {
        public void RegisterSingleton<T>(T instance) where T : class =>
            DIContainer.RegisterSingleton(instance);

        public void Register<T>(Func<T> factory) where T : class =>
            DIContainer.Register(factory);
    }
}