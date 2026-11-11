using UnityEngine;

namespace Shadowhulk{
    public class Player : MonoBehaviour
    {
        [SerializeField] private bool debug;
            private void Log(string contents){ if (debug){ Debug.Log(contents + " at: " + name); }}

        [Space(10)]
        
        [SerializeField] private int currentNode;
        public int STAT_currentNode => currentNode;

        [SerializeField] private Ship selectedShip;
        public Ship STAT_selectedShip => selectedShip;

        [SerializeField] private GenericDictionary<ENUM_EquipmentType,int> equipmentInventory = new();
        public GenericDictionary<ENUM_EquipmentType,int> STAT_equipmentInventory => equipmentInventory;

        #region Controls
            
            //Generic
            void Ping(Vector3 location){} //Clear last ping create new ping call ships to update reference rotation if following ping
            void UseEquipment(ENUM_EquipmentType type) //Starts Use Equipment Pop Up to allow for selection of target/location if applicable else use straight away
            {
                    if (!equipmentInventory.ContainsKey(type)) return; //No Equipment Found of that type!
            }

            //Ships
            //void SelectShip(Ship ship){ selectedShip = ship; }
            void MoveShip(Vector3 targetPosition){}
            void MoveShipAddToQueue(Vector3 targetPosition){}
            void SetShipRelativeRotation(Quaternion relativeRotation){}
            void SetShipRotationReference(){} //Map North //Ping //Ship Front

        #endregion
    }
}