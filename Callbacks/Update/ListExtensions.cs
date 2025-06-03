using System.Collections.Generic;

namespace Whaledevelop
{
    public static class ListExtensions
    {
        public static void AddIfType<T1, T2>(this List<T2> list, T1 item, bool checkContains = true)
        {
            if (item is not T2 itemOfType)
            {
                return;
            }
            if (!checkContains || !list.Contains(itemOfType))
            {
                list.Add(itemOfType);
            }
        }
        
        public static void RemoveIfType<T1, T2>(this List<T2> list, T1 item, bool checkContains = true)
        {
            if (item is not T2 itemOfType)
            {
                return;
            }
            if (!checkContains || list.Contains(itemOfType))
            {
                list.Remove(itemOfType);
            }
        }
    }
}