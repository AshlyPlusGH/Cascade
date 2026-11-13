using TMPro;
using UnityEngine;

namespace Cascade {
    public class UI_Fleet_WeaponSelectorButton : MonoBehaviour
    {
        [SerializeField] private bool debug;
            private void Log(string contents){ if (debug){ Debug.Log(contents + " at: " + name); }}

        [Space(10)]

        public soDATA_Weapon data;

        [SerializeField] private TextMeshProUGUI nameTMP;
        [SerializeField] private TextMeshProUGUI costTMP;

        public void Setup(soDATA_Weapon newData)
        {
            data = newData;

            nameTMP.text = data.name;
            costTMP.text = data.cost.ToString();
        }

        public void Activate(){ FindAnyObjectByType<FleetEditor>().SetWeaponInSelectedSlot(data); }
    }
}