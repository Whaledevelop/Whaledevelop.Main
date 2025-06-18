namespace Whaledevelop
{
    public interface IDiContainer
    {
        void Inject<T>(T instance) where T : class;

        T Resolve<T>() where T : class;

        bool TryResolve<T>(out T instance) where T : class;
    }
}