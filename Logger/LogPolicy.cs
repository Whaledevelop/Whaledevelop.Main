using System;
using System.Collections.Generic;

namespace Whaledevelop
{
    [Serializable]
    public class LogPolicy
    {
        private bool _logUntagged = true;
        private Dictionary<LogTag, bool> _tags = new();

        public bool ShouldBeLogged(LogTag tag)
        {
            return _tags.TryGetValue(tag, out var result) ? result : _logUntagged;
        }
    }
}