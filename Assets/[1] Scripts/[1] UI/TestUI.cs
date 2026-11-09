using UnityEngine;
using UnityEngine.UIElements;

public class TestUI : MonoBehaviour
{
    [field: SerializeField] private UIDocument classification;
    
    [field: SerializeField] private Button button;

    public void Awake()
    {
        button = classification.rootVisualElement.Q<Button>("DaButton");
        button.clicked += OnPress;
    }

    public void OnPress()
    {
        Debug.Log("FSJifan");
    }
}