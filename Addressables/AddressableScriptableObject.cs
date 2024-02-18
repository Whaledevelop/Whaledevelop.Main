using UnityEngine;

namespace Whaledevelop.Addressables
{
    public abstract class AddressableScriptableObject : ScriptableObject, IAddressable
    {
        [SerializeField]
        private Reference _reference;

        protected Reference Reference => _reference;

#if UNITY_EDITOR
        private string Guid => _reference.AssetGuid;
#endif

        #region IAddressable

        string IAddressable.Name => name;

        #endregion
    }

    public abstract class AddressableScriptableObject<T> : AddressableScriptableObject, IAddressable<T>
        where T : class
    {
        #region IAddressable<T>

        Reference<T> IAddressable<T>.Reference => Reference.ToReference<T>();

        #endregion
    }
}