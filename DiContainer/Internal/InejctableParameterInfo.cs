namespace Whaledevelop.DiContainer.Internal
{
    readonly struct InjectableParameterInfo
    {
        public readonly System.Type Type;
        public readonly string Id;

        public InjectableParameterInfo(System.Type type, string id)
        {
            Type = type;
            Id = id;
        }
    }
}