using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

#pragma warning disable 1998
public class UI : MonoBehaviour
{
    [SerializeField] private GameObject uiParent;
    
    [SerializeField] protected bool fastStart = false;

    [Button] public virtual async UniTask Play()
    {
    }

    [Button]
    public virtual void Show()
    {
        uiParent.SetActive(true);
    }

    [Button]
    public virtual void Hide()
    {
        uiParent.SetActive(false);
    }

    [Button]
    public virtual void Switch()
    {
        if (uiParent.activeInHierarchy) uiParent.SetActive(false);
        else uiParent.SetActive(true);
    }
}