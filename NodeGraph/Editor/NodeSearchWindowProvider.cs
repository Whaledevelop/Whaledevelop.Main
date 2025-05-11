using System;
using System.Collections.Generic;
using System.Linq;
using Whaledevelop.Scopes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Whaledevelop.NodeGraph
{
    public abstract class NodeSearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        public Action<Type, Vector2> OnCreateNode { get; set; }

        protected abstract IOrderedEnumerable<KeyValuePair<Type, string>> GetSortedNodes();

        #region ISearchWindowProvider

        List<SearchTreeEntry> ISearchWindowProvider.CreateSearchTree(SearchWindowContext context)
        {
            
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node")),
            };

            using (HashSetScope<string>.Create(out var usedMenuParts))
            {
                foreach (var (type, menuTitle) in GetSortedNodes())
                {
                    var menuParts = menuTitle.Split('/');
                    var prefixMenuPart = "";

                    for (var i = 0; i < menuParts.Length - 1; i++)
                    {
                        prefixMenuPart += "/";
                        prefixMenuPart += menuParts[i];

                        if (!usedMenuParts.Add(prefixMenuPart))
                        {
                            continue;
                        }

                        var searchGroupTreeEntry = new SearchTreeGroupEntry(new GUIContent(menuParts[i]))
                        {
                            level = i + 1
                        };
                        tree.Add(searchGroupTreeEntry);
                    }

                    var searchTreeEntry = new SearchTreeEntry(new GUIContent(menuParts[^1]))
                    {
                        level = menuParts.Length,
                        userData = type
                    };
                    tree.Add(searchTreeEntry);
                }
            }

            return tree;
        }

        bool ISearchWindowProvider.OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            
            OnCreateNode?.Invoke((Type)searchTreeEntry.userData, context.screenMousePosition);
            return true;
        }

        #endregion
    }
}