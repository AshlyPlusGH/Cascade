using Shadowhulk;
using UnityEngine;

public class TAG_ENTITY_Selectable : MonoBehaviour
{
    [SerializeField] private Entity entity;
    public Entity STAT_entity => entity;

    public void Retset(){ if (entity == null) entity = GetComponentInParent<Entity>(); }
}