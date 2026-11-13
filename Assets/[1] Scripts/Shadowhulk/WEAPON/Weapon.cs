using System;
using System.Collections.Generic;
using NaughtyAttributes;
using PurrNet;
using UnityEngine;

namespace Shadowhulk
{
    //Fires at Closest Enemy Entity once every Cooldown period
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private bool debug;
            [ReadOnly, SerializeField, ShowIf("debug")] private Entity DEBUG_closestTarget;
            [ReadOnly, SerializeField, ShowIf("debug")] private bool DEBUG_readyToFire;
            private void Log(string contents){ if (debug){ Debug.Log(contents + " at: " + name); }}

        [Space(10)]

        [SerializeField] private Entity controllingEntity;
            public PlayerID? owner => controllingEntity.owner;

        [SerializeField] private WEAPON_FiringArc[] arcs;
            void RegisterArcs(){ foreach(var arc in arcs){ arc.RegisterListener(UpdateTargets); }} //Auto Call UpdateTargets
            void UnregisterArcs(){ foreach(var arc in arcs){ arc.UnregisterListener(UpdateTargets); }}

        [ReadOnly] public List<Entity> targets = new();
            public Entity STAT_closestTarget => FindClosestTarget();
            void UpdateTargets() //Auto Called by Arcs
            {
                foreach (WEAPON_FiringArc arc in arcs) targets.AddRange(arc.STAT_entitiesInArea);
                //Filter for Enemies
                //foreach (Entity target in targets) if (target.owner == owner) targets.Remove(target);
                //Filter for Self DEMO
                foreach (Entity target in targets){ if (target == controllingEntity){ targets.Remove(target); }}
            }

        [SerializeField] private int damage = 1;
        [SerializeField] private float cooldown = 1;
            [SerializeField, ReadOnly] private float cooldownTimer;
            void StartCooldown(){ cooldownTimer = 0; }
            void HandleCooldown(){ cooldownTimer += Time.deltaTime; }
            public bool STAT_readyToFire => cooldownTimer > cooldown;

        void Awake(){ Setup(null); }
        public void Setup(Entity newControllingEntity)
        {
            if (controllingEntity == null){ controllingEntity = newControllingEntity; }
            if ((arcs == null) || (arcs?.Length == 0)){ arcs = GetComponentsInChildren<WEAPON_FiringArc>(); }

            RegisterArcs();
        }

        public void Reset()
        {
            if (controllingEntity == null){ controllingEntity = GetComponentInParent<Entity>(); }
            if ((arcs == null) || (arcs?.Length == 0)){ arcs = GetComponentsInChildren<WEAPON_FiringArc>(); }
        }

        void OnDestroy(){ HandleDestroy(); }
        void HandleDestroy(){ UnregisterArcs(); }

        public void Update()
        {
            HandleCooldown();
            TryFire();
        }

        void Fire(Entity target){ target?.Damage(damage); }

        void TryFire()
        {
                if (STAT_closestTarget == null) return;
                if (!STAT_readyToFire) return;
            Fire(STAT_closestTarget);
            StartCooldown();
        }

        private Entity FindClosestTarget()
        {
            Entity toReturn = null;

            foreach (var target in targets)
            {
                    if (toReturn == null){ toReturn = target; continue;}
                float distanceToReturning = Vector2.Distance(transform.position,toReturn.transform.position);
                float distanceToNewTarget = Vector2.Distance(transform.position,target.transform.position);
                if (distanceToNewTarget < distanceToReturning) toReturn = target;
            }

                DEBUG_closestTarget = toReturn;
            return toReturn;
        }

        private bool GetReadyToFireCondition()
        {
            bool toReturn = cooldownTimer > cooldown;
                DEBUG_readyToFire = toReturn;
            return toReturn;
        }
    }
}