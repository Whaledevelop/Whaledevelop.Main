namespace Whaledevelop.Services
{
    public class ServicesContainer : InitializablesContainer<IService>
    {
        public ServicesContainer(IDiContainer diContainer) : base(diContainer)
        {
        }
    }
}