using System;
using System.Collections.Generic;
using Ash;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Cascade {
    public class FleetEditor : MonoBehaviour
    {
        [SerializeField] private bool debug;
            private void Log(string contents){ if (debug){ Debug.Log(contents + " at: " + name); }}

        #region Vars

            [Space(10)]

            public List<ShipData> ships = new(){};
            public List<soDATA_Weapon> availableWeapons = new(){};
            public List<soDATA_Hull> availableHulls = new(){};
            public List<soDATA_Ability> availableAbilities = new(){};

            [Space(10)]

            [SerializeField] private soDATA_Weapon defaultWeapon;
            [SerializeField] private soDATA_HULL_Broadside defaultHullBroadside;
            [SerializeField] private soDATA_HULL_Lance defaultHullLance;
            [SerializeField] private soDATA_HULL_Flank defaultHullFlank;

            [Space(10)]

            [field:SerializeField, ReadOnly] int selectedShipIndex = -1;
            private ShipData selectedShip => ships[selectedShipIndex];
            private int spentPoints = 0;
                public int STAT_spentPoints => spentPoints;

            private int selectedWeaponSlot = -1;
            bool selectedShipIsValid => selectedShipIndex != -1;

            private const float selectShipButtonHeight = 200;

            private static Color emptyHullSectionColor = new Color(1,0,0,0.25f);
            private static Color filledHullSectionColor = new Color(0,1,0,0.25f);

            [Space(10)]

            //Components
            [SerializeField] private SpriteRenderer selectedShipSkinSpriteRenderer;

            [Space(10)]

            [SerializeField] private TMP_InputField selectedShipNameInputField;

            [Space(10)]

            [SerializeField] private TextMeshProUGUI shipInfoTMP;
            [SerializeField] private TextMeshProUGUI pointsCounter;

            [Space(10)]

            [SerializeField] private GameObject selectShipButtonPrefab;
            [SerializeField] private RectTransform fleetSelectionContent;

            [Space(10)]

            [SerializeField] private GameObject weaponSelector;
            [SerializeField] private TextMeshProUGUI weaponInformation;
            [SerializeField] private GenericDictionary<int,Image> selectedWeaponIconSlots;

            [Space(10)]

            [SerializeField] private GameObject selectWeaponButtonPrefab;
            [SerializeField] private RectTransform weaponSelectionContent;

            [Space(10)]

            [SerializeField] private GameObject hullSelector;
            [SerializeField] private TextMeshProUGUI hullInformation;
            [SerializeField] private UI_Image selectedLanceHullIcon;
            [SerializeField] private UI_Image selectedLarboardHullIcon;
            [SerializeField] private UI_Image selectedStarboardHullIcon;
            [SerializeField] private UI_Image selectedFlankHullIcon;
            [SerializeField] private Button lanceHullButton;
            [SerializeField] private Button larboardHullButton;
            [SerializeField] private Button starboardHullButton;
            [SerializeField] private Button flankHullButton;

            [Space(10)]

            [SerializeField] private GameObject selectHullButtonPrefab;
            [SerializeField] private RectTransform hullSelectionContent;
            
            [Space(10)]

            [SerializeField] private GameObject abilitySelector;
            [SerializeField] private TextMeshProUGUI abilityInformation;
            [SerializeField] private UI_Image selectedAbilityImage;

            [Space(10)]

            [SerializeField] private GameObject selectAbilityButtonPrefab;
            [SerializeField] private RectTransform abilitySelectionContent;

            [Space(10)]

            [SerializeField] private TMP_Dropdown skinSelectionDropdown;

            [Space(10)]

            [SerializeField] private Selectable[] shipEditorUI;

        #endregion

        void Awake(){ Setup(); }
        void Setup(){ Load(); }

        #region Explicit Function

            public void Save(){ ACCOUNT_Ships.SetShips(ships); ACCOUNT_Ships.fleetValue = GetFleetValue(); if (debug) ACCOUNT_Ships.PrintShips(); }

            public void Load(){ ships = ACCOUNT_Ships.ships; UpdateUI(); }

            public void Select(int index)
            {
                selectedShipIndex = index;
                UpdateUI();
            }

            public void RemoveShipAt(int index){ ships.RemoveAt(index); if (selectedShipIndex == index){ selectedShipIndex = -1; } UpdateUI(); }

            public void AddNewShip()
            {
                    if (ships.Count > 9){ return; }
                SFX.PlaySFX(ENUM_SFXTag.ShipSpawned);

                ships.Add(new ShipData());

                Select(ships.Count-1);

                selectedWeaponSlot = 1;
                SetWeaponInSelectedSlot(defaultWeapon);

                //Set Default Hull Segments
                if (false){
                    //SetHullFlankInSelectedSlot(defaultHullFlank);
                    //SetHullStarboardInSelectedSlot(defaultHullBroadside);
                    //SetHullLarboardInSelectedSlot(defaultHullBroadside);
                    //SetHullLanceInSelectedSlot(defaultHullLance);
                }

                UpdateUI();
            }

        #endregion

        #region Getters and Setters
            public int GetFleetValue()
            {
                int value = 0;

                foreach (ShipData ship in ships) value += ship.GetValue();

                return value;
            }

            public void SetSelectedShipName(string newName){ selectedShip.Name = newName; UpdateUI(); }

            public void SetSkinCode(int dropdownOptionsIndex)
            {
                    if (!selectedShipIsValid){ return; }
                string selectedSkin = skinSelectionDropdown.options[dropdownOptionsIndex].text;
                    if (selectedSkin == "None"){ selectedShip.SkinCode = ""; return; }
                selectedShip.SkinCode = skinSelectionDropdown.options[dropdownOptionsIndex].text;
                
                UpdateUI();
            }

            public void SetWeaponInSelectedSlot(soDATA_Weapon choice){ selectedShip.Weapons[selectedWeaponSlot-1] = choice; UpdateAllWeaponSlotIcons(); UpdateWeaponInformationText(); UpdateSpentPointsCounter(); }

            public void SetHullLanceInSelectedSlot(soDATA_HULL_Lance choice){ selectedShip.LanceHull = choice; selectedLanceHullIcon.SetSprite(choice.icon); UpdateHullInformationText("Lance"); }
            public void SetHullLarboardInSelectedSlot(soDATA_HULL_Broadside choice){ selectedShip.LarboardHull = choice; selectedLarboardHullIcon.SetSprite(choice.icon); UpdateHullInformationText("Larboard"); }
            public void SetHullStarboardInSelectedSlot(soDATA_HULL_Broadside choice){ selectedShip.StarboardHull = choice; selectedStarboardHullIcon.SetSprite(choice.icon); UpdateHullInformationText("Starboard"); }
            public void SetHullFlankInSelectedSlot(soDATA_HULL_Flank choice){ selectedShip.FlankHull = choice; selectedFlankHullIcon.SetSprite(choice.icon); UpdateHullInformationText("Flank"); }

            public void SetAbilityInSelectedSlot(soDATA_Ability choice){ selectedShip.Ability = choice; UpdateAbilitySlotIcon(); UpdateAbilityInformationText(); }
        #endregion

        #region UI
            public void UpdateUI()
            {
                    CloseWeaponSelector();

                UpdateFleetSelector();

                UpdateUIInteractableStates();

                UpdateSelectedShipSprite();
                UpdateSelectedShipNameInputField();
                UpdateAllWeaponSlotIcons();
                UpdateAllHullSlotIcons();
                UpdateAllHullButtons();
                UpdateAbilitySlotIcon();
                UpdateSkinSelector();
                UpdateSpentPointsCounter();

                if (selectedShipIsValid) shipInfoTMP.text = selectedShip?.GetDataParagraph();
            }

            void UpdateUIInteractableStates(){ foreach (Selectable uiItem in shipEditorUI) uiItem.interactable = selectedShipIsValid; }

            void UpdateSkinSelector()
            {
                    if (!selectedShipIsValid) return;
                int index = skinSelectionDropdown.options.FindIndex(option => option.text == selectedShip.SkinCode);

                if (index >= 0) skinSelectionDropdown.SetValueWithoutNotify(index);
            }

            void UpdateFleetSelector()
            {
                //Clear Current Fleet
                TransformOperations.DestroyChildren(fleetSelectionContent);

                //Set Content Height
                fleetSelectionContent.sizeDelta = new Vector2(fleetSelectionContent.sizeDelta.x, selectShipButtonHeight*ships.Count);

                //Regenerate Selection Buttons
                int counter = 0;
                foreach (var ship in ships)
                {
                    GameObject newShipSelectButton = Instantiate(selectShipButtonPrefab,fleetSelectionContent);
                        newShipSelectButton.GetComponent<Image>().sprite = Ship.LoadSkinFromResources(Ship.DecryptSkinCode(ship.SkinCode));
                        newShipSelectButton.GetComponent<UI_Fleet_ShipSelectorButton>().Setup(counter);
                    counter++;
                }
            }

            void UpdateWeaponSelector()
            {
                //Clear Current Buttons
                TransformOperations.DestroyChildren(weaponSelectionContent);

                //Regenerate Selection Buttons
                foreach (var weapon in availableWeapons)
                {
                    GameObject newWeaponSelectButton = Instantiate(selectWeaponButtonPrefab,weaponSelectionContent);
                        newWeaponSelectButton.GetComponent<Image>().sprite = weapon.icon;
                        newWeaponSelectButton.GetComponent<UI_Fleet_WeaponSelectorButton>().Setup(weapon);
                }

                //Set Content Height
                float newContentHeight = selectWeaponButtonPrefab.GetComponent<RectTransform>().rect.height*availableWeapons.Count;
                TransformOperations.SetRectHeight(weaponSelectionContent,newContentHeight);
            }

            void UpdateHullSelector(string sectionType)
            {
                    if (sectionType == "null"){}
                //Clear Current Buttons
                TransformOperations.DestroyChildren(hullSelectionContent);

                //Regenerate Selection Buttons
                foreach (var hull in availableHulls) if ((hull.type == sectionType) || ((hull.type == "Broadside") && ((sectionType == "Starboard") || (sectionType == "Larboard")))) 
                    Instantiate(selectHullButtonPrefab,hullSelectionContent).GetComponent<UI_Fleet_HullSelectorButton>().Setup(hull,sectionType);

                //Set Content Height
                float newContentHeight = selectHullButtonPrefab.GetComponent<RectTransform>().rect.height*availableHulls.Count;
                TransformOperations.SetRectHeight(hullSelectionContent,newContentHeight);
            }

            void UpdateAbilitySelector()
            {
                //Clear Current Buttons
                TransformOperations.DestroyChildren(abilitySelectionContent);

                //Regenerate Selection Buttons
                foreach (var ability in availableAbilities) 
                    Instantiate(selectAbilityButtonPrefab,abilitySelectionContent).GetComponent<UI_Fleet_AbilitySelectorButton>().Setup(ability);

                //Set Content Height
                float newContentHeight = selectAbilityButtonPrefab.GetComponent<RectTransform>().rect.height*availableAbilities.Count;
                TransformOperations.SetRectHeight(abilitySelectionContent,newContentHeight);
            }

            void UpdateAllHullButtons()
            {
                    if (!selectedShipIsValid) return;
                var lanceButtonCB = lanceHullButton.colors;
                if (selectedShip.LanceHull == null) lanceButtonCB.normalColor = emptyHullSectionColor;
                else lanceButtonCB.normalColor = filledHullSectionColor;
                lanceHullButton.colors = lanceButtonCB;

                var larboardButtonCB = larboardHullButton.colors;
                if (selectedShip.LarboardHull == null) larboardButtonCB.normalColor = emptyHullSectionColor;
                else larboardButtonCB.normalColor = filledHullSectionColor;
                larboardHullButton.colors = larboardButtonCB;

                var starboardButtonCB = starboardHullButton.colors;
                if (selectedShip.StarboardHull == null) starboardButtonCB.normalColor = emptyHullSectionColor;
                else starboardButtonCB.normalColor = filledHullSectionColor;
                starboardHullButton.colors = starboardButtonCB;

                var flankButtonCB = flankHullButton.colors;
                if (selectedShip.FlankHull == null) flankButtonCB.normalColor = emptyHullSectionColor;
                else flankButtonCB.normalColor = filledHullSectionColor;
                flankHullButton.colors = flankButtonCB;
            }

            void UpdateSelectedShipNameInputField()
            {
                    if (!selectedShipIsValid){ selectedShipNameInputField.gameObject.SetActive(false); return; }
                selectedShipNameInputField.gameObject.SetActive(true);
                selectedShipNameInputField.text = selectedShip.Name;
            }

            public void UpdateSelectedShipSprite()
            {
                    if (!selectedShipIsValid){ selectedShipSkinSpriteRenderer.sprite = null; return; }
                string skinCode = Ship.DecryptSkinCode(ships[selectedShipIndex].SkinCode);
                    if (skinCode == ""){ selectedShipSkinSpriteRenderer.sprite = null; return; }
                selectedShipSkinSpriteRenderer.sprite = Ship.LoadSkinFromResources(skinCode);
            }

            public void UpdateSpentPointsCounter(){ pointsCounter.text = "Fleet Value: " + GetFleetValue(); }

            void UpdateAllWeaponSlotIcons()
            {
                    if (!selectedShipIsValid) return;
                for (int i = 0; i < 3; i++)
                {
                        if (selectedShip.Weapons[i] == null){ selectedWeaponIconSlots[i].gameObject.SetActive(false); continue; }
                    selectedWeaponIconSlots[i].gameObject.SetActive(true);
                    selectedWeaponIconSlots[i].sprite = selectedShip.Weapons[i].icon;
                }
            }
            void UpdateWeaponInformationText()
            {
                    if (!selectedShipIsValid) return;
                    if (selectedShip.Weapons[selectedWeaponSlot-1] == null){ weaponInformation.text = ""; return; }
                weaponInformation.text = selectedShip.Weapons[selectedWeaponSlot-1].description
                + selectedShip.Weapons[selectedWeaponSlot-1].GetDataParagraph();
            }

            void UpdateAllHullSlotIcons()
            {
                    if (!selectedShipIsValid) return;
                selectedLanceHullIcon.SetSprite(selectedShip.LanceHull?.icon);
                selectedLarboardHullIcon.SetSprite(selectedShip.LarboardHull?.icon);
                selectedStarboardHullIcon.SetSprite(selectedShip.StarboardHull?.icon);
                selectedFlankHullIcon.SetSprite(selectedShip.FlankHull?.icon);
            }

            void UpdateHullInformationText(string arg, string info = "Error: not set")
            {
                if (info == "Error: not set"){
                    if (arg == "Lance"){ info = selectedShip.LanceHull?.info + selectedShip.LanceHull?.GetDataParagraph(); }
                    if (arg == "Starboard"){ info = selectedShip.StarboardHull?.info + selectedShip.StarboardHull?.GetDataParagraph(); }
                    if (arg == "Larboard"){ info = selectedShip.LarboardHull?.info + selectedShip.LarboardHull?.GetDataParagraph(); }
                    if (arg == "Flank"){ info = selectedShip.FlankHull?.info + selectedShip.FlankHull?.GetDataParagraph(); }
                }
                hullInformation.text = info;

                UpdateUI();
            }

            void UpdateAbilitySlotIcon(){ if (!selectedShipIsValid){ return; } selectedAbilityImage.SetSprite(selectedShip.Ability?.icon); }

            void UpdateAbilityInformationText()
            {
                    if (!selectedShipIsValid) return;
                    if (selectedShip.Ability == null){ abilityInformation.text = ""; return; }
                abilityInformation.text = selectedShip.Ability.description;
            }

            public void OpenWeaponSelector(int weaponSlot = 1)
            {
                    if (!selectedShipIsValid){ return; }
                    if (selectedWeaponSlot == weaponSlot && (weaponSelector.activeInHierarchy == true)){ CloseWeaponSelector(); return; }
                UpdateWeaponSelector();
                selectedWeaponSlot = weaponSlot;
                weaponSelector.SetActive(true);

                UpdateAllWeaponSlotIcons();
                UpdateWeaponInformationText();

                CloseHullSelector();
                CloseAbilitySelector();
            }
            public void CloseWeaponSelector(){ weaponSelector.SetActive(false); }

            public void OpenHullSelector(string arg = "null")
            {
                    if (!selectedShipIsValid){ return; }
                    if (arg == "null"){}
                UpdateHullSelector(arg);
                UpdateHullInformationText(arg);

                hullSelector.SetActive(true);

                CloseWeaponSelector();
                CloseAbilitySelector();
            }
            public void CloseHullSelector(){ hullSelector.SetActive(false); }

            public void OpenAbilitySelector()
            {
                    if (!selectedShipIsValid){ return; }
                UpdateAbilitySelector();
                UpdateAbilityInformationText();

                abilitySelector.SetActive(true);

                CloseWeaponSelector();
                CloseHullSelector();
            }
            public void CloseAbilitySelector(){ abilitySelector.SetActive(false); }

        #endregion
    }
}