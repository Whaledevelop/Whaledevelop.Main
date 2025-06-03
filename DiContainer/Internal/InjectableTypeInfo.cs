using System.Collections.Generic;
using System.Reflection;

namespace Whaledevelop.DiContainer.Internal
{
    class InjectableTypeInfo
    {
        public List<InjectableFieldInfo> Fields = new();
        public MethodInfo Method;
        public List<InjectablePropertyInfo> Properties = new();
        public List<InjectableParameterInfo> ConstructParameters = new();
        public MethodInfo ConstructMethod;
    }
}