namespace DependencyInjection.Runtime.InjectableMarkers
{
    /// <summary>
    /// Defines an interface for injection in classes that cannot inherit from the Injectable base class.
    /// Implementing classes must define an Inject method and ensure it is called during initialization.
    /// </summary>
    public interface IInjectable
    {
        void InjectDependencies();
    }
}