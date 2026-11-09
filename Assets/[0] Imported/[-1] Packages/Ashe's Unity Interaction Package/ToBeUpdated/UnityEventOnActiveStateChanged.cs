using UnityEngine;
using UnityEngine.Events;

public class UnityEventOnActiveStateChanged : MonoBehaviour
{
    [SerializeField] private UnityEvent unityEvent;
    [SerializeField] GameObject target;

    bool stateLastFrame;

    public void Update(){ if (target.activeInHierarchy != stateLastFrame) unityEvent.Invoke(); }

    public void LateUpdate(){ stateLastFrame = target.activeInHierarchy; }
}