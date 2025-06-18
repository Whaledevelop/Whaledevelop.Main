using System;

namespace Whaledevelop
{
    public interface IDiContainerWrapper
    {
        void Inject<T>(T instance) where T : class;
        
        T Resolve<T>() where T : class;
        
        void Bind<T>(T instance) where T : class;
        
        void Bind(Type type, object instance);
    }
}