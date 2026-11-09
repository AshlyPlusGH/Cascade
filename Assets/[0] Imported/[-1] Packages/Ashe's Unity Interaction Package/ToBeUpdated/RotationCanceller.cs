using NaughtyAttributes;
using UnityEngine;

public class RotationCanceller : MonoBehaviour
{
    [Button] void LateUpdate(){ transform.rotation = Quaternion.identity; }
}