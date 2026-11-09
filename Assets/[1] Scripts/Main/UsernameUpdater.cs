using UnityEngine;
using TMPro;

public class UsernameUpdater : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    void Awake(){ Setup(); }
    void Setup(){ inputField.text = Account.username; }

    public void UpdateUsername(string input){ Account.username = input; }
}