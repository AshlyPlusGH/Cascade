using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Shadowhulk
{
    public class WEAPON_FiringArc : MonoBehaviour
    {
        [SerializeField] private bool debug;
            private void Log(string contents){ if (debug){ Debug.Log(contents + " at: " + name); }}

        [Space(10)]

        [SerializeField] private Collider2D area;
        public Collider2D STAT_area => area;

        [SerializeField, ReadOnly] private List<Entity> entitiesInArea = new();
        public List<Entity> STAT_entitiesInArea => entitiesInArea;

        private event Action onArcContentsChanged;

        void Reset(){ if (area == null) area = GetComponent<Collider2D>(); }

        public void OnTriggerEnter2D(Collider2D enteringObject)
        {
                Log("Object Entering Firing Arc: " + enteringObject);
            Entity enteringEntity = enteringObject?.GetComponent<Entity>();
                Log("Entity Entering Firing Arc: " + enteringEntity);
                if (enteringEntity == null) return;
                Log("Entity Entering Firing Arc is Valid!");
            if (!entitiesInArea.Contains(enteringEntity)) entitiesInArea.Add(enteringEntity);
            onArcContentsChanged?.Invoke();
        }
        public void OnTriggerExit2D(Collider2D leavingObject)
        {
                Log("Object Leaving Firing Arc: " + leavingObject);
            Entity leavingEntity = leavingObject?.GetComponent<Entity>();
                if (leavingEntity == null) return;
            entitiesInArea.Remove(leavingEntity);
            onArcContentsChanged?.Invoke();
        }

        public void RegisterListener(Action listener){ onArcContentsChanged += listener; }
        public void UnregisterListener(Action listener){ onArcContentsChanged -= listener; }
    }
}