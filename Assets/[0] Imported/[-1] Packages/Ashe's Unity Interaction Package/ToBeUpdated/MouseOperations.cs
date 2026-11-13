using System.Collections.Generic;
using UnityEngine;

public class MouseOperations
{
    public static Vector3 mousePositionWorld => GetMousePositionWorld();

    public static GameObject[] GetGameObjectsWithTriggerCollidersUnderMouse<T>(bool requireComponent = true)
        where T : Component
    {
            if (!IsValid(Input.mousePosition)) return null;
        Collider2D[] hits = Physics2D.OverlapPointAll(mousePositionWorld);
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
        Collider2D[] hits = Physics2D.OverlapPointAll(mousePositionWorld);

        var results = new List<GameObject>();
        foreach (var c in hits)
        {
            if (c.isTrigger)
                results.Add(c.gameObject);
        }

        return results.ToArray();
    }

    public static Vector3 GetMousePositionWorld()
    {
            Camera camera = Camera.main;
            if (camera == null) camera = Object.FindAnyObjectByType<Camera>();
                if (camera == null){ Debug.Log("No Camera found in Scene"); return Vector3.zero; }
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public static bool IsMousePositionValid(){ return IsValid(Input.mousePosition); }

    private static bool IsValid(Vector3 v){ return !(float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z) || float.IsInfinity(v.x) || float.IsInfinity(v.y) || float.IsInfinity(v.z)); }
}