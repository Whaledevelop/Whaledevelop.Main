using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Whaledevelop.UIToolkit
{
    public static class UxmlElementReferenceUtility
    {
        public static bool TryGetSharedVisualTreeAsset(out VisualTreeAsset sharedAsset, params UxmlElementReference[] references)
        {
            sharedAsset = null;

            if (references == null || references.Length == 0)
            {
                return false;
            }

            sharedAsset = references[0]?.Asset;
            if (!sharedAsset)
            {
                return false;
            }

            for (int i = 1; i < references.Length; i++)
            {
                if (references[i]?.Asset != sharedAsset)
                {
                    sharedAsset = null;
                    return false;
                }
            }

            return true;
        }
    }
}