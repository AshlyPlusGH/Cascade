using NaughtyAttributes;
using UnityEngine;
using PurrNet;

/*
public class GAMEMANAGER_DEBUG : MonoBehaviour
{
    #region Vars
        [SerializeField] private GameManager gameManager;

        [Label("Ship Spawning")]
        [SerializeField] private string shipCodeToSpawn;
        [SerializeField] private Vector3Int posToSpawn;
    #endregion

    #region Methods
        [Button] public void StartRound(){ gameManager.StartRound(); }
        [Button] public void CheckForWinner(){ gameManager.CheckForWinner(); }

        [Button] public void SpawnDefaultShipLocalOwner(){ gameManager.SpawnShip(shipCodeToSpawn, posToSpawn); }
        [Button] public void SpawnDefaultShipNonLocalOwner()
        {
                PlayerID nonLocalPlayer = default;
            foreach (var player in gameManager.networkManager.players){ if (player != gameManager.owner){ nonLocalPlayer = player; }}
            gameManager.SpawnShip(shipCodeToSpawn, posToSpawn, nonLocalPlayer); 
        }
    #endregion
}
*/