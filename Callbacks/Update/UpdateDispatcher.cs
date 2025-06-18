using System.Collections.Generic;

namespace Whaledevelop.GameSystems
{
    public class UpdateDispatcher
    {
        private readonly List<IUpdate> _updates = new();
        private readonly List<IFixedUpdate> _fixedUpdates = new();
        private readonly List<ILateUpdate> _lateUpdates = new();

        private IUpdateCallbacks _callbacks;

        public void Initialize(IUpdateCallbacks callbacks)
        {
            _callbacks = callbacks;
            _callbacks.OnUpdate += OnUpdate;
            _callbacks.OnFixedUpdate += OnFixedUpdate;
            _callbacks.OnLateUpdate += OnLateUpdate;
        }

        public void Dispose()
        {
            if (_callbacks != null)
            {
                _callbacks.OnUpdate -= OnUpdate;
                _callbacks.OnFixedUpdate -= OnFixedUpdate;
                _callbacks.OnLateUpdate -= OnLateUpdate;
                _callbacks = null;
            }

            Clear();
        }

        public void TryRegister<T>(T item) where T : class
        {
            AddIfType(_updates, item);
            AddIfType(_fixedUpdates, item);
            AddIfType(_lateUpdates, item);
        }

        public void TryUnregister(object item)
        {
            RemoveIfType(_updates, item);
            RemoveIfType(_fixedUpdates, item);
            RemoveIfType(_lateUpdates, item);
        }
        
        private static void AddIfType<T1, T2>(IList<T2> list, T1 item, bool checkContains = true) where T1 : class where T2 : class
        {
            if (item is T2 itemOfType && (!checkContains || !list.Contains(itemOfType)))
            {
                list.Add(itemOfType);
            }
        }
        
        private static void RemoveIfType<T1, T2>(IList<T2> list, T1 item, bool checkContains = true)
        {
            if (item is T2 itemOfType && (!checkContains || list.Contains(itemOfType)))
            {
                list.Remove(itemOfType);
            }
        }

        public void Clear()
        {
            _updates.Clear();
            _fixedUpdates.Clear();
            _lateUpdates.Clear();
        }

        private void OnUpdate()
        {
            foreach (var updatable in _updates)
            {
                updatable.OnUpdate();
            }
        }

        private void OnFixedUpdate()
        {
            foreach (var fixedUpdatable in _fixedUpdates)
            {
                fixedUpdatable.OnFixedUpdate();
            }
        }

        private void OnLateUpdate()
        {
            foreach (var lateUpdatable in _lateUpdates)
            {
                lateUpdatable.OnLateUpdate();
            }
        }
    }
}
