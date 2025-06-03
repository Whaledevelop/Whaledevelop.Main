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

        public void Register(object system)
        {
            _updates.AddIfType(system);
            _fixedUpdates.AddIfType(system);
            _lateUpdates.AddIfType(system);
        }

        public void Unregister(object system)
        {
            _updates.RemoveIfType(system);
            _fixedUpdates.RemoveIfType(system);
            _lateUpdates.RemoveIfType(system);
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
