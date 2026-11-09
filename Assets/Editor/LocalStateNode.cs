using UnityEngine;

namespace LocalSM
{
    /// <summary>
    /// Standalone (non-networked) equivalent of PurrNet's StateNode.
    /// A plain MonoBehaviour state owned by a <see cref="LocalStateMachine"/>.
    /// Attach this (a subclass) to a GameObject and add it to the machine's states list.
    /// </summary>
    public abstract class LocalStateNode : MonoBehaviour
    {
        protected LocalStateMachine machine { get; private set; }

        /// <summary>True while this node is the machine's active state.</summary>
        public bool isCurrentState => machine && machine.currentStateNode == this;

        /// <summary>Wires the node back to its owning machine. Called by the machine.</summary>
        public void Setup(LocalStateMachine stateMachine) => machine = stateMachine;

        /// <summary>Called once when this state becomes active.</summary>
        public virtual void Enter() { }

        /// <summary>Called every frame while this state is active (driven by the machine's Update).</summary>
        public virtual void StateUpdate() { }

        /// <summary>Called once when this state is left.</summary>
        public virtual void Exit() { }

        /// <summary>Override to block entry into this state.</summary>
        public virtual bool CanEnter() => true;

        /// <summary>Override to block leaving this state.</summary>
        public virtual bool CanExit() => true;
    }

    /// <summary>
    /// State that receives a typed payload on entry, mirroring PurrNet's StateNode&lt;T&gt;.
    /// </summary>
    /// <typeparam name="T">Type of the data passed in on transition.</typeparam>
    public abstract class LocalStateNode<T> : LocalStateNode
    {
        /// <summary>Called once when this state becomes active, with the supplied data.</summary>
        public virtual void Enter(T data) { }

        /// <summary>Override to block entry based on the incoming data.</summary>
        public virtual bool CanEnter(T data) => true;
    }
}
