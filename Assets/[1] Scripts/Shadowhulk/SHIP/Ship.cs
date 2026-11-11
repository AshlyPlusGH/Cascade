using System.Collections.Generic;
using UnityEngine;

namespace Shadowhulk
{
    public class Ship : MonoBehaviour
    {
        [SerializeField] private bool debug;
            private void Log(string contents){ if (debug){ Debug.Log(contents + " at: " + name); }}

        [Space(10)]

        [SerializeField] private int currentNode;
        public int STAT_currentNode => currentNode;

        [SerializeField] private int hullPoints;
        public int STAT_hullPoints => hullPoints;
        [SerializeField] private int armorPoints;
        public int STAT_armorPoints => armorPoints;
        [SerializeField] private int shieldPoints;
        public int STAT_shieldPoints => shieldPoints;

        private SHIP_Thruster[] thrusters;

        void Awake(){ Setup(); }
        public void Setup(){  }

        #region Functions

            void Move(){}
            void Teleport(){}
            void Rotate(){}
            void RotateSensors(){}
            void Damage(){}
            void Die(){}

        #endregion

        #region Queries

            //public ENUM_BiomeType GetOccupyingBiomeType(transform.position, STAT_currentNode){ return Map.GetBiomeTypeAtPosition(); }

        #endregion
    }
}