using System;
using System.Collections.Generic;
using UnityEngine;

namespace LocalSM
{
    /// <summary>
    /// Standalone (non-networked) state machine. A single-player reimplementation of
    /// PurrNet's StateMachine: an ordered list of <see cref="LocalStateNode"/>s with
    /// index-based transitions, typed-data entry, and an onStateChanged event.
    /// No NetworkBehaviour, SyncList, RPCs, or authority gating.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    [AddComponentMenu("Local State Machine/State Machine")]
    public sealed class LocalStateMachine : MonoBehaviour
    {
        [Tooltip("Ordered list of states. The first entry is entered automatically on Start.")]
        [SerializeField] private List<LocalStateNode> _states = new();

        [Tooltip("If false, the first state is NOT entered automatically; call SetState yourself.")]
        [SerializeField] private bool _autoStart = true;

        private int _currentId = -1;
        private int _previousId = -1;

        // Guards against re-entrant transitions (e.g. SetState called from inside Enter/Exit).
        private bool _transitioning;
        private readonly Queue<Action> _pending = new();

        public IReadOnlyList<LocalStateNode> states => _states;

        public LocalStateNode currentStateNode  => InRange(_currentId)  ? _states[_currentId]  : null;
        public LocalStateNode previousStateNode => InRange(_previousId) ? _states[_previousId] : null;

        /// <summary>The state Next() would advance to, or null if the list is empty. Does not transition.</summary>
        public LocalStateNode nextStateNode => _states.Count == 0 ? null : _states[GetNextId(_currentId)];

        public int currentStateId  => _currentId;
        public int previousStateId => _previousId;

        /// <summary>Fired after every successful transition: (previousState, newState).</summary>
        public event Action<LocalStateNode, LocalStateNode> onStateChanged;

        private bool InRange(int i) => i >= 0 && i < _states.Count;

        private void Awake()
        {
            for (int i = 0; i < _states.Count; i++)
                if (_states[i]) _states[i].Setup(this);
        }

        private void Start()
        {
            if (_autoStart && _states.Count > 0)
                SetState(_states[0]);
        }

        private void Update()
        {
            if (InRange(_currentId))
                _states[_currentId].StateUpdate();
        }

        // ---- Transitions -------------------------------------------------------

        /// <summary>Transition to a specific state instance.</summary>
        /// <param name="state">A state already present in the states list.</param>
        /// <param name="force">Skip the CanExit/CanEnter checks.</param>
        public bool SetState(LocalStateNode state, bool force = false)
        {
            int id = IndexOfOrWarn(state);
            if (id < 0) return false;
            if (!force && currentStateNode != null && !currentStateNode.CanExit()) return false;
            if (!force && !state.CanEnter()) return false;

            EnqueueTransition(id, () => state.Enter());
            return true;
        }

        /// <summary>Transition to a specific state instance, passing typed data.</summary>
        public bool SetState<T>(LocalStateNode<T> state, T data, bool force = false)
        {
            int id = IndexOfOrWarn(state);
            if (id < 0) return false;
            if (!force && currentStateNode != null && !currentStateNode.CanExit()) return false;
            if (!force && (!state.CanEnter() || !state.CanEnter(data))) return false;

            EnqueueTransition(id, () => state.Enter(data));
            return true;
        }

        /// <summary>Advance to the next state in the list (wraps around).</summary>
        public bool Next(bool force = false)
        {
            if (_states.Count == 0) return false;
            return SetState(_states[GetNextId(_currentId)], force);
        }

        /// <summary>Advance to the next state, passing typed data.</summary>
        public bool Next<T>(T data, bool force = false)
        {
            if (_states.Count == 0) return false;
            if (_states[GetNextId(_currentId)] is LocalStateNode<T> node)
                return SetState(node, data, force);

            Debug.LogError($"[LocalStateMachine] Next: next node is not a LocalStateNode<{typeof(T).Name}>.");
            return false;
        }

        /// <summary>Step back to the previous state in the list (wraps around).</summary>
        public bool Previous(bool force = false)
        {
            if (_states.Count == 0) return false;
            return SetState(_states[GetPreviousId(_currentId)], force);
        }

        /// <summary>Walk forward from the current state until a state accepts entry.</summary>
        public bool NextValid()
        {
            if (_states.Count == 0) return false;
            if (currentStateNode != null && !currentStateNode.CanExit()) return false;

            int start = _currentId;
            int id = GetNextId(start);
            do
            {
                if (SetState(_states[id])) return true;
                id = GetNextId(id);
            } while (id != start);

            return false;
        }

        // ---- State lookup helpers ---------------------------------------------

        /// <summary>Returns the first state of type T, or null.</summary>
        public T GetState<T>() where T : LocalStateNode
        {
            for (int i = 0; i < _states.Count; i++)
                if (_states[i] is T match) return match;
            return null;
        }

        public bool IsCurrentState<T>() where T : LocalStateNode => currentStateNode is T;

        public bool IsCurrentState<T>(out T stateInstance) where T : LocalStateNode
        {
            if (currentStateNode is T s) { stateInstance = s; return true; }
            stateInstance = null;
            return false;
        }

        public bool IsCurrentState(LocalStateNode stateToCheck) => currentStateNode == stateToCheck;

        // ---- List management ---------------------------------------------------

        public void AddState(LocalStateNode state)
        {
            if (!state) { Debug.LogError("[LocalStateMachine] AddState: state is null."); return; }
            _states.Add(state);
            state.Setup(this);
        }

        public void InsertState(LocalStateNode state, int index)
        {
            if (!state) { Debug.LogError("[LocalStateMachine] InsertState: state is null."); return; }
            _states.Insert(index, state);
            state.Setup(this);
            // Keep the active/previous indices pointing at the same instances.
            if (index <= _currentId)  _currentId++;
            if (index <= _previousId) _previousId++;
        }

        public bool RemoveState(LocalStateNode state)
        {
            if (state == currentStateNode)
            {
                Debug.LogError("[LocalStateMachine] RemoveState: cannot remove the current state.");
                return false;
            }
            int idx = _states.IndexOf(state);
            if (idx < 0) return false;

            _states.RemoveAt(idx);
            if (idx < _currentId)  _currentId--;
            if (idx < _previousId) _previousId--;
            else if (idx == _previousId) _previousId = -1;
            return true;
        }

        // ---- Internals ---------------------------------------------------------

        private int IndexOfOrWarn(LocalStateNode state)
        {
            if (!state) { Debug.LogError("[LocalStateMachine] state is null."); return -1; }
            int id = _states.IndexOf(state);
            if (id < 0)
                Debug.LogError($"[LocalStateMachine] '{state.name}' ({state.GetType().Name}) is not in the states list.");
            return id;
        }

        private void EnqueueTransition(int newId, Action enterCall)
        {
            _pending.Enqueue(() => ApplyTransition(newId, enterCall));

            if (_transitioning)
                return; // Drained by the in-flight ApplyTransition loop.

            _transitioning = true;
            try
            {
                while (_pending.Count > 0)
                    _pending.Dequeue().Invoke();
            }
            finally
            {
                _transitioning = false;
            }
        }

        private void ApplyTransition(int newId, Action enterCall)
        {
            var prev = currentStateNode;

            try { prev?.Exit(); }
            catch (Exception e) { Debug.LogException(e); }

            _previousId = _currentId;
            _currentId  = newId;
            var newState = _states[newId];

            try { onStateChanged?.Invoke(prev, newState); }
            catch (Exception e) { Debug.LogException(e); }

            try { enterCall(); }
            catch (Exception e) { Debug.LogException(e); }
        }

        private int GetNextId(int id)
        {
            int next = id + 1;
            return next >= _states.Count ? 0 : next;
        }

        private int GetPreviousId(int id)
        {
            int prev = id - 1;
            return prev < 0 ? _states.Count - 1 : prev;
        }
    }
}
