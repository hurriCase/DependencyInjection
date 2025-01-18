# Unity Custom DI Package

A dependency injection system for Unity projects. Uses attribute-based field injection to reduce boilerplate code and make dependencies explicit.

## Core Components

### DependencyInjector
Performs the actual injection of dependencies into fields marked with [Inject]. Uses reflection to find injectable fields and caches them for performance. You usually won't call this directly since the base classes handle injection timing.

```csharp
// Manual injection if needed
DependencyInjector.InjectDependencies(target);
```

### DIContainer
Central registry of all services. Handles two types of registrations:
- Singletons: Same instance used everywhere
- Factory registrations: New instance created each time

```csharp
// Register a singleton when you want the same instance everywhere
DIContainer.RegisterSingleton<IGameSettings>(gameSettings);

// Register a factory when each class should get its own instance
DIContainer.Register<IDataService>(() => new DataService());

// Get an instance (will use factory if registered that way)
var service = DIContainer.Resolve<T>();

// Clear registrations (useful when changing scenes)
DIContainer.Clear();  // Clear everything
DIContainer.ClearSingletonDependency<T>();  // Clear one type
```

## Base Classes
The base classes handle injection timing automatically. Choose the right one based on what you're making:

### Injectable
For regular C# classes. Injection happens in constructor:
```csharp
public class DataManager : Injectable 
{
    [Inject] private IDataService _dataService; // Injected when constructed
    
    public void SaveData() {
        _dataService.Save();
    }
}
```

### InjectableBehaviour
For MonoBehaviours. Injection happens in Awake before any other initialization:
```csharp
public class GameManager : InjectableBehaviour
{
    [Inject] private IDataService _dataService; // Injected in Awake
    
    protected override void Awake()
    {
        base.Awake(); // Must call base.Awake() first for injection
        // Your Awake code here
    }
}
```

### InjectableEditor
For custom Unity inspectors. Injection happens in OnEnable:
```csharp
public class GameManagerEditor : InjectableEditor
{
    [Inject] private IEditorDataService _editorService; // Injected in OnEnable
    
    protected override void OnEnable()
    {
        base.OnEnable(); // Must call base.OnEnable() first
        // Your OnEnable code here
    }
}
```

### InjectableEditorWindow
For editor windows. Injection happens in OnEnable:
```csharp
public class DataWindow : InjectableEditorWindow 
{
    [Inject] private IEditorDataService _service; // Injected in OnEnable
    
    protected override void OnEnable()
    {
        base.OnEnable(); // Must call base.OnEnable() first
        // Your OnEnable code here
    }
}
```

## Service Registration
ServiceRegisterBase helps organize service registration. Split between singletons and regular services for clarity:

```csharp
public class GameServices : ServiceRegisterBase
{
    // Regular services - new instance each time
    protected override void ConfigureServices(IServiceCollection services)
    {
        // Each class gets its own DataService
        services.Register<IDataService>(() => new DataService());
        
        // Could have configuration:
        services.Register<INetworkService>(() => {
            var service = new NetworkService();
            service.Initialize(settings);
            return service;
        });
    }

    // Singletons - same instance everywhere
    protected override void ConfigureSingletonServices(IServiceCollection services)
    {
        // Everyone shares the same GameState
        services.RegisterSingleton<IGameState>(GameState.Instance);
        services.RegisterSingleton<IGameSettings>(GameSettings.Instance);
    }
}
```

## IInjectable Interface
For classes that can't inherit from base classes but still need injection. You control when injection happens:

```csharp
public class SpecialCase : SomeOtherBaseClass, IInjectable
{
    [Inject] private IService _service;

    void InjectDependencies()
    {
        DependencyInjector.InjectDependencies(this);
    }
    
    void Initialize()
    {
        InjectDependencies(); // You decide when to inject
    }
}
```

## Common Issues

- If injection isn't working in MonoBehaviour, make sure you called base.Awake()
- If editor injection isn't working, make sure you called base.OnEnable()
- Factory registrations create new instances each time - use singleton if you need the same instance
- Register services early (like in a startup system) before any injection happens
- Clear the container when changing scenes to prevent memory leaks