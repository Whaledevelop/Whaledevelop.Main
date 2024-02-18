using System;
using System.Reflection;

namespace Whaledevelop.DiContainer.Internal
{
    readonly struct InjectableFieldInfo
    {
        public readonly string Id;
        public readonly bool Optional;
        private readonly FieldInfo _fieldInfo;

        public Type TargetType => _fieldInfo.FieldType;

        public InjectableFieldInfo(string id, bool optional, FieldInfo fieldInfo)
        {
            Id = id;
            Optional = optional;
            _fieldInfo = fieldInfo;
        }

        public void SetValue(object instance, object value)
        {
            _fieldInfo.SetValue(instance, value);
        }
    }
}