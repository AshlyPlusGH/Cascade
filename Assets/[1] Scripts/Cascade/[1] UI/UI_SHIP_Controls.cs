using System.Threading.Tasks;
using Ash;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 4014
namespace Cascade {
    public class UI_SHIP_Controls : MonoBehaviour
    {
        [SerializeField] private bool debug;
            private void Log(string contents){ if (debug){ Debug.Log(contents + " at: " + name); }}

        Ship ship => Player.local.selected;
        int hp => ship.STAT_hp;
        int ap => ship.STAT_ap;
        int sp => ship.STAT_sp;

        private Ship lastUpdateSource;

        //Components
        [SerializeField] private UI_TileCounter hpCounter;
        [SerializeField] private UI_TileCounter apCounter;
        [SerializeField] private UI_TileCounter spCounter;
        [SerializeField] private UI_PrefabImageCounter movesCounter;

        [Space(10)]

        [SerializeField] private TextMeshProUGUI shipNameTMP;

        [Space(10)]

        [SerializeField] private Button fireSelectedWeapon;

        [Space(10)]

        [SerializeField] private UI_Image selectedWeaponImage;
        [SerializeField] private UI_Image nextWeaponImage;
        [SerializeField] private UI_Image previousWeaponImage;
        [SerializeField] private UI_Image abilityImage;
        [SerializeField] private TextMeshProUGUI selectedWeaponNumberTMP;
        [SerializeField] private TextMeshProUGUI nextWeaponNumberTMP;
        [SerializeField] private TextMeshProUGUI previousWeaponNumberTMP;

        [Space(10)]

        [SerializeField] private GameObject abilityPanelParent;

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

        public void UpdateUI()
        {
                if (lastUpdateSource != ship) abilityPanelParent.SetActive(false);
                if (ship == null) return;
            lastUpdateSource = ship;
            shipNameTMP.text = ship.name;

            hpCounter.SetValueDirty(hp);
            apCounter.SetValueDirty(ap);
            spCounter.SetValueDirty(sp);
            movesCounter.SetValueDirty(ship.STAT_moves);

                bool isLoaded = ship.QueryWeaponHasAmmo(ship.selectedWeaponIndex);
                bool isMyTurn = GameManager.instance.GetActivePlayer() == ship.owner;
            fireSelectedWeapon.interactable = isLoaded && isMyTurn;

            if (ship.QueryWeaponHasAmmo(ship.selectedWeaponIndex)){ selectedWeaponImage.SetColor(Color.white); }
                else { selectedWeaponImage.SetColor(Color.red); }
            if (ship.QueryWeaponHasAmmo(ship.GetNextWeaponIndex())){ nextWeaponImage.SetColor(Color.white); }
                else { nextWeaponImage.SetColor(Color.red); }
            if (ship.QueryWeaponHasAmmo(ship.GetPreviousWeaponIndex())){ previousWeaponImage.SetColor(Color.white); }
                else { previousWeaponImage.SetColor(Color.red); }

            selectedWeaponImage.SetSprite(Ship.LoadWeaponIconFromResources(ship.STAT_selectedWeapon.weaponName));
            nextWeaponImage.SetSprite(Ship.LoadWeaponIconFromResources(ship.STAT_nextWeapon.weaponName));
            previousWeaponImage.SetSprite(Ship.LoadWeaponIconFromResources(ship.STAT_previousWeapon.weaponName));
            abilityImage.SetSprite(Ship.LoadAbilityIconFromResources(ship.STAT_ability.name));

            selectedWeaponNumberTMP.text = ship.selectedWeaponIndex.ToString();
            nextWeaponNumberTMP.text = ship.GetNextWeaponIndex().ToString();
            previousWeaponNumberTMP.text = ship.GetPreviousWeaponIndex().ToString();
        }
    }
}