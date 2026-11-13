using System.Linq;
using TMPro;
using UnityEngine;

namespace Cascade {
    public class SHIP_UI : MonoBehaviour
    {
        [SerializeField] private bool debug;
            private void Log(string contents){ if (debug){ Debug.Log(contents + " at: " + name); }}

        [Space(10)]

        [SerializeField] private GameObject parent;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Space(10)]

        [SerializeField] private Transform hpCounter;
        [SerializeField] private GameObject hpIcon;
        [SerializeField] private Transform apCounter;
        [SerializeField] private GameObject apIcon;
        [SerializeField] private Transform spCounter;
        [SerializeField] private GameObject spIcon;
        [SerializeField] private Transform movesCounter;
        [SerializeField] private GameObject moveIcon;

        [Space(10)]

        [SerializeField] private TextMeshProUGUI nameTMP;
        [SerializeField] private TextMeshProUGUI hpTMP;
        [SerializeField] private TextMeshProUGUI apTMP;
        [SerializeField] private TextMeshProUGUI spTMP;

        [Space(10)]

        [SerializeField] private GameObject selectedShipIndicator;

        [Space(10)]

        [SerializeField] private Transform tokenCounter;
        [SerializeField] private GenericDictionary<ENUM_Tokens,GameObject> tokens = new();

        private UI_SHIP_Controls selectedShipControls => FindAnyObjectByType<UI_SHIP_Controls>().GetComponent<UI_SHIP_Controls>();

        public Ship ship => GetComponent<Ship>();

        public void Setup(){ spriteRenderer.material = new Material(spriteRenderer.material); spriteRenderer.material.SetColor("_Color", new Color(0,0,0,0)); UpdateUI(); }

        public void Show(){ UpdateUI(); parent.SetActive(true); }
        public void Hide(){ parent.SetActive(false); UpdateUI(); }

        public void UpdateUI()
        {
                Log("Updating UI!");
            Cleanup();

            for (int i = 0; i < ship.STAT_hp; i++){ Instantiate(hpIcon,hpCounter); }
            for (int i = 0; i < ship.STAT_ap; i++){ Instantiate(apIcon,apCounter); }
            for (int i = 0; i < ship.STAT_sp; i++){ Instantiate(spIcon,spCounter); }
            for (int i = 0; i < ship.STAT_moves; i++){ Instantiate(moveIcon,movesCounter); }

            nameTMP.text = ship.name;
                if (ship.localPlayer != ship.owner)
                {
                    nameTMP.color = Color.red;
                    spriteRenderer.material.SetColor("_Color", Color.red);
                }
            hpTMP.text = ship.STAT_hp.ToString();
            apTMP.text = ship.STAT_ap.ToString();
            spTMP.text = ship.STAT_sp.ToString();

            if (ship.STAT_squad > 1) for (int i = 0; i < ship.STAT_squad; i++){ Instantiate(tokens[ENUM_Tokens.Squadron],tokenCounter); }

            //Show Occupying Tile Type for tooltips etc
            switch (ship.STAT_tileType)
            {
                case ENUM_TileType.None:
                    break;
                case ENUM_TileType.Asteroid:
                    Instantiate(tokens[ENUM_Tokens.Asteroids],tokenCounter);
                    break;
                case ENUM_TileType.Debris:
                    Instantiate(tokens[ENUM_Tokens.Debris],tokenCounter);
                    break;
                case ENUM_TileType.Gas:
                    Instantiate(tokens[ENUM_Tokens.Gas],tokenCounter);
                    break;
                case ENUM_TileType.Nebulae:
                    Instantiate(tokens[ENUM_Tokens.Nebulae],tokenCounter);
                    break;
            }

            if (Player.local.selected != ship) selectedShipIndicator.SetActive(false);
            else selectedShipIndicator.SetActive(true);

            selectedShipControls.UpdateUI();
        }

        private void Cleanup()
        {
                Log("Cleaning Up Ship UI");
            TransformOperations.DestroyChildren(hpCounter);
            TransformOperations.DestroyChildren(apCounter);
            TransformOperations.DestroyChildren(spCounter);
            TransformOperations.DestroyChildren(movesCounter);
            TransformOperations.DestroyChildren(tokenCounter);
        }

        public void ShowWeaponRange(){}
    }
}