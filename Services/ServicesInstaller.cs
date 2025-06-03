using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Whaledevelop.DiContainer;


namespace Whaledevelop.Services
{
    [CreateAssetMenu(fileName = nameof(ServicesInstaller), menuName = "Whaledevelop/Installers/" + nameof(ServicesInstaller))]
    public class ServicesInstaller : ScriptableObjectInstaller
    {
        [SerializeField]
        private Service[] _services;

        public override void InstallBindings()
        {
            var servicesContainer = new ServicesContainer(Container);
            foreach (var service in _services)
            {
                Container.BindToAssignableInterface<IService>(service);
                servicesContainer.Add(service);
            }
            Container.BindToInterface<IServicesContainer>(servicesContainer);
        }
    }
}