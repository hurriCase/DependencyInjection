namespace DependencyInjection.Runtime.InjectionBase.Service
{
    /// <summary>
    /// Base class for registering services with the dependency injection container.
    /// Separates registration into static (type-based) and runtime (instance-based) services.
    /// </summary>
    /// <remarks>
    /// The separation addresses different registration needs:
    /// - Static services: Type registrations that work in both editor and play mode
    /// - Runtime services: Instance-based registrations that require existing objects
    /// </remarks>
    public abstract class ServiceRegisterBase
    {
        /// <summary>
        /// Configures type-based service registrations that work in both editor and play mode.
        /// Use this for registering services that don't require existing object instances.
        /// </summary>
        protected abstract void ConfigureStaticServices();

        /// <summary>
        /// Configures instance-based registrations for runtime-only services.
        /// </summary>
        protected abstract void ConfigureRuntimeServices();

        /// <summary>
        /// Registers all type-based services. Safe to call in both editor and play mode.
        /// </summary>
        public void RegisterStaticServices() => ConfigureStaticServices();

        /// <summary>
        /// Registers instance-based services. Only call in play mode contexts where instances exist.
        /// </summary>
        public void RegisterRuntimeServices() => ConfigureRuntimeServices();
    }
}