using TMPro;
using UnityEngine;

public class UI_Fleet_HullSelectorButton : MonoBehaviour
{
    public soDATA_Hull data;
    public string arg;

    [SerializeField] private TextMeshProUGUI nameTMP;
    [SerializeField] private TextMeshProUGUI costTMP;

    public void Setup(soDATA_Hull newData)
    {
        data = newData;

        nameTMP.text = data.name;
        costTMP.text = data.cost.ToString();
    }

    public void Activate()
    {
        switch (arg)
        {
            case "Null":
                break;
            case "Lance":
                FindAnyObjectByType<FleetEditor>().SetHullLanceInSelectedSlot(data as soDATA_HULL_Lance);
                break;
            case "Larboard":
                FindAnyObjectByType<FleetEditor>().SetHullLarboardInSelectedSlot(data as soDATA_HULL_Broadside);
                break;
            case "Starboard":
                FindAnyObjectByType<FleetEditor>().SetHullStarboardInSelectedSlot(data as soDATA_HULL_Broadside);
                break;
            case "Flank":
                FindAnyObjectByType<FleetEditor>().SetHullFlankInSelectedSlot(data as soDATA_HULL_Flank);
                break;
        }
    }
}