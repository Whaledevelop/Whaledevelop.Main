using System.Collections.Generic;

namespace Whaledevelop
{
    public static class UpdateExtensions
    {
        public static void AddToUpdateLists<T>(this T self,
            List<IUpdate> updatables,
            List<IFixedUpdate> fixedUpdatables,
            List<ILateUpdate> lateUpdatables,
            bool checkContains = true)
        {
            if (self is IUpdate updateTarget)
            {
                if (!checkContains || !updatables.Contains(updateTarget))
                {
                    updatables.Add(updateTarget);
                }
            }

            if (self is IFixedUpdate fixedUpdateTarget)
            {
                if (!checkContains || !fixedUpdatables.Contains(fixedUpdateTarget))
                {
                    fixedUpdatables.Add(fixedUpdateTarget);
                }
            }

            if (self is ILateUpdate lateUpdateTarget)
            {
                if (!checkContains || !lateUpdatables.Contains(lateUpdateTarget))
                {
                    lateUpdatables.Add(lateUpdateTarget);
                }
            }
        }

        public static void RemoveFromUpdateLists<T>(this T self,
            List<IUpdate> updatables,
            List<IFixedUpdate> fixedUpdatables,
            List<ILateUpdate> lateUpdatables,
            bool checkContains = true)
        {
            if (self is IUpdate updateTarget)
            {
                if (!checkContains || updatables.Contains(updateTarget))
                {
                    updatables.Remove(updateTarget);
                }
            }

            if (self is IFixedUpdate fixedUpdateTarget)
            {
                if (!checkContains || fixedUpdatables.Contains(fixedUpdateTarget))
                {
                    fixedUpdatables.Remove(fixedUpdateTarget);
                }
            }

            if (self is ILateUpdate lateUpdateTarget)
            {
                if (!checkContains || lateUpdatables.Contains(lateUpdateTarget))
                {
                    lateUpdatables.Remove(lateUpdateTarget);
                }
            }
        }
    }
}
