using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cascade {
    public class UI_Fleet_AbilitySelectorButton : MonoBehaviour
    {
        public soDATA_Ability data;

        [SerializeField] private TextMeshProUGUI nameTMP;
        [SerializeField] private TextMeshProUGUI costTMP;
        [SerializeField] private Image iconImage;

        public void Setup(soDATA_Ability newData)
        {
            data = newData;

            nameTMP.text = data.name;
            costTMP.text = data.cost.ToString();
            iconImage.sprite = data.icon;
        }

        public void Activate(){ FindAnyObjectByType<FleetEditor>().SetAbilityInSelectedSlot(data); }
    }
}