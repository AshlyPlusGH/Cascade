using System;
using System.Collections;
using System.Collections.Generic;
using PurrNet;
using UnityEngine;

//namespace Cascade{
public class Player : NetworkBehaviour
{
    [SerializeField] private bool debug;
        private void Log(string contents){ if (debug){ Debug.Log(contents + " at: " + name); }}

    #region Statics
        private static bool STATIC_debug = false;

        //Pointers
        public static Player local => GetLocalPlayer();
        public static PlayerID localID => local.owner.Value;
    
        public static Player GetPlayerFromID(PlayerID id)
        {
            foreach (var player in FindObjectsByType<Player>()){ if (player.owner == id){ return player; }}

            return null;
        }

        public static Player GetLocalPlayer()
        {
            foreach (var player in FindObjectsByType<Player>()){ if (player.isOwner){ return player; }}

                if (STATIC_debug) Debug.Log("No Local Player Found!");
            return null;
        }
    #endregion

    [Space(10)]

    #region Vars-
        //Data
        public bool isBusy = false;

        public Ship selected = null;

        //Events
        public event Action EVENT_ShipSelectionChanged;

        //Components
        public PLAYER_Input input => FindAnyObjectByType<PLAYER_Input>();

        //Pointers
        public List<Ship> friendlyShips => GetAllFriendlyShips();
    #endregion

    void Update()
    {
        DeselectionCheck();
    }

    #region Game State Updaters and Commands
        public void StartDeployment(){}
        public void EndDeployment(){ FindAnyObjectByType<GameManager>().DeploymentCompleted(); }

        [ObserversRpc] public void GameStart()
        {
                if (!isOwner){ return; } //Non-Mirrored Instance only
            input.GameStart(GameManager.instance.GetActivePlayer() == owner);
        }

        [ObserversRpc] public void StartTurn()
        {
                if (!isOwner){ return; }  //Non-Mirrored Instance only
            //Update Inputs and UI
            UI_Manager.instance.StartTurn();
            UI_Cursor.Show();
            input.TurnStart();
            input.Enable();

            RearmFriendlyShips();
        }
        [ObserversRpc] public void EndTurn()
        {
                if (!isOwner){ return; }
            //Update Inputs and UI
            input.TurnEnd();
            UI_Manager.instance.EndTurn();
            UI_Cursor.Hide();

            DealTileDamage();
        }

        void DealTileDamage(){ friendlyShips.ForEach(ship => { ship.CheckForTileDamage(); }); }

        public void RearmFriendlyShips(){ friendlyShips.ForEach(ship => { ship.RecoverMoves(); ship.Reload(); }); }
    #endregion

    #region Ship Controls
        public void TrySelectShip(Ship ship)
        {
                //if (GameManager.instance.GetActivePlayer() != owner){ return; } //Not your turn. No Selecting
                if (isBusy){ return; }
            TryDeselectShip();
                Log("Selecting Ship: " + ship);
            selected = ship;
            selected.Selected();

            if (selected.owner == owner) input.Enable();
            else { input.Disable(); }

            EVENT_ShipSelectionChanged?.Invoke();
        }

        public void TryDeselectShip()
        {
                if (isBusy){ return; }
                if (selected == null){ return; }
            selected.Deselected();
            selected = null;

            input.Disable();

            EVENT_ShipSelectionChanged?.Invoke();
        }
            void DeselectionCheck()
            {
                if (Input.GetMouseButton(1)) TryDeselectShip();
                    return;
            }

        public void TryRotateSelectedShipLeft()
        {
                if (!QueryCanRotateShip()){ return; }
            selected.Rotate(ENUM_RotationDirection.Left);
        }
        public void TryRotateSelectedShipRight()
        {
                if (!QueryCanRotateShip()){ return; }
            selected.Rotate(ENUM_RotationDirection.Right);
        }

        public void TryMoveSelectedShip()
        {
                if (!isOwner) return;
            Log("Trying to start move selected Ship");
                if (isBusy){ Log("Player is Busy!"); return; }
                if (selected == null){ Log("No ship selected!"); return; }
                if (selected.owner != owner){ Log("Does not own this ship!"); return; }

            SetBusy(true);
            UI_Cursor.SwitchMode(ENUM_CursorType.Movement);
            StartCoroutine(MoveSelectedShip());
        }
            private IEnumerator MoveSelectedShip()
            {
                    Log("Trying to move Selected ship");
                while (isBusy)
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                            Log("Ship move order Disregarded!");
                        SetBusy(false);
                        UI_Cursor.SwitchMode(ENUM_CursorType.None);
                        yield break;
                    }
                    if (Input.GetMouseButtonDown(0))
                    {
                        Vector3Int coordinates = Map.GetCoordinatesAtMouse();
                        if (Map.QueryIsTileValidMovement(coordinates))
                        {
                            if (selected.QueryCanTravelTo(coordinates))
                            {
                                selected.Move(coordinates);
                                    Log("Ship move order Given!");
                                SetBusy(false);
                                UI_Cursor.SwitchMode(ENUM_CursorType.None);
                                yield break;
                            }
                            else
                            {
                                    Log("Ship move order Disregarded!");
                                SetBusy(false);
                                UI_Cursor.SwitchMode(ENUM_CursorType.None);
                                yield break;
                            }
                        }
                        else
                        {
                                Log("Ship move order Disregarded!");
                            SetBusy(false);
                            UI_Cursor.SwitchMode(ENUM_CursorType.None);
                            yield break;
                        }
                    }

                    yield return null;
                }

                    Debug.Log("ERROR UNREACHABLE STATEMENT TRIGGERED!");
                UI_Cursor.SwitchMode(ENUM_CursorType.None);
                yield break;
            }

        public void TryFireSelectedShip()
        {
                if (!isOwner) return;
            Log("Trying to start fire selected Ship");
                if (isBusy){ Log("Player is Busy!"); return; }
                if (selected == null){ Log("No ship selected!"); return; }
                if (selected.owner != owner){ Log("Does not own this ship!"); return; }

            SetBusy(true);
            UI_Cursor.SwitchMode(ENUM_CursorType.Combat, selected.STAT_selectedWeapon.area);
            UI_Cursor.instance.SetOutOfRangeTiles(selected.GetCoordinatesInRange());
            UI_Cursor.ShowWeaponRange();
            StartCoroutine(FireSelectedShip());
        }
            IEnumerator FireSelectedShip()
            {
                    Log("Trying to fire Selected ship");
                while (isBusy)
                {
                    //Cancel Fire Order
                    if (Input.GetMouseButtonDown(1))
                    {
                            Log("Cancelling Fire Order!");
                        SetBusy(false); //Local
                        UI_Cursor.SwitchMode(ENUM_CursorType.None); //Outside Class
                        UI_Cursor.HideWeaponRange();
                        yield break;
                    }
                    //Execute Fire Order
                    if (Input.GetMouseButtonDown(0))
                    {
                            Log("Trying to Fire selected Weapon!");
                        selected.TryFire(Map.GetCoordinatesAtMouse());
                            Log("Ship fire order Given!");
                        SetBusy(false);
                        UI_Cursor.HideWeaponRange();
                        UI_Cursor.SwitchMode(ENUM_CursorType.None);
                        yield break;
                    }

                    yield return null;
                }
                    Debug.Log("ERROR UNREACHABLE STATEMENT TRIGGERED!");
                    SetBusy(false);
                    UI_Cursor.SwitchMode(ENUM_CursorType.None);
                    UI_Cursor.HideWeaponRange();
                    yield break;
            }
    #endregion

    #region Queries
        bool QueryCanOrderShip()
        {
                if (isBusy){ Log("Player is Busy!"); return false; }
                if (selected == null){ Log("No ship selected!"); return false; }
                if (selected.owner != owner){ Log("Does not own this ship!"); return false; }
            return true;
        }

        bool QueryCanRotateShip()
        {
                if (!QueryCanOrderShip()){ return false; }
                if (selected.STAT_moves < 1){ Log("No Moves Remaining!"); return false; }
            return true;
        }
    #endregion

    #region Getters and Setters
        public List<Ship> GetAllFriendlyShips()
        {
            List<Ship> friendlyShips = new();

            foreach (var ship in FindObjectsByType<Ship>()){ if (ship.owner == owner){ friendlyShips.Add(ship); }}

            return friendlyShips;
        }

        void SetBusy(bool arg)
        {
            isBusy = arg;
                if (isBusy){ input.Disable(); }
                else{ if (selected != null) input.Enable(); }
        }
    #endregion

    #region DEBUG
        public void DEBUG_SetTurnOrderFirstThis()
        {
            FindAnyObjectByType<GameManager>().SetActivePlayer(owner.Value);
        }
    #endregion
}
//}