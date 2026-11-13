using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Cascade {
    public class UI_Deployment : UI
    {
        //Components
        [SerializeField] private GameObject shipDeployerParent;
        [SerializeField] private GameObject shipDeployerPrefab;

        [SerializeField] private Button endDeploymentButton;

        //Pointers
        List<ShipData> ships => ACCOUNT_Ships.ships;

        public void Setup(bool isServer)
        {
                //Fallback to DEFAULT Shipcodes if fleet not set
                if (ships.Count < 1)
                {
                    foreach (var ship in ACCOUNT_Ships.staticInputShips)
                    {
                        UI_ShipDeployer newShipDeployer = Instantiate(shipDeployerPrefab,shipDeployerParent.transform).GetComponent<UI_ShipDeployer>();

                        newShipDeployer.Setup(ship, isServer);
                    }
                }
            foreach (var ship in ships)
            {
                UI_ShipDeployer newShipDeployer = Instantiate(shipDeployerPrefab,shipDeployerParent.transform).GetComponent<UI_ShipDeployer>();

                newShipDeployer.Setup(ship.STAT_ShipCode, isServer);
            }
        }

        public void ShipPlaced(){ endDeploymentButton.gameObject.SetActive(true); }

        public override void Show()
        {
            foreach (var item in FindObjectsByType<TAG_TilemapDeploymentZone>())
            {
                item.GetComponent<TilemapRenderer>().enabled = true;
            }

            base.Show();
        }
        public override void Hide()
        {
            foreach (var item in FindObjectsByType<TAG_TilemapDeploymentZone>())
            {
                item.GetComponent<TilemapRenderer>().enabled = false;
            }

            base.Hide();
        }
    }
}