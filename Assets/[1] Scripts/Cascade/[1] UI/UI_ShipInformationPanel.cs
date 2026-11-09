using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 4014
public class UI_ShipInformationPanel : UI
{
    [SerializeField] private bool debug;
        private void Log(string contents){ if (debug){ Debug.Log(contents + " at: " + name); }}

    [Space(10)]

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image skinImage;

    [SerializeField] private TextMeshProUGUI hpCounter;

    //[SerializeField] private TextMeshProUGUI weaponSlot1NameText;
    //[SerializeField] private Image weaponSlot1IconImage;
    [SerializeField] private TextMeshProUGUI weaponSlot1StatsText;
    [SerializeField] private TextMeshProUGUI weaponSlot2StatsText;
    [SerializeField] private TextMeshProUGUI weaponSlot3StatsText;

    void Awake(){ SubscribeToEvent(); }
    public void Setup(){ Player.local.EVENT_ShipSelectionChanged += UpdateUI; }

    async Task SubscribeToEvent()
    {
        while (Player.local == null)
        {
            await Task.Yield();
        }

        Setup();
    }

    void UpdateUI()
    {
            if (Player.local.selected == null){ Hide(); return; }
        Show();

        Ship ship = Player.local.selected;

        //Base
        nameText.text = ship.name;
        skinImage.sprite = ship.GetSkin();

        //Counters
        // HP
        hpCounter.text = ship.GetHP() + "*";
        // AP

        // SP


        //Weapons
        // 1
        //weaponSlot1NameText.text = ship.GetWeapons()[0].
        weaponSlot1StatsText.text =
        "Damage: " + ship.GetWeapons()[0].damage + "\n" +
        "Type: " + ship.GetWeapons()[0].type + "\n" +
        "Salvo: " + ship.GetWeapons()[0].salvo + "\n" +
        "Range: " + ship.GetWeapons()[0].range + "\n" +
        "Area: " + ship.GetWeapons()[0].area + "\n"
        ;
        // 2
        //weaponSlot1NameText.text = ship.GetWeapons()[0].
        weaponSlot2StatsText.text =
        "Damage: " + ship.GetWeapons()[1].damage + "\n" +
        "Type: " + ship.GetWeapons()[1].type + "\n" +
        "Salvo: " + ship.GetWeapons()[1].salvo + "\n" +
        "Range: " + ship.GetWeapons()[1].range + "\n" +
        "Area: " + ship.GetWeapons()[1].area + "\n"
        ;
        // 3
        //weaponSlot1NameText.text = ship.GetWeapons()[0].
        weaponSlot3StatsText.text =
        "Damage: " + ship.GetWeapons()[2].damage + "\n" +
        "Type: " + ship.GetWeapons()[2].type + "\n" +
        "Salvo: " + ship.GetWeapons()[2].salvo + "\n" +
        "Range: " + ship.GetWeapons()[2].range + "\n" +
        "Area: " + ship.GetWeapons()[2].area + "\n"
        ;
    }
}