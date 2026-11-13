using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cascade
{
    public class AllowPlayWhenFleetPresent : MonoBehaviour
    {
        [SerializeField] private bool debug;
            private void Log(string contents){ if (debug){ Debug.Log(contents + " at: " + name); }}

        [Space(10)]

        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI text;

        void Awake(){ Setup(); }
        public void Setup()
        {
            if (ACCOUNT_Ships.ships.Count > 0){ button.interactable = true; text.text = "Play"; }
            else { button.interactable = false; text.text = "Play - \"Create a Fleet to Play!\""; }
        }
    }
}