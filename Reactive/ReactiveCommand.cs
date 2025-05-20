using System;
using System.Collections.Generic;

namespace Whaledevelop.Reactive
{
    public class ReactiveCommand<T>
    {
        private readonly List<Action<T>> _subscribers = new();

        public void Execute(T value)
        {
            foreach (var subscriber in _subscribers)
            {
                subscriber.Invoke(value);
            }
        }

        public IDisposable Subscribe(Action<T> action)
        {
            _subscribers.Add(action);

            return new Subscription(() => _subscribers.Remove(action));
        }
        
        public void Unsubscribe(Action<T> action)
        {
            _subscribers.Remove(action);
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

    public class ReactiveCommand
    {
        private readonly List<Action> _subscribers = new();

        public void Execute()
        {
            foreach (var subscriber in _subscribers)
            {
                subscriber.Invoke();
            }
        }

        public IDisposable Subscribe(Action action)
        {
            _subscribers.Add(action);

            return new Subscription(() => _subscribers.Remove(action));
        }

        public void Unsubscribe(Action action)
        {
            _subscribers.Remove(action);
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
    
    public class ReactiveCommand<T1, T2>
    {
        private readonly List<Action<T1, T2>> _subscribers = new();

        public void Execute(T1 arg1, T2 arg2)
        {
            foreach (var subscriber in _subscribers)
            {
                subscriber.Invoke(arg1, arg2);
            }
        }

        public IDisposable Subscribe(Action<T1, T2> action)
        {
            _subscribers.Add(action);

            return new Subscription(() => _subscribers.Remove(action));
        }

        public void Unsubscribe(Action<T1, T2> action)
        {
            _subscribers.Remove(action);
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
