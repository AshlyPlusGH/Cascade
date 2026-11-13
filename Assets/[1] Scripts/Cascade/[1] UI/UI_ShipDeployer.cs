using UnityEngine;
using UnityEngine.UI;
using System;

namespace Cascade {
    public class UI_ShipDeployer : MonoBehaviour
    {
        [SerializeField] private bool debug;
            private void Log(string contents){ if (debug){ Debug.Log(contents); }}

        private static UI_ShipDeployer current;

        #region Vars
            [SerializeField] private Button button;
            [SerializeField] private GameObject placeGhostPrefab;

            private GameObject placeGhost;

            [SerializeField] private bool isDeploying = false;
            [SerializeField] private string shipCode;
            [SerializeField] private string skinCode;

            Vector3 spawnPos;

            bool isServer;
        #endregion

        public void Setup(string setupShipCode, bool portedIsServer)
        {
            isServer = portedIsServer;
                if (!isServer) transform.Rotate(0f, 0f, 180f);
            shipCode = setupShipCode;
                string[] codes = shipCode.Split('_', StringSplitOptions.None);
                skinCode = codes[codes.Length-1];

            button.image.sprite = Ship.LoadSkinFromResources(skinCode);
            button.onClick.AddListener(StartDeploying);
        }

        void SpawnPlaceGhost()
        {
                Log("Spawning Place Ghost with Skin: " + skinCode);
            placeGhost = Instantiate(placeGhostPrefab);
                if (!isServer) placeGhost.transform.Rotate(0f, 0f, 180f);
            placeGhost.GetComponent<SpriteRenderer>().sprite = Ship.LoadSkinFromResources(skinCode);
        }
        void DestroyPlaceGhost(){ Destroy(placeGhost); }

        public void Update()
        {
            UpdateSpawnPos();

            if (isDeploying){ Deploying(); if (Input.GetMouseButtonDown(0)){ TryDeploy(); }}
        }
        void UpdateSpawnPos()
        {
                Vector3 newPos = Map.GetTilePositionAtMouse();
                int[] zoneCodes;
                if (GameManager.instance.isHost) zoneCodes = new int[]{1};
                else zoneCodes = new int[]{2};
            if (!Map.QueryIsTileValidDeployment(Map.GetCoordinatesAtPosition(newPos), zoneCodes)){ return; }

            spawnPos = newPos;
        }

        void StartDeploying(){ if (current == null){ current = this; } else { return; } button.enabled = false; isDeploying = true; SpawnPlaceGhost(); }

        void Deploying()
        {
            UpdatePlaceGhostPosition();
        }
            void UpdatePlaceGhostPosition(){ placeGhost.transform.position = spawnPos; }

        void TryDeploy()
        {
                Vector3 newPos = Map.GetTilePositionAtMouse();
                int[] zoneCodes;
                if (GameManager.instance.isHost) zoneCodes = new int[]{1};
                else zoneCodes = new int[]{2};
            if (!Map.QueryIsTileValidDeployment(Map.GetCoordinatesAtPosition(newPos), zoneCodes)){ return; }//Check if in deployment zone before allowing deployment

            FindAnyObjectByType<GameManager>().SpawnShip(shipCode,Map.GetCoordinatesAtPosition(spawnPos),Player.local.owner.Value);
            DestroyPlaceGhost();

            isDeploying = false;

            current = null;

                FindAnyObjectByType<UI_Deployment>().ShipPlaced();
            Destroy(gameObject);
        }
    }
}