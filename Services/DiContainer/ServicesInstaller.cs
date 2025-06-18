using UnityEngine;

namespace Whaledevelop.Services
{
    [CreateAssetMenu(fileName = nameof(ServicesInstaller), menuName = "Whaledevelop/Installers/" + nameof(ServicesInstaller))]
    public class ServicesInstaller : ScriptableObject, IInstallerWrapper
    {
        [SerializeReference] 
        private IService[] _services;

        public void InstallBindings(IDiContainerWrapper container)
        {
            var servicesContainer = new ServicesContainer(_services);
            foreach (var service in _services)
            {
                container.BindToAssignableInterface(service);
            }
            container.Bind<IServicesContainer>(servicesContainer);
        }
    }
}