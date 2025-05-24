using System.Collections.Generic;

namespace Whaledevelop
{
    public static class UpdateExtensions
    {
        public static void DistributeToUpdateLists<T>(this T self,
            List<IUpdate> updatables,
            List<IFixedUpdate> fixedUpdatables,
            List<ILateUpdate> lateUpdatables)
        {
            if (self is IUpdate u)
            {
                updatables.Add(u);
            }

            if (self is IFixedUpdate f)
            {
                fixedUpdatables.Add(f);
            }

            if (self is ILateUpdate l)
            {
                lateUpdatables.Add(l);
            }
        }
    }
}