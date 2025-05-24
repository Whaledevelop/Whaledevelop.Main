namespace Whaledevelop.Services
{
    public class ServicesContainer : InitializablesContainer<IService>, IServicesContainer
    {
        public ServicesContainer(IDiContainer diContainer) : base(diContainer)
        {
        }
    }
}