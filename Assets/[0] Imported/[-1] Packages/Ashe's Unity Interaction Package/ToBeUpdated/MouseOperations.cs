using System.Collections.Generic;
using UnityEngine;

public class MouseOperations
{
    public static GameObject[] GetGameObjectsWithTriggerCollidersUnderMouse<T>(bool requireComponent = true)
        where T : Component
    {
            if (!IsValid(Input.mousePosition)) return null;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Collider2D[] hits = Physics2D.OverlapPointAll(worldPos);
        List<GameObject> results = new List<GameObject>();

        foreach (var c in hits)
        {
            if (!c.isTrigger)
                continue;

            if (!requireComponent || c.GetComponent<T>() != null)
                results.Add(c.gameObject);
        }

        return results.ToArray();
    }

    public static GameObject[] GetGameObjectsWithTriggerCollidersUnderMouse()
    {
            if (!IsValid(Input.mousePosition)) return null;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D[] hits = Physics2D.OverlapPointAll(worldPos);

        var results = new List<GameObject>();
        foreach (var c in hits)
        {
            if (c.isTrigger)
                results.Add(c.gameObject);
        }

        return results.ToArray();
    }

    public static bool IsMousePositionValid(){ return IsValid(Input.mousePosition); }

    private static bool IsValid(Vector3 v){ return !(float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z) || float.IsInfinity(v.x) || float.IsInfinity(v.y) || float.IsInfinity(v.z)); }
}