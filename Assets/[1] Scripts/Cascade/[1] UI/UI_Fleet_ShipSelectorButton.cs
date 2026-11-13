using TMPro;
using UnityEngine;

namespace Cascade {
    public class UI_Fleet_ShipSelectorButton : MonoBehaviour
    {
        public int index = -1;

        [SerializeField] private TextMeshProUGUI indexCounter;

        public void Setup(int newIndex)
        {
            index = newIndex;

            indexCounter.text = "." + index;
        }

        public void Activate(){ FindAnyObjectByType<FleetEditor>().Select(index); }

        public void RemoveFromFleet(){ FindAnyObjectByType<FleetEditor>().RemoveShipAt(index); }
    }
}