using System;
using System.Reflection;

namespace Whaledevelop.DiContainer.Internal
{
    readonly struct InjectablePropertyInfo
    {
        public readonly string Id;
        public readonly bool Optional;
        private readonly PropertyInfo _propertyInfo;

        public Type TargetType => _propertyInfo.PropertyType;

        public InjectablePropertyInfo(string id, bool optional, PropertyInfo propertyInfo)
        {
            Id = id;
            Optional = optional;
            _propertyInfo = propertyInfo;
        }

        public void SetValue(object instance, object value)
        {
            _propertyInfo.SetValue(instance, value);
        }
    }
}