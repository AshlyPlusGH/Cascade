using System;
using UnityEngine;

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
    }
