using PurrNet;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;
using System.Threading.Tasks;
using Ash;
using System;

#pragma warning disable 1998
namespace Cascade {
    //Runs any code that is GameState related and holds commonly used methods not relating to any one system.
    public class GameManager : NetworkBehaviour
    {
        #region DEBUG
            [SerializeField] private bool debug;
                private void Log(string contents){ if (debug){ Debug.Log(contents); }}
                private void LogColor(string contents, string color = "white"){ if (debug){ Debug.Log($"<color={color}>{contents}</color>"); }}
                    //Colors:
                    //white
                    //blue
                    //red
                    //black
                    //green
                    //yellow
                [ObserversRpc(requireServer:false)] public void LogServerwide(string contents){ Log($"<color=yellow>Message from Server: </color>" + contents); }
                [ObserversRpc(requireServer:false)] public void LogColorServerwide(string contents, string color = "yellow"){ LogColor("Message from Server: " + contents, color); }
        #endregion

        //Static
        #region Static Vars
            //Public
            public static GameManager instance => FindAnyObjectByType<GameManager>();

            //Class Only
            private static PlayerID localPlayerID => instance.networkManager.localPlayer;

            public static bool gameInProgress {get; private set;} = false;
        #endregion

        #region Static Methods
            static void ResetStatics()
            {
                Ship.ResetStatics();
                Map.ResetStatics();
                gameInProgress = false;
            }
        #endregion

        //Class Instance
        #region Vars
            //Resource Vars
            public NetworkIdentity shipPrefab;

            //Serverside Workings
            private SyncDictionary<PlayerID, bool> readyClients = new();

                private SyncVar<PlayerID> activePlayerSyncVar = new();
            private PlayerID activePlayer => activePlayerSyncVar.value;
            private Player activePlayerClass => Player.GetPlayerFromID(activePlayer);
                private SyncVar<PlayerID> inactivePlayerSyncVar = new();
            private PlayerID inactivePlayer => inactivePlayerSyncVar.value;
            private Player inactivePlayerClass => Player.GetPlayerFromID(inactivePlayer);
        #endregion

        protected void Update(){ if (queueCheckForWinner){ CheckForWinner(); queueCheckForWinner = false; }}
        protected override void OnSpawned(){ base.OnSpawned(); UpdateClientList(); } 
            //Update Servers Records when I as a client have joined Game!
            //Doesnt run Serverside as GameManager starts spawned!

        #region Round Setup
            [ServerRpc] public async void StartRound() //Start -> Deployment Start
            {
                ResetStatics();

                    LogColorServerwide("Server: Starting Round!", "yellow");

                    UnreadyAllPlayers();
                UPDATECLIENTS_StartRound(Map.GetRandomSeed());
                    await AwaitAllReady();

                StartDeployment();
            }
                //Actions taken locally on clients
                [ObserversRpc] private async void UPDATECLIENTS_StartRound(Int32 mapSetSeed)
                {
                        LogColor("Starting Round!", "blue");
                    Map.CreateRandom(mapSetSeed);

                    await UpdateOpponentUsername();

                    await FindAnyObjectByType<UI_Manager>().StartRound();

                        LogServerwide(Account.username + "'s ships: ");
                        foreach (var ship in ACCOUNT_Ships.ships){ LogServerwide(Ship.GenerateShipCode(ship)); }
            
                    ReadyLocalPlayer();
                }

            [ServerRpc] public async void StartDeployment()
            {
                    LogColorServerwide("Server: Starting Deployment!", "yellow");

                    UnreadyAllPlayers();
                UPDATECLIENTS_StartDeployment();
                    await AwaitAllReady();

                StartTurnOrderDecision();
            }
                [ObserversRpc] private async void UPDATECLIENTS_StartDeployment()
                {
                        LogColor("Starting Deployment!", "blue");
                    FindAnyObjectByType<UI_Manager>().StartDeployment(isServer);
                    Player.local.StartDeployment();
                }
                public void DeploymentCompleted(){ UI_Manager.instance.EndDeployment(); ReadyLocalPlayer(); }

            [ServerRpc] public async void StartTurnOrderDecision()
            {
                    LogColorServerwide("Server: Starting Turn Order Decision!", "yellow");

                    UnreadyAllPlayers();
                    SetActivePlayer(default); //Default to tell when assigned by decision
                    SetInactivePlayer(default); //Default to tell when assigned by decision
                UPDATECLIENTS_StartTurnOrderDecision();
                    await AwaitAllReady();

                    LogColorServerwide("Server: First turn goes to: " + activePlayer + "!", "yellow");
                StartFirstTurn();
            }
                [ObserversRpc] private async void UPDATECLIENTS_StartTurnOrderDecision()
                {
                        LogColor("Starting Turn Order Decision!", "blue");
                    FindAnyObjectByType<UI_Manager>().StartTurnOrderDecision();
                    await UniTask.WaitUntil(() => activePlayer != default);
                    FindAnyObjectByType<UI_Manager>().EndTurnOrderDecision();

                    ReadyLocalPlayer();
                }

            [ServerRpc] public async void StartFirstTurn()
            {
                    LogColorServerwide("Server: Starting First Turn!", "yellow");
                activePlayerClass.GameStart();
                inactivePlayerClass.GameStart();

                activePlayerClass.StartTurn();

                UPDATECLIENTS_SignalGameInProgress();
            }

            [ObserversRpc] public void UPDATECLIENTS_SignalGameInProgress(){ gameInProgress = true; }
        #endregion

        #region Game State Methods
            [ServerRpc(requireOwnership:false)] public async void PassTurn()
            {
                    Log("Passing Turn!");
                SwapActivePlayers();
                
                inactivePlayerClass.EndTurn();
                activePlayerClass.StartTurn();
            }
                private void UpdateInactivePlayer(){ foreach (var player in FindObjectsByType<Player>()){ if (player.owner != activePlayer){ SetInactivePlayer(player.owner.Value); }}}
                private void SwapActivePlayers()
                {
                        UpdateInactivePlayer();
                        PlayerID nextActivePlayer = inactivePlayer; //Temp Storage to avoid Overwrite
                    SetInactivePlayer(activePlayer);
                    SetActivePlayer(nextActivePlayer);
                }
        
                [ObserversRpc(requireServer:false)] public async void EndGame(PlayerID? winner)
                {
                    if (isServer)
                    {
                        while (networkManager.playerCount > 1)
                        {
                            await Task.Yield(); // or await Task.Delay(100);
                        }
                    }

                    if (localPlayer == winner){ Win(); }
                    else { Lose(); }
                }
                void Win(){ SCENE.LoadScene("win"); Log("You Win!"); SCENE.UnloadScene("main"); }
                void Lose(){ SCENE.LoadScene("lose"); Log("You Lose!"); SCENE.UnloadScene("main"); }
        #endregion

        #region Server Commands
            [ServerRpc(requireOwnership:false)] public void SpawnShip(string shipCode, Vector3Int coordinates, PlayerID shipOwner = default)
            {
                    if (shipCode == null){ return; }
                    if (shipOwner == default){ shipOwner = localPlayer.Value; }
                Quaternion targetRotation = Quaternion.identity;
                if (shipOwner != localPlayer.Value){ targetRotation = Quaternion.Euler(0f, 0f, 180f); } //Flip Enemy Ships around
                NetworkIdentity newShip = Instantiate(shipPrefab, Map.GetPositionAtCoordinates(coordinates), targetRotation);
                newShip.GiveOwnership(shipOwner);
                newShip.GetComponent<Ship>().Setup(shipCode);
            }
            [ServerRpc(requireOwnership:false)] public void CheckForWinner()
            {
                    Log("Checking for Winner!");
                PlayerID? winner = null;
                foreach (var ship in FindObjectsByType<Ship>())
                {
                    if (winner == null){ winner = ship.owner; continue; }
                    if (ship.owner != winner){ return; } //No Winner Opposition Detected!
                }

                    Log("Triggering End state!");
                EndGame(winner);
            }
                bool queueCheckForWinner = false;
                [ServerRpc(requireOwnership:false)] public void ShipDestroyed(){ queueCheckForWinner = true; }
        #endregion

        #region Server Function Utility
            [ServerRpc(requireOwnership:false)] void UpdateClientList()
            {
                readyClients.Clear();
                foreach (var playerID in networkManager.players){ readyClients.Add(playerID,true); }
                
                //Round Starter
                if (networkManager.playerCount > 1){ StartRound(); }
            }
            
            //READIED PLAYER SYSTEM
            public void ReadyLocalPlayer(){ ReadyPlayer(localPlayerID); }
            [ServerRpc(requireOwnership:false)] void ReadyPlayer(PlayerID client){ Log("GM is Ready at: " + client); readyClients[client] = true; }
            [ServerOnly] void UnreadyAllPlayers()
            {
                    LogColorServerwide("Server: Unreadying Serverside Client List!", "yellow");
                if (readyClients.Count < 1){ Log("No Clients Found Resetting Dictionary!"); UpdateClientList(); }
                if (readyClients.Count < 1){ Log("No Clients Found Cancelling Request!"); return; }
                foreach (var client in readyClients.ToList()){ Log("GM at: " + client.Key + " is set not Ready!"); readyClients[client.Key] = false; }
            }
            private async UniTask AwaitAllReady(){ await UniTask.WaitUntil(() => readyClients.All(kvp => kvp.Value)); Log("All Ready!"); }
        #endregion

        #region Getters and Setters
            [TargetRpc(requireServer:false)] public async Task<string> GetUsernameFromPlayer(PlayerID target){ return Account.username; }
            public string opponentUsername = "ERROR: Unset";
            public static string GetOpponentUsername(){ return FindAnyObjectByType<GameManager>().opponentUsername; }
            public async Task<bool> UpdateOpponentUsername()
            {
                PlayerID opponentPlayerID = networkManager.players[1];
                foreach (PlayerID id in networkManager.players){ if (id != localPlayer){ opponentPlayerID = id; break; }}
                opponentUsername = await GetUsernameFromPlayer(opponentPlayerID);
                    return true;
            }

            public PlayerID GetActivePlayer(){ return activePlayer; }
            [ServerRpc(requireOwnership:false)] public void SetActivePlayer(PlayerID toSet){ activePlayerSyncVar.value = toSet; if (inactivePlayer == default) foreach(var playerID in networkManager.players) if (playerID != activePlayer) SetInactivePlayer(playerID); }
            public PlayerID GetInactivePlayer(){ return activePlayer; }
            [ServerRpc(requireOwnership:false)] public void SetInactivePlayer(PlayerID toSet){ inactivePlayerSyncVar.value = toSet; }
        #endregion
    }
}