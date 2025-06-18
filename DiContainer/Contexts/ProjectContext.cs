using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Whaledevelop.DiContainer.Internal;

namespace Whaledevelop.DiContainer
{
    class ProjectContext
    {
        private static ProjectContext _instance;

        private static readonly string[] PREFIX_OF_SKIPPED_ASSEMBLIES =
        {
            "Unity",
            "Mono.",
            "System",
            "ExCSS.Unity",
            "mscorlib"
        };

        private readonly Dictionary<string, IDiContainer> _diContainers = new();
        private InjectableInfoContainer _injectableInfoContainer;

        internal static ProjectContext Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = new();
                _instance.Initialize();
                return _instance;
            }
        }

        internal IDiContainer MainContainer { get; private set; }

        private void Initialize()
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(
                    x =>
                    {
                        var fullName = x.FullName;
                        for (var i = 0; i < PREFIX_OF_SKIPPED_ASSEMBLIES.Length; i++)
                        {
                            if (fullName.StartsWith(PREFIX_OF_SKIPPED_ASSEMBLIES[i]))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                )
                .ToArray();

            _injectableInfoContainer = new();
            _injectableInfoContainer.Initialization(allAssemblies);

            MainContainer = new Internal.DiContainer(_injectableInfoContainer, new());
            MainContainer.Bind(MainContainer, "main");
        }

        internal IDiContainer CreateContainer(string containerId, IDiContainer baseContainer = null)
        {
            Dictionary<BindKey, object> binds = null;
            HashSet<IDisposable> disposables;

            if (baseContainer is Internal.DiContainer parentContainer)
            {
                binds = parentContainer.Binds;
                disposables = parentContainer.Disposables;
            }
            else
            {
                disposables = new();
            }

            if (_diContainers.TryGetValue(containerId, out var previousDiContainer))
            {
                _diContainers.Remove(containerId);

                if (previousDiContainer is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            var diContainer = new Internal.DiContainer(_injectableInfoContainer, disposables, binds);
            _diContainers.Add(containerId, diContainer);
            return diContainer;
        }

        internal IDiContainer GetContainer(string containerId)
        {
            _diContainers.TryGetValue(containerId, out var container);
            return container;
        }
        
        internal void DestroyAllContainers()
        {
            foreach (var kvp in _diContainers.Values.ToList())
            {
                if (kvp is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            _diContainers.Clear();
        }
        
        internal static void Reset()
        {
            if (_instance != null)
            {
                _instance.DestroyAllContainers();
                _instance.MainContainer = null;
                _instance = null;
            }
        }

        internal void DestroyContainer(IDiContainer container)
        {
            foreach (var diContainer in _diContainers.Where(currentContainer => currentContainer.Value == container))
            {
                _diContainers.Remove(diContainer.Key);
                break;
            }

            if (container is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}