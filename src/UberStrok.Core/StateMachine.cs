using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UberStrok.Core
{
    public class StateMachine<T> where T : struct, IConvertible
    {
        private State _current;

        private readonly Stack<T> _states;
        private readonly Dictionary<T, State> _registeredStates;

        public StateMachine()
        {
            _states = new Stack<T>();
            _registeredStates = new Dictionary<T, State>();
        }

        public T Current => _states.Count > 0 ? _states.Peek() : default;

        public void Register(T stateId, State state)
        {
            if (_registeredStates.ContainsKey(stateId))
            {
                throw new Exception("Already contains a state handler for the specified state ID.");
            }

            _registeredStates.Add(stateId, state);
        }

        public void Set(T stateId)
        {
            if (!_registeredStates.TryGetValue(stateId, out State state))
            {
                throw new Exception("No state handler registered for the specified state ID.");
            }

            if (!Current.Equals(stateId))
            {
                _current?.OnExit();

                _current = state;
                _states.Push(stateId);

                _current?.OnEnter();
            }
        }

        public void Previous()
        {
            _current?.OnExit();

            _ = _states.Pop();

            if (_states.Count > 0)
            {
                T stateId = _states.Peek();
                bool exists = _registeredStates.TryGetValue(stateId, out State state);

                Debug.Assert(exists);

                _current = state;
                _current?.OnResume();
            }
            else
            {
                _current = null;
            }
        }

        public void Reset()
        {
            while (_states.Count > 0)
            {
                Previous();
            }
        }

        public void Tick()
        {
            _current?.OnTick();
        }
    }
}
