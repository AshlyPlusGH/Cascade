using System;
using UnityEngine;

namespace Cascade {
    [Serializable]
    public struct Weapon
    {
        public string weaponName;
        public int damage;
        public ENUM_WeaponType type;
        public int salvo;
        public int range;
        public Vector3Int[] area;
    }
        [CreateAssetMenu(fileName = "[Weapon] New",menuName = "++Weapon")]
        public class soDATA_Weapon : ScriptableObject
        {
            public Weapon data;
            
            public string description;
            public Sprite icon;
            public int cost;

            public string GetDataParagraph()
            {
                string toReturn = "";

                toReturn += "\n" + "<color=red>" + "Damage: " + data.damage + "</color>";
                toReturn += "\n" + "<color=green>" + "Type: " + data.type + "</color>";
                toReturn += "\n" + "<color=yellow>" + "Salvo: " + data.salvo + "</color>";
                toReturn += "\n" + "<color=blue>" + "Range: " + data.range + "</color>";
                //toReturn += "\n" + "Area: " + data.area;

                return toReturn;
            }
        }
}