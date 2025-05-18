using System;
using System.Collections.Generic;

namespace START_Project
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

        public void Subscribe(Action<T> action)
        {
            _subscribers.Add(action);
            action.Invoke(default);
        }

        public void Unsubscribe(Action<T> action)
        {
            _subscribers.Remove(action);
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

        public void Subscribe(Action action)
        {
            _subscribers.Add(action);
            action.Invoke();
        }

        public void Unsubscribe(Action action)
        {
            _subscribers.Remove(action);
        }
    }
}