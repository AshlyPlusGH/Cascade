using UnityEngine;
using UnityEngine.Events;

namespace Ash
{
    public class UnityEventOnLoopTimer : MonoBehaviour
    {
        [SerializeField] private bool debug;
            private void Log(string contents){ if (debug){ Debug.Log(contents + " at: " + name); }}

        [Space(10)]

        [SerializeField] private UnityEvent unityEvent;
        [SerializeField] private float loopTime;
        private float timer;

        void Update()
        {
            if (timer > loopTime){ Trigger(); timer = 0; }
            timer += Time.deltaTime;
        }

        void Trigger(){ unityEvent.Invoke(); }
    }
}