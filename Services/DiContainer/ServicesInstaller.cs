using UnityEngine;
using Whaledevelop.DiContainer;


namespace Whaledevelop.Services
{
    [CreateAssetMenu(fileName = nameof(ServicesInstaller), menuName = "Whaledevelop/Installers/" + nameof(ServicesInstaller))]
    public class ServicesInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private int _test;
        
        [SerializeReference] 
        private IService[] _services;
        
        public override void InstallBindings()
        {
            var servicesContainer = new ServicesContainer(_services);
            foreach (var service in _services)
            {
                Container.BindToAssignableInterface<IService>(service);
            }
            Container.BindToInterface<IServicesContainer>(servicesContainer);
        }
    }
}