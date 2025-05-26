using System;
using System.Linq;

namespace Whaledevelop.DiContainer
{
    public static class DiContainerExtensions
    {
        public static IDiContainer CreateIfNull(this IDiContainer self, string containerId, IDiContainer baseContainer = null)
        {
            return self ?? DiContainerUtility.CreateContainer(containerId, baseContainer);
        }

        public static void Destroy(this IDiContainer self)
        {
            ProjectContext.Instance.DestroyContainer(self);
        }
        
        public static void BindToInterface<T>(this IDiContainer self, T target)
        {
            self.Bind(typeof(T), target);
        }

        public static void BindToAssignableInterface<T>(this IDiContainer self, T target)
        {
            var type = typeof(T);
            var interfaces = target.GetType().GetInterfaces();
            var mainInterface = interfaces.FirstOrDefault(i =>
                i != type && type.IsAssignableFrom(i));
    
            self.Bind(mainInterface, target);
        }

        public static bool TryInject<T>(this IDiContainer self, T @object)
            where T : class
        {
            if (!self.IsInjectable(@object))
            {
                return false;
            }
            self.Inject(@object);
            return true;
        }

        public static void BindAndInject<T>(this IDiContainer self, T @object, string id = null)
            where T : class
        {
            self.Bind(@object, id);
            self.Inject(@object);
        }

        public static T ResolveAndInject<T>(this IDiContainer self, string id = null)
            where T : class
        {
            var instance = self.Resolve<T>(id);
            self.Inject(instance);
            return instance;
        }
    }
}