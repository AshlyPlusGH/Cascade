using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class UI_Tooltip : MonoBehaviour
{
    [field:SerializeField, TextArea] public string contents {get; private set;}
    [field:SerializeField] public int priority {get; private set;}
}