using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DependencyInjection.Runtime.InjectionBase
{
    /// <summary>
    /// Provides dependency injection functionality for objects with injectable fields.
    /// </summary>
    internal static class DependencyInjector
    {
        private static readonly ConcurrentDictionary<Type, FieldInfo[]> _fieldCache = new();

        /// <summary>
        /// Injects dependencies into the target object's injectable fields.
        /// </summary>
        /// <param name="target">The target object to inject dependencies into.</param>
        internal static void InjectDependencies(object target)
        {
            if (target == null) return;

            var targetType = target.GetType();
            var injectableFields = GetInjectableFields(targetType);

            foreach (var field in injectableFields)
            {
                var injectAttribute = field.GetCustomAttribute<InjectAttribute>();
                if (injectAttribute == null) continue;

                try
                {
                    var dependency = DIContainer.Resolve(field.FieldType);

                    field.SetValue(target, dependency);
                }
                catch (Exception e)
                {
                    Debug.LogError(
                        "Dependency Injection Failed: " +
                        $"Type: {targetType.Name}, " +
                        $"Field: {field.Name}, " +
                        $"Error: {e.Message}");
                }
            }
        }

        private static FieldInfo[] GetInjectableFields(Type type)
        {
            return _fieldCache.GetOrAdd(type, t =>
                t.GetFields(
                        BindingFlags.Public |
                        BindingFlags.NonPublic |
                        BindingFlags.Instance
                    ).Where(f => f.GetCustomAttribute<InjectAttribute>() != null)
                    .ToArray()
            );
        }
    }
}