namespace DependencyInjection.Runtime.InjectionBase
{
    /// <summary>
    /// Base class for registering services with the dependency injection container.
    /// Provides hooks for configuring both regular and singleton services.
    /// </summary>
    internal abstract class ServiceRegisterBase
    {
        /// <summary>
        /// Configures regular (non-singleton) services.
        /// Override this method to register services using the factory pattern.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        protected abstract void ConfigureServices(IServiceCollection services);

        /// <summary>
        /// Configures singleton services.
        /// Override this method to register singleton instances.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        protected abstract void ConfigureSingletonServices(IServiceCollection services);

        /// <summary>
        /// Registers all regular (non-singleton) services with the container.
        /// </summary>
        internal void RegisterServices()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
        }

        /// <summary>
        /// Registers all singleton instances with the container.
        /// </summary>
        internal void RegisterSingletonInstance()
        {
            var services = new ServiceCollection();
            ConfigureSingletonServices(services);
        }
    }
}