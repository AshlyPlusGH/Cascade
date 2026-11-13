using System.Collections.Generic;
using NaughtyAttributes;
using PurrNet;
using UnityEngine;

namespace Shadowhulk{
    //A selectable object with health and a position
    [RequireComponent(typeof(OwnedObject))]
    public class Entity : MonoBehaviour
    {
        [SerializeField] private bool debug;
        [SerializeField, ShowIf("debug")] private bool DEBUG_spawnAsEnemy = false;
            private void Log(string contents){ if (debug){ Debug.Log(contents + " at: " + name); }}

            //     private PlayerID DEBUG_PlayerIDToSet;
            // [Button] void DEBUG_GiveOwnershipToHost(){ ownedObject.SetPlayerID(ownedObject.networkManager.); }
            // [Button] void DEBUG_GiveOwnershipToSpecified(PlayerID){ ownedObject.SetPlayerID(new PlayerID()); }
            // [Button] void DEBUG_GiveOwnershipToNew(){ ownedObject.SetPlayerID(new PlayerID()); }

        [Space(10)]

        #region Component Variables

            [SerializeField] private OwnedObject ownedObject;
            public PlayerID? owner => ownedObject.owner;

            [SerializeField] protected Sensor[] sensors;
            public Sensor[] STAT_sensors => sensors;

            [SerializeField] protected Weapon[] weapons;
            public Weapon[] STAT_weapons => weapons;

        #endregion

        #region Data Variables

            [SerializeField] protected int currentNode = -1;
            public int STAT_currentNode => currentNode;
            //STAT_currentBiome => Map.GetBiomeTypeAtPosition(transform.position);

            [SerializeField] protected int hullPoints = 1;
            public int STAT_hullPoints => hullPoints;
            [SerializeField] protected int armorPoints;
            public int STAT_armorPoints => armorPoints;
            [SerializeField] protected int shieldPoints;
            public int STAT_shieldPoints => shieldPoints;

            //WORKING VARS
            [SerializeField, ReadOnly] protected Vector2 targetPosition;
                private bool hasReachedTargetPosition = true;
            [SerializeField, ReadOnly] private List<Vector2> movementQueue = new();
            
        #endregion

        public virtual void Reset(){ FindReferences(); }
            public virtual void FindReferences()
            {
                if (ownedObject == null) ownedObject = GetComponent<OwnedObject>();
                if ((sensors == null) || (sensors.Length == 0)) sensors = GetComponentsInChildren<Sensor>();
                if ((weapons == null) || (weapons.Length == 0)) weapons = GetComponentsInChildren<Weapon>();
            }

        void Awake(){ Setup(); }
            public virtual void Setup()
            {
                    //if (DEBUG_spawnAsEnemy) ownedObject.SetPlayerID();
                foreach(var sensor in sensors){ sensor.Setup(); }
                foreach(var weapon in weapons){ weapon.Setup(this); }
            }

        public virtual void Update()
        {
            HandleMovement(); //Triggers Move();
        }

        #region Functions

            public virtual void Move(){ Teleport(targetPosition); }
            public virtual void QueueMove(Vector2 newTargetPosition){ movementQueue.Add(newTargetPosition); } //Add TargetPosition to Queue
            public virtual void Teleport(Vector2 teleportTargetPosition){ transform.position = teleportTargetPosition; }
            public virtual void Rotate(){}
            public virtual void RotateSensors(){}
            public virtual void Damage(int damage)
            {
                int shieldDamage = Mathf.Clamp(damage,0,shieldPoints);
                shieldPoints -= shieldDamage;
                damage -= shieldDamage;
                    if (damage <= 0) return;
                int armorDamage = Mathf.Clamp(damage,0,armorPoints);
                armorPoints -= armorDamage;
                damage -= armorDamage;
                    if (damage <= 0) return;
                int hullDamage = Mathf.Clamp(damage,0,hullPoints);
                hullPoints -= hullDamage;
                damage -= hullDamage;
                    if (hullPoints <= 0) Die();
            }
            public virtual void Die(){ Destroy(gameObject); }

        #endregion

        #region Back End Workings

            void HandleMovement()
            {
                    //No Movement Needed
                    if ((hasReachedTargetPosition == true) && ((movementQueue?.Count == 0) || (movementQueue == null))) return;
                //Pop new movement from Queue
                if ((hasReachedTargetPosition == true) && (movementQueue.Count > 0))
                {
                    targetPosition = movementQueue[0];
                    movementQueue.RemoveAt(0);
                    hasReachedTargetPosition = false;
                }

                //Move
                Move();

                //Check if Reached
                if (Vector2.Distance(targetPosition,transform.position) < .5f) hasReachedTargetPosition = true;
            }

        #endregion

        #region Queries

            public bool QueryCanSeeEntity(Entity target){ return true; }

        #endregion

        #region Getters and Setters

            public void ClearMovementQueue(){ movementQueue.Clear(); }

            //STATICS
            public static Entity[] GetAllUnderMouse()
            {
                Entity[] toReturn;

                GameObject[] gameObjectsUnderMouse = MouseOperations.GetGameObjectsWithTriggerCollidersUnderMouse<TAG_ENTITY_Selectable>();
                    if ((gameObjectsUnderMouse == null) || (gameObjectsUnderMouse?.Length == 0)) return null;
                List<Entity> entitiesToReturn = new();
                foreach (GameObject gameObject in gameObjectsUnderMouse)
                {
                    Entity entityFound = gameObject.GetComponent<TAG_ENTITY_Selectable>().STAT_entity;
                        if (entityFound == null) continue;
                    entitiesToReturn.Add(entityFound);
                }
                toReturn = entitiesToReturn.ToArray();

                return toReturn;
            }

            public static Entity[] GetAllFriendlyUnderMouse(PlayerID? friendlyOwner)
            {
                Entity[] toReturn;

                GameObject[] gameObjectsUnderMouse = MouseOperations.GetGameObjectsWithTriggerCollidersUnderMouse<TAG_ENTITY_Selectable>();
                    if ((gameObjectsUnderMouse == null) || (gameObjectsUnderMouse?.Length == 0)) return null;
                List<Entity> entitiesToReturn = new();
                foreach (GameObject gameObject in gameObjectsUnderMouse)
                {
                    Entity entityFound = gameObject.GetComponent<TAG_ENTITY_Selectable>().STAT_entity;
                        if (entityFound == null) continue;
                        if (entityFound.owner != friendlyOwner) continue;
                    entitiesToReturn.Add(entityFound);
                }
                toReturn = entitiesToReturn.ToArray();

                return toReturn;
            }

        #endregion
    }
}