using UnityEngine;
using UnityEngine.UI;
namespace Cascade {
    public class PLAYER_Input : MonoBehaviour
    {
        [SerializeField] private bool debug;
            private void Log(string contents){ if (debug){ Debug.Log(contents + " at: " + name); }}

        [Space(10)]

        #region Vars
            //Object References
            [SerializeField] private GameObject parent;
            [SerializeField] private GameObject endTurnButton;

            [SerializeField] private Button[] buttonsDisabledOutsideTurn;

            public Ship selected => Player.local.selected;
        #endregion

        void Update()
        {
                if (Player.local == null) return;
                if (selected == null) return;
                if (Player.local.isBusy) return;
            //Select Weapon Controls
            if (Input.mouseScrollDelta.y > 0) selected.CycleSelectedWeapon(1);
            if (Input.mouseScrollDelta.y < 0) selected.CycleSelectedWeapon(-1);

            if (Input.GetKeyDown(KeyCode.Alpha1)) selected.SetSelectedWeapon(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) selected.SetSelectedWeapon(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) selected.SetSelectedWeapon(2);
        }

        #region Update Controls UI
            public void GameStart(bool isActivePlayer)
            {
                if (!isActivePlayer) TurnEnd();
            }
            public void Enable()
            {
                    Log("Trying to enable Controls!");
                    if (selected == null){ Log("No Ships Selected!"); return; }
                    if (!selected.isLocal){ Log("Selected Ship is an Enemy!"); return; }
                parent.SetActive(true);
            }
            public void Disable()
            {
                    Log("Trying to disable Controls!");
                parent.SetActive(false);
            }

            public void TurnStart()
            {
                foreach (var button in buttonsDisabledOutsideTurn){ button.interactable = true; }

                endTurnButton.SetActive(true);
            }
            public void TurnEnd()
            {
                endTurnButton.SetActive(false);

                foreach (var button in buttonsDisabledOutsideTurn){ button.interactable = false; }
            }
        #endregion

        #region Control Transport
            //Game State
            public void StartRound(){ FindAnyObjectByType<GameManager>().StartRound(); }
            
            public void IGoFirst(){ FindAnyObjectByType<GameManager>().SetActivePlayer(Player.localID); }

            public void EndDeployment(){ FindAnyObjectByType<GameManager>().DeploymentCompleted(); }

            //Ships
            public void Move(){ Player.local.TryMoveSelectedShip(); }
            public void Fire(){ Player.local.TryFireSelectedShip(); }
            public void RotateLeft(){ Player.local.TryRotateSelectedShipLeft(); }
            public void RotateRight(){ Player.local.TryRotateSelectedShipRight(); }
        #endregion
    }
}