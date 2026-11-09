using UnityEngine;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine.UI;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] private bool debug;
        private void Log(string contents){ if (debug){ Debug.Log(contents + " at: " + name); }}

    public static UI_Manager instance => FindAnyObjectByType<UI_Manager>();

    [Space(10)]

    [SerializeField] private TextMeshProUGUI gameStatusTMP;

    [SerializeField] private TextMeshProUGUI localPlayerTMP;
    [SerializeField] private TextMeshProUGUI opponentTMP;

    [SerializeField] private UI startRoundUI;
    [SerializeField] private UI_Deployment deploymentUI;
    [SerializeField] private UI turnOrderUI;

    #region Start/Enable UI
        [Button] public async UniTask StartRound()
        {
                Log("Playing Start Round Effects!");
            localPlayerTMP.text = Account.username;
            opponentTMP.text = GameManager.instance.opponentUsername;

            await startRoundUI.Play();
        }

        public void StartDeployment(bool isServer)
        {
            deploymentUI.Setup(isServer);
            deploymentUI.Show();
        }
        public void EndDeployment()
        {
            deploymentUI.Hide();
        }

        public void StartTurnOrderDecision()
        {
            turnOrderUI.Show();
        }
        public void EndTurnOrderDecision()
        {
            turnOrderUI.Hide();
        }

        public void StartTurn(){ gameStatusTMP.text = "Turn Ongoing"; }
        public void EndTurn(){ gameStatusTMP.text = "Waiting for Opponent..."; }
    #endregion
}