using System;
using NaughtyAttributes;
using UnityEngine;

namespace Cascade {
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
        public string SkinCode = "Time";

        public string STAT_ShipCode => Ship.GenerateShipCode(this);

        public string GetDataParagraph()
        {
            string toReturn = "";

            int hp = HP
                + (LanceHull?.hp ?? 0)
                + (LarboardHull?.hp ?? 0)
                + (StarboardHull?.hp ?? 0)
                + (FlankHull?.hp ?? 0);

            int ap = AP
                + (LanceHull?.ap ?? 0)
                + (LarboardHull?.ap ?? 0)
                + (StarboardHull?.ap ?? 0)
                + (FlankHull?.ap ?? 0);

            int sp = SP
                + (LanceHull?.sp ?? 0)
                + (LarboardHull?.sp ?? 0)
                + (StarboardHull?.sp ?? 0)
                + (FlankHull?.sp ?? 0);

            int moves = Moves
                + (LanceHull?.speed ?? 0)
                + (LarboardHull?.speed ?? 0)
                + (StarboardHull?.speed ?? 0)
                + (FlankHull?.speed ?? 0);

            toReturn += "\n" + "<color=white>" + "ID: " + Name + "</color>";
            toReturn += "\n" + "<color=red>" + "HP: " + hp + "</color>";
            toReturn += "\n" + "<color=yellow>" + "AP: " + ap + "</color>";
            toReturn += "\n" + "<color=blue>" + "SP: " + sp + "</color>";
            toReturn += "\n" + "<color=green>" + "Speed: " + moves + "</color>";

            return toReturn;
        }

        public int GetValue()
        {
            int runningTotal = 0;

            foreach (var weapon in Weapons) if (weapon != null) runningTotal += weapon.cost;
            
            if (LanceHull != null) runningTotal += LanceHull.cost;
            if (LarboardHull != null) runningTotal += LarboardHull.cost;
            if (StarboardHull != null) runningTotal += StarboardHull.cost;
            if (FlankHull != null) runningTotal += FlankHull.cost;

            if (Ability != null) runningTotal += Ability.cost;

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
}