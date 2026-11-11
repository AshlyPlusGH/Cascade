using UnityEngine;
using PurrNet;

namespace Shadowhulk
{
    [RequireComponent(typeof(OwnedObject))]
    public class Structure : MonoBehaviour
    {
        [SerializeField] private bool debug;
            private void Log(string contents){ if (debug){ Debug.Log(contents + " at: " + name); }}

        [Space(10)]

        [SerializeField] private OwnedObject ownedObject;

        private PlayerID? owner => ownedObject.owner;

        void Awake(){ Setup(); }
        public virtual void Setup(){  }

        public void Reset(){ ownedObject = FindAnyObjectByType<OwnedObject>(); }
    }
}