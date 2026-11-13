using System.Collections.Generic;
using UnityEngine;

namespace Cascade {
    public class ACCOUNT_Ships
    {
        public static void SetShips(List<ShipData> newShips){ ships = newShips; }

        public static List<ShipData> ships = new();

        public static int fleetValue = -1;
        
        //"Name_HP_AP_SP_Speed_Weapons...._SkinCode"
        public static List<string> staticInputShips = new()
        {
            //"Sasorii_7_4_9_2_0_8.3.1.6._Sasorii",
            "Zahur_4_4_4_3_1_Basic Plasma.1.4.1.2.,Basic Plasma.1.4.1.2.,Basic Plasma.1.4.1.2._2",
            "Zahinr_4_4_4_3_1_Basic Plasma.1.4.1.2.,Basic Plasma.1.4.1.2.,Basic Plasma.1.4.1.2._2",
        };

        public static void PrintShips()
        {
            foreach (var ship in ships) Debug.Log(ship.STAT_ShipCode);
        }
    }
}