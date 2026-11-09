using UnityEngine;

public class TransformOperations
{
    public static void DestroyChildren(Transform parent){ foreach(Transform child in parent){ Object.Destroy(child.gameObject); }}

    public static void SetRectWidth(RectTransform target, float width){ target.sizeDelta = new Vector2(width,target.sizeDelta.y); }
    public static void SetRectHeight(RectTransform target, float height){ target.sizeDelta = new Vector2(target.sizeDelta.x,height); }
    public static void SetRectSize(RectTransform target, Vector2 newSize){ target.sizeDelta = newSize; }
}