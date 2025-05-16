using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Whaledevelop
{
    [CreateAssetMenu(menuName = "Whaledevelop/Systems/Game Systems Config")]
    public class GameSystemsConfig : ScriptableObject
    {
        [SerializeField]
        private GameSystemScriptable[] _gameSystems;

        [SerializeField, ValidateInput(nameof(ValidateInput))] 
        private GameSystemScriptable[] _updateOrder;

        public GameSystemScriptable[] GameSystems => _gameSystems;
        public GameSystemScriptable[] UpdateOrder => _updateOrder;

        private bool ValidateInput()
        {
            if (_gameSystems == null || _updateOrder == null)
            {
                return false;
            }

            foreach (var system in _updateOrder)
            {
                if (!_gameSystems.Contains(system))
                {
                    return false;
                }

                if (system is not IUpdatable && system is not IFixedUpdatable && system is not ILateUpdatable)
                {
                    return false;
                }
            }

            return true;
        }
        
        public IEnumerable<T> GetSorted<T>() where T : class
        {
            var set = new HashSet<GameSystemScriptable>(_updateOrder);

            foreach (var prioritized in _updateOrder)
            {
                if (prioritized is T match)
                {
                    yield return match;
                }
            }

            foreach (var system in _gameSystems)
            {
                if (!set.Contains(system) && system is T match)
                {
                    yield return match;
                }
            }
        }
        
#if UNITY_EDITOR
        
        private const string SearchPath = "Assets/_Game/Scriptables/Systems";
        
        [Button("Find Systems in Folder")]
        private void FindAllSystemsInFolder()
        {
            var guids = AssetDatabase.FindAssets("t:GameSystemScriptable", new[] { SearchPath });
            var found = guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<GameSystemScriptable>)
                .Where(x => x != null)
                .ToList();

            var current = new HashSet<GameSystemScriptable>(_gameSystems);
            current.UnionWith(found);

            _gameSystems = current.ToArray();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}