namespace DependencyInjection.Runtime.InjectionBase.Service
{
    /// <summary>
    /// Defines how a service's lifetime should be managed.
    /// </summary>
    internal enum ServiceLifetime
    {
        /// <summary>
        /// A new instance is created for each resolution
        /// </summary>
        Transient,

        /// <summary>
        /// One instance is shared across all resolutions
        /// </summary>
        Singleton
    }
}