using System;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public class ShipData
{
    public string Name;
    public int HP = 1;
    public int AP = 0;
    public int SP = 0;
    public int Moves = 3;
    public int Squad = 1;
    public soDATA_Weapon[] Weapons = new soDATA_Weapon[3];
    public soDATA_HULL_Lance LanceHull;
    public soDATA_HULL_Broadside LarboardHull;
    public soDATA_HULL_Broadside StarboardHull;
    public soDATA_HULL_Flank FlankHull;
    public soDATA_Ability Ability;
    public string SkinCode = "";

    public string STAT_ShipCode => Ship.GenerateShipCode(this);

    public int GetValue()
    {
        int runningTotal = 0;

        foreach (var weapon in Weapons) if (weapon != null) runningTotal += weapon.cost;
        
        if (LanceHull != null) runningTotal += LanceHull.cost;
        if (LarboardHull != null) runningTotal += LarboardHull.cost;
        if (StarboardHull != null) runningTotal += StarboardHull.cost;
        if (FlankHull != null) runningTotal += FlankHull.cost;

        return runningTotal;
    }
}
    [CreateAssetMenu(fileName = "[Ship] New",menuName = "++Ship")]
    public class soDATA_Ship : ScriptableObject
    {
        public ShipData data;

        [Button]
        public void PrintShipCode(){ Debug.Log(Ship.GenerateShipCode(data)); }
    }