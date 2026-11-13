using UnityEngine;
using UnityEngine.UI;

public class UI_ScrollingImage : MonoBehaviour
{
    [SerializeField] private RawImage target;
    [SerializeField] private Vector2 speed = new Vector2(1,1);

    void Update(){ target.uvRect = new Rect(target.uvRect.position + speed*Time.deltaTime,target.uvRect.size); }
}