using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Whaledevelop.DiContainer;


namespace Whaledevelop.Services
{
    [CreateAssetMenu(fileName = nameof(ServicesInstaller), menuName = "Installers/" + nameof(ServicesInstaller))]
    public class ServicesInstaller : SingletonScriptableObjectInstaller
    {
        [SerializeField]
        private ServiceScriptableObject[] _services;

        protected override void OnInstallBindings()
        {
            var servicesContainer = new ServicesContainer(Container);
            foreach (var service in _services)
            {
                Container.BindToInterface<IService>(service);
                servicesContainer.Add(service);
            }
            Container.Bind(servicesContainer);
        }
    }
}