using NaughtyAttributes;
using UnityEngine;

[ExecuteAlways]
public class RotationCanceller : MonoBehaviour
{
    [Button] void LateUpdate(){ transform.rotation = Quaternion.identity; }
}