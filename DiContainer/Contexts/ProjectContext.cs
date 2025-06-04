using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        private readonly Dictionary<string, IDiInternalContainer> _diContainers = new();

        private InjectableInfoContainer _injectableInfoContainer;

        internal static ProjectContext Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = new ProjectContext();
                _instance.Initialize();

                return _instance;
            }
        }

        internal IDiInternalContainer MainContainer { get; private set; }

        private void Initialize()
        {
            var filteredAssemblies = new List<Assembly>();
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in allAssemblies)
            {
                var fullName = assembly.FullName;

                var skip = false;
                foreach (var prefix in PREFIX_OF_SKIPPED_ASSEMBLIES)
                {
                    if (fullName.StartsWith(prefix))
                    {
                        skip = true;
                        break;
                    }
                }

                if (!skip)
                {
                    filteredAssemblies.Add(assembly);
                }
            }

            _injectableInfoContainer = new InjectableInfoContainer();
            _injectableInfoContainer.Initialization(filteredAssemblies.ToArray());

            MainContainer = new Internal.DiContainer(_injectableInfoContainer, new HashSet<IDisposable>());
            MainContainer.Bind<IDiContainer>(MainContainer);
        }

        internal IDiInternalContainer CreateContainer(string containerId, IDiInternalContainer baseContainer = null)
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
                disposables = new HashSet<IDisposable>();
            }

            if (_diContainers.Remove(containerId, out var previousContainer))
            {
                if (previousContainer is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            var diContainer = new Internal.DiContainer(_injectableInfoContainer, disposables, binds);
            _diContainers.Add(containerId, diContainer);

            return diContainer;
        }

        internal IDiInternalContainer GetContainer(string containerId)
        {
            _diContainers.TryGetValue(containerId, out var container);

            return container;
        }

        internal void DestroyAllContainers()
        {
            foreach (var container in _diContainers.Values)
            {
                if (container is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            _diContainers.Clear();
        }

        internal static void Reset()
        {
            if (_instance == null)
            {
                return;
            }

            _instance.DestroyAllContainers();
            _instance.MainContainer = null;
            _instance = null;
        }

        internal void DestroyContainer(IDiInternalContainer container)
        {
            var toRemove = _diContainers.FirstOrDefault(pair => pair.Value == container);

            if (!string.IsNullOrEmpty(toRemove.Key))
            {
                _diContainers.Remove(toRemove.Key);
            }

            if (container is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
