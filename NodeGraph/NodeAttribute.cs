using System;

namespace Whaledevelop.NodeGraph
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeAttribute : Attribute
    {
        public readonly string MenuTitle;
        public readonly bool HideFromSearch;

        public NodeAttribute(string menuTitle, bool hideFromSearch = false)
        {
            MenuTitle = menuTitle;
            HideFromSearch = hideFromSearch;
        }
    }
}