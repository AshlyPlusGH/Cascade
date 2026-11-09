using UnityEngine;

namespace Ash
{
    public class Rotate : MonoBehaviour
    {
        [SerializeField] private bool debug;
            private void Log(string contents){ if (debug){ Debug.Log(contents + " at: " + name); }}

        [Space(10)]

        [SerializeField] private Vector3 speed;
        [SerializeField] private bool isInverted;

        void Awake(){ Setup(); }
        public void Setup(){  }

        void Update()
        {
            if (isInverted){ transform.Rotate(-speed); return; }
            transform.Rotate(speed);
        }
    }
}