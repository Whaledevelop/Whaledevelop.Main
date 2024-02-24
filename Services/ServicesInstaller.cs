using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;
using Whaledevelop.DiContainer;
using Whaledevelop.Extensions;

namespace Whaledevelop.Services
{
    [CreateAssetMenu(fileName = nameof(ServicesInstaller), menuName = "Installers/" + nameof(ServicesInstaller))]
    public class ServicesInstaller : SingletonScriptableObjectInstaller
    {
        [NonSerialized]
        #if ODIN_INSPECTOR
        [ShowInInspector]
        [HideInEditorMode]
        #endif
        private List<IService> _runtimeServices = new();
        
        [SerializeReference]
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private IService[] _services = Array.Empty<IService>();

        [NonSerialized]
        private IDisposable _subscription;

        protected override void OnInstallBindings()
        {
            _runtimeServices.Clear();

            for (var i = 0; i < _services.Length; i++)
            {
                var service = _services[i];
                _runtimeServices.Add(service);
                var mainInterface = service.GetType().GetMainInterface(typeof(IService));
                Container.Bind(mainInterface, service);
            }

            Container.Bind(_runtimeServices);

            for (var i = 0; i < _runtimeServices.Count; i++)
            {
                Container.TryInject(_runtimeServices[i]);
            }

            _runtimeServices.InitializeAsync(Application.exitCancellationToken).Forget();
        }
    }
}