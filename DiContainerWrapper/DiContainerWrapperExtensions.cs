using System.Linq;

namespace Whaledevelop
{
    public static class DiContainerWrapperExtensions
    {
        public static void BindToAssignableInterface<T>(this IDiContainerWrapper self, T target)
        {
            var type = typeof(T);
            var interfaces = target.GetType().GetInterfaces();
            var mainInterface = interfaces.FirstOrDefault(i =>
                i != type && type.IsAssignableFrom(i));
    
            self.Bind(mainInterface, target);
        }
    }
}