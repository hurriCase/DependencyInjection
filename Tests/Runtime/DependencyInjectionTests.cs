using System;
using NUnit.Framework;
using UnityEngine;
using DependencyInjection.Runtime;
using DependencyInjection.Runtime.InjectionBase;
using DependencyInjection.Runtime.InjectionBase.Service;
using DependencyInjection.Runtime.InjectableMarkers;

namespace DependencyInjection.Tests
{
    internal sealed class DependencyInjectionTests
    {
        private const string SingletonErrorMessage = "Singleton service should reuse instance";
        private const string TransientErrorMessage = "Transient service should create new instance";
        private const string ResolveErrorMessage = "The type of the resolved service doesn't match the registered type";

        [SetUp]
        public void Setup()
        {
            DIContainer.Clear();
        }

        [Test]
        public void RegisterSingleton_WithInstance_RegistersAndResolvesSameInstance()
        {
            // Arrange
            var manager = new GameManager();

            // Act
            DIContainer.RegisterSingleton<IGameManager>(manager);
            var resolved1 = DIContainer.Resolve<IGameManager>();
            var resolved2 = DIContainer.Resolve<IGameManager>();

            // Assert
            Assert.That(resolved1, Is.SameAs(manager), SingletonErrorMessage);
            Assert.That(resolved2, Is.SameAs(manager), SingletonErrorMessage);
            Assert.That(resolved1, Is.SameAs(resolved2), SingletonErrorMessage);
        }

        [Test]
        public void RegisterSingleton_WithType_CreatesAndReusesSingleInstance()
        {
            // Act
            DIContainer.RegisterSingleton<IScoreSystem, ScoreSystem>();
            var resolved1 = DIContainer.Resolve<IScoreSystem>();
            var resolved2 = DIContainer.Resolve<IScoreSystem>();

            // Assert
            Assert.That(resolved1, Is.Not.Null);
            Assert.That(resolved2, Is.Not.Null);
            Assert.That(resolved1, Is.SameAs(resolved2), SingletonErrorMessage);
        }

        [Test]
        public void RegisterTransient_WithType_CreatesNewInstanceEachTime()
        {
            // Act
            DIContainer.RegisterTransient<IGameService, GameService>();
            var resolved1 = DIContainer.Resolve<IGameService>();
            var resolved2 = DIContainer.Resolve<IGameService>();

            // Assert
            Assert.That(resolved1, Is.Not.Null);
            Assert.That(resolved2, Is.Not.Null);
            Assert.That(resolved1, Is.Not.SameAs(resolved2));
        }

        [Test]
        public void ServiceRegisterBase_RegistersServicesCorrectly()
        {
            // Arrange
            var runtimeRegister = new RuntimeServiceRegister();

            // Act
            runtimeRegister.RegisterStaticServices();
            runtimeRegister.RegisterRuntimeServices();

            // Assert
            var gameService = DIContainer.Resolve<IGameService>();
            var enemyFactory = DIContainer.Resolve<IEnemyFactory>();
            var scoreSystem = DIContainer.Resolve<IScoreSystem>();
            var gameManager = DIContainer.Resolve<IGameManager>();

            Assert.That(gameService, Is.TypeOf<GameService>(), ResolveErrorMessage);
            Assert.That(enemyFactory, Is.TypeOf<EnemyFactory>(), ResolveErrorMessage);
            Assert.That(scoreSystem, Is.TypeOf<ScoreSystem>(), ResolveErrorMessage);
            Assert.That(gameManager, Is.TypeOf<GameManager>(), ResolveErrorMessage);

            // Verify lifetime management
            var gameService2 = DIContainer.Resolve<IGameService>();
            var scoreSystem2 = DIContainer.Resolve<IScoreSystem>();

            Assert.That(gameService2, Is.Not.SameAs(gameService), TransientErrorMessage);
            Assert.That(scoreSystem2, Is.SameAs(scoreSystem), SingletonErrorMessage);
        }

        [Test]
        public void ServiceRegisterBase_WithTwoDifferentImplementation_RegisterServicesCorrectly()
        {
            // Arrange
            var runtimeRegister = new RuntimeServiceRegister();
            var editorRegister = new EditorServiceRegister();
            runtimeRegister.RegisterStaticServices();
            runtimeRegister.RegisterRuntimeServices();
            editorRegister.RegisterStaticServices();
            editorRegister.RegisterRuntimeServices();

            var testObject = new TestInjectableClass();

            // Assert
            Assert.That(testObject.GameService, Is.Not.Null);
            Assert.That(testObject.GameManager, Is.Not.Null);
            Assert.That(testObject.ScoreSystem, Is.Not.Null);
            Assert.That(testObject.EnemyFactory, Is.Not.Null);
            Assert.That(testObject.EditorService, Is.Not.Null);
            Assert.That(testObject.AssetProcessor, Is.Not.Null);
            Assert.That(testObject.ProjectSettings, Is.Not.Null);
            Assert.That(testObject.EditorManager, Is.Not.Null);
        }

        [Test]
        public void InjectableBehaviour_InjectsAllDependencies()
        {
            // Arrange
            var runtimeRegister = new RuntimeServiceRegister();
            runtimeRegister.RegisterStaticServices();
            runtimeRegister.RegisterRuntimeServices();

            var gameObject = new GameObject();
            var component = gameObject.AddComponent<TestInjectableBehaviour>();

            // Assert
            Assert.That(component.GameService, Is.Not.Null);
            Assert.That(component.ScoreSystem, Is.Not.Null);

            // Cleanup
            UnityEngine.Object.Destroy(gameObject);
        }

        [Test]
        public void Resolve_UnregisteredService_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => DIContainer.Resolve<IGameService>());
        }

        [Test]
        public void Clear_RemovesAllRegistrations()
        {
            // Arrange
            DIContainer.RegisterSingleton<IGameManager>(new GameManager());
            DIContainer.RegisterTransient<IGameService, GameService>();

            // Act
            DIContainer.Clear();

            // Assert
            Assert.Throws<InvalidOperationException>(() => DIContainer.Resolve<IGameManager>());
            Assert.Throws<InvalidOperationException>(() => DIContainer.Resolve<IGameService>());
        }

        [Test]
        public void ClearSingletonDependency_RemovesSpecificRegistration()
        {
            // Arrange
            DIContainer.RegisterSingleton<IGameManager>(new GameManager());
            DIContainer.RegisterSingleton<IScoreSystem, ScoreSystem>();

            // Act
            DIContainer.ClearSingletonDependency<IGameManager>();

            // Assert
            Assert.Throws<InvalidOperationException>(() => DIContainer.Resolve<IGameManager>());
            Assert.DoesNotThrow(() => DIContainer.Resolve<IScoreSystem>());
        }
    }

    internal interface IGameService
    {
        void Process();
    }

    internal sealed class GameService : IGameService
    {
        public void Process() { }
    }

    internal interface IGameManager
    {
        void Update();
    }

    internal sealed class GameManager : IGameManager
    {
        public void Update() { }
    }

    internal interface IEnemyFactory
    {
        GameObject Create();
    }

    internal sealed class EnemyFactory : IEnemyFactory
    {
        public GameObject Create() => null;
    }

    internal interface IScoreSystem
    {
        int GetScore();
    }

    internal sealed class ScoreSystem : IScoreSystem
    {
        public int GetScore() => 0;
    }

    internal interface IEditorService
    {
        void Process();
    }

    internal sealed class EditorService : IEditorService
    {
        public void Process() { }
    }

    internal interface IEditorManager
    {
        void Refresh();
    }

    internal sealed class EditorManager : IEditorManager
    {
        public void Refresh() { }
    }

    internal interface IAssetProcessor
    {
        void Process();
    }

    internal sealed class AssetProcessor : IAssetProcessor
    {
        public void Process() { }
    }

    internal interface IProjectSettings
    {
        string GetSetting(string key);
    }

    internal sealed class ProjectSettings : IProjectSettings
    {
        public string GetSetting(string key) => string.Empty;
    }

    internal sealed class TestInjectableClass : Injectable
    {
        [Inject] private readonly IGameService _gameService;
        [Inject] private readonly IGameManager _gameManager;
        [Inject] private readonly IScoreSystem _scoreSystem;
        [Inject] private readonly IEnemyFactory _enemyFactory;
        [Inject] private readonly IEditorService _editorService;
        [Inject] private readonly IAssetProcessor _assetProcessor;
        [Inject] private readonly IProjectSettings _projectSettings;
        [Inject] private readonly IEditorManager _editorManager;

        public IGameService GameService => _gameService;
        public IGameManager GameManager => _gameManager;
        public IScoreSystem ScoreSystem => _scoreSystem;
        public IEnemyFactory EnemyFactory => _enemyFactory;
        public IEditorService EditorService => _editorService;
        public IAssetProcessor AssetProcessor => _assetProcessor;
        public IProjectSettings ProjectSettings => _projectSettings;
        public IEditorManager EditorManager => _editorManager;
    }

    internal sealed class TestInjectableBehaviour : InjectableBehaviour
    {
        [Inject] private IGameService _gameService;
        [Inject] private IScoreSystem _scoreSystem;

        public IGameService GameService => _gameService;
        public IScoreSystem ScoreSystem => _scoreSystem;
    }

    internal sealed class RuntimeServiceRegister : ServiceRegisterBase
    {
        protected override void ConfigureStaticServices()
        {
            DIContainer.RegisterTransient<IGameService, GameService>();
            DIContainer.RegisterTransient<IEnemyFactory>(() => new EnemyFactory());
        }

        protected override void ConfigureRuntimeServices()
        {
            DIContainer.RegisterSingleton<IScoreSystem, ScoreSystem>();
            DIContainer.RegisterSingleton<IGameManager>(new GameManager());
        }
    }

    internal sealed class EditorServiceRegister : ServiceRegisterBase
    {
        protected override void ConfigureStaticServices()
        {
            DIContainer.RegisterTransient<IEditorService, EditorService>();
            DIContainer.RegisterTransient<IAssetProcessor, AssetProcessor>();
        }

        protected override void ConfigureRuntimeServices()
        {
            DIContainer.RegisterSingleton<IProjectSettings, ProjectSettings>();
            DIContainer.RegisterSingleton<IEditorManager>(new EditorManager());
        }
    }
}