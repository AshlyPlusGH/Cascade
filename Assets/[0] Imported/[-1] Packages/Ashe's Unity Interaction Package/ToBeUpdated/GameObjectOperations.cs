
using UnityEngine;

public class GameObjectOperations : MonoBehaviour
{
    public void SwitchActiveInHeirarchy(){ if (gameObject.activeInHierarchy) gameObject.SetActive(false); else gameObject.SetActive(true); }
}