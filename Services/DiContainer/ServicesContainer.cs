namespace Whaledevelop.Services
{
    public class ServicesContainer : InitializableContainer<IService>, IServicesContainer
    {
        public ServicesContainer(IService[] services) : base(services)
        {
        }
    }
}