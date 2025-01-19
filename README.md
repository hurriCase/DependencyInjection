# Unity Custom DI Package

A dependency injection system for Unity projects that provides attribute-based field injection to reduce boilerplate code and make dependencies explicit.

## Core Components

### DependencyInjector
Handles the injection of dependencies into fields marked with [Inject]. Uses reflection with caching for performance. Base classes manage injection timing automatically.

```csharp
// Manual injection if needed
DependencyInjector.InjectDependencies(target);
```

### DIContainer
Manages service registration and resolution with support for singleton and transient lifetimes:

```csharp
// Register type implementations
DIContainer.RegisterSingleton<IService, ServiceImpl>();
DIContainer.RegisterTransient<IService, ServiceImpl>();

// Register with factory methods
DIContainer.RegisterSingleton<IService>(() => new ServiceImpl());
DIContainer.RegisterTransient<IService>(() => new ServiceImpl());

// Register existing instances
DIContainer.RegisterSingleton<IService>(existingInstance);

// Resolve services
var service = DIContainer.Resolve<IService>();

// Container management
DIContainer.Clear();  // Clear all registrations
DIContainer.ClearSingletonDependency<IService>();  // Clear specific registration
```

## Base Classes

### Injectable
Base class for regular C# classes, performs injection in constructor:
```csharp
public class DataManager : Injectable 
{
    [Inject] private IDataService _dataService; 
}
```

### InjectableBehaviour
Base class for MonoBehaviours, performs injection in Awake:
```csharp
public class GameManager : InjectableBehaviour
{
    [Inject] private IDataService _dataService;
    
    protected override void Awake()
    {
        base.Awake(); // Required for injection
        // Your Awake code
    }
}
```

### InjectableEditor
Base class for Unity custom inspectors:
```csharp
public class GameManagerEditor : InjectableEditor
{
    [Inject] private IEditorService _editorService;
    
    protected override void OnEnable()
    {
        base.OnEnable(); // Required for injection
        // Your OnEnable code
    }
}
```

### InjectableEditorWindow
Base class for editor windows:
```csharp
public class DataWindow : InjectableEditorWindow 
{
    [Inject] private IEditorService _editorService;
    
    protected override void OnEnable()
    {
        base.OnEnable(); // Required for injection
        // Your OnEnable code
    }
}
```

## Service Registration
Use ServiceRegisterBase to organize registrations with separate methods for static (type-based) and runtime (instance-based) services:

```csharp
public class GameServiceRegister : ServiceRegisterBase
{
    // Type-based registrations that work in both editor and play mode
    protected override void ConfigureStaticServices()
    {
        // These work everywhere since they don't need existing instances
        DIContainer.RegisterTransient<IDataService, DataService>();
        DIContainer.RegisterSingleton<IGameState, GameState>();
        DIContainer.RegisterSingleton<IGameService>(() => new GameService());
    }

    // Instance-based registrations that require existing objects
    protected override void ConfigureRuntimeServices()
    {
        // These only work when the instances exist (play mode)
        DIContainer.RegisterSingleton<IGameManager>(existingGameManager);
        DIContainer.RegisterSingleton<IUISystem>(uiSystem);
    }
}
```

## IInjectable Interface
For classes that can't inherit from the base classes:

```csharp
public class SpecialCase : SomeOtherBaseClass, IInjectable
{
    [Inject] private IService _service;

    public void InjectDependencies()
    {
        DependencyInjector.InjectDependencies(this);
    }
}
```

## Best Practices

1. Service Registration
    - Use `ConfigureStaticServices()` for type-based registrations that work everywhere
    - Use `ConfigureRuntimeServices()` for instance-based registrations that need existing objects
    - Register services before any injection occurs

2. Injection Order
    - Always call base.Awake() or base.OnEnable() first in derived classes
    - Register services before attempting injection
    - Clear container when appropriate to prevent memory leaks

3. Lifetime Management
    - Use transient registration when each consumer needs its own instance
    - Use singleton registration when sharing an instance across multiple consumers
    - Be careful with instance-based registrations in editor code

## Common Issues

1. Missing Dependencies
    - Ensure services are registered before injection occurs
    - Check that base.Awake() or base.OnEnable() is called in derived classes
    - Verify service registration method matches resolution needs (singleton vs transient)

2. Runtime Issues
    - Instance-based registrations require the instance to exist
    - Clear container when changing scenes to prevent stale references
    - Use appropriate lifetime scope for your services

3. Editor Integration
    - Editor tools may need special handling for instance-based dependencies
    - Consider using type-based registration for editor services when possible
    - Be cautious with runtime dependencies in editor code