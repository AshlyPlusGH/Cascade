using UnityEngine;

public class DDoL : MonoBehaviour
{
    void Awake(){ DontDestroyOnLoad(transform); }
}