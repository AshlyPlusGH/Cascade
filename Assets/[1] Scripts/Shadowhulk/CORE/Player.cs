using PurrNet;
using UnityEngine;

namespace Shadowhulk{
    [RequireComponent(typeof(OwnedObject))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private bool debug;
            private void Log(string contents){ if (debug){ Debug.Log(contents + " at: " + name); }}

        [Space(10)]

        [SerializeField] private OwnedObject ownedObject;
            private PlayerID? owner => ownedObject.owner;
        
        [SerializeField] private int currentNode = -1;
        public int STAT_currentNode => currentNode;

        [SerializeField] private Entity selected;
        public Entity STAT_selected => selected;
        public Ship STAT_selectedShip => selected as Ship;

        [SerializeField] private GenericDictionary<ENUM_EquipmentType,int> equipmentInventory = new();
        public GenericDictionary<ENUM_EquipmentType,int> STAT_equipmentInventory => equipmentInventory;

        void Reset()
        {
            if (ownedObject == null) ownedObject = GetComponent<OwnedObject>();
        }

        void Update()
        {
            CheckInputs();
        }

        #region Functions
            
            //Generic
            void Ping(Vector3 location){} //Clear last ping create new ping call ships to update reference rotation if following ping
            void UseEquipment(ENUM_EquipmentType type) //Starts Use Equipment Pop Up to allow for selection of target/location if applicable else use straight away
            {
                    if (!equipmentInventory.ContainsKey(type)) return; //No Equipment Found of that type!
            }
            void Select(Entity newSelected){ selected = newSelected; }
            void Deselect(){ selected = null; }

            //Ships
            void MoveShip(Vector3 targetPosition){}
            void MoveShipAddToQueue(Vector3 targetPosition){}
            void SetShipRelativeRotation(Quaternion relativeRotation){}
            void SetShipRotationReference(){} //Map North //Ping //Ship Front

        #endregion

        #region Back End Workings

            void CheckInputs()
            {
                //Get Inputs
                bool lMouseButtonDown = Input.GetMouseButtonDown(0);
                bool rMouseButtonDown = Input.GetMouseButtonDown(1);
                bool queueActionKeyHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                Entity[] friendlyEntitiesUnderMouse = Entity.GetAllFriendlyUnderMouse(owner);

                //Update Operations
                //Select
                if (lMouseButtonDown)
                {
                    if ((friendlyEntitiesUnderMouse == null) || (friendlyEntitiesUnderMouse?.Length == 0)) Deselect();
                    else Select(friendlyEntitiesUnderMouse[0]);
                }
                //Give Orders
                if (rMouseButtonDown)
                {
                    if (!queueActionKeyHeld) STAT_selectedShip?.ClearMovementQueue();
                    STAT_selectedShip?.QueueMove(MouseOperations.mousePositionWorld);
                }
            }

        #endregion

        #region Getters and Setters

        #endregion
    }
}