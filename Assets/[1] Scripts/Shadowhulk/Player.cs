using Shadowhulk;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private MapNodeData location;
    public MapNodeData STAT_location => location;
}