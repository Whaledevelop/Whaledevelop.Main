using System;

namespace Whaledevelop.Reactive
{
    public class ReactiveValue<T>
    {
        private T _value;

        public T Value
        {
            get => _value;
            set
            {
                if (!Equals(_value, value))
                {
                    _value = value;
                    _onValueChanged?.Invoke(_value);
                }
            }
        }

        private event Action<T> _onValueChanged;

        public ReactiveValue(T initialValue = default)
        {
            _value = initialValue;
        }

        public IDisposable Subscribe(Action<T> callback, bool callCallbackFirstTime = true)
        {
            if (callCallbackFirstTime)
            {
                callback?.Invoke(_value);
            }
            _onValueChanged += callback;

            return new Subscription(() => _onValueChanged -= callback);
        }
        
        public void Unsubscribe(Action<T> callback)
        {
            _onValueChanged -= callback;
        }

        private class Subscription : IDisposable
        {
            private Action _onDispose;
            private bool _disposed;

            public Subscription(Action onDispose)
            {
                _onDispose = onDispose;
            }

            public void Dispose()
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                _onDispose?.Invoke();
                _onDispose = null;
            }
        }
    }
}