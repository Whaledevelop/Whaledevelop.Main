namespace Whaledevelop.Addressables
{
    public interface IAddressable
    {
        string Name { get; }
    }

    public interface IAddressable<T> : IAddressable
        where T : class
    {
        Reference<T> Reference { get; }
    }
}