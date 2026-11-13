using System.Collections.Generic;
using NaughtyAttributes;
using PurrNet;
using UnityEngine;
using System.Linq;
using System;
using Ash;

namespace Cascade{
    public class Ship : NetworkBehaviour
    {
        [SerializeField] private bool debug;
            private void Log(string contents){ if (debug){ Debug.Log(contents + " at: " + name); }}

        [SerializeField] private Vector3Int DEBUG_givenCoordinates;
        [ReadOnly] public int DEBUG_sideFacing;

        #region Statics
            public static Ship GetShipAtCoordinates(Vector3Int coordinates)
            {
                foreach (var ship in FindObjectsByType<Ship>())
                {
                    if (ship.GetCoordinates() == coordinates){ return ship; }
                }

                return null;
            }

            public static string DecryptSkinCode(string shipCode)
            {
                    if ((shipCode == "") || (shipCode == null)) return "DEFAULT";
                string[] codes = shipCode.Split('_', StringSplitOptions.None); 
                string skinCode = codes[codes.Length-1];
                    if ((skinCode == null)){ skinCode = "DEFAULT"; } //(skinCode == "") || 
                return skinCode;
            }

            public static Weapon asteroidWeapon = new Weapon()
            {
                type = ENUM_WeaponType.None,
                damage = 1,
                salvo = 1
            };
            public static Weapon debrisWeapon = new Weapon()
            {
                type = ENUM_WeaponType.None,
                damage = 1,
                salvo = 2
            };

            public static Sprite LoadSkinFromResources(string skinCode){ return Resources.Load<Sprite>("Skins/" + skinCode); }
            public static Sprite LoadWeaponIconFromResources(string weaponName){ if (weaponName == ""){ weaponName = "DEFAULT"; } return Resources.Load<Sprite>("Weapons/" + weaponName); }
            public static Sprite LoadAbilityIconFromResources(string ability){ return Resources.Load<Sprite>("Abilities/" + ability); }

            public static Vector3Int[] CoordinateCodesToCoordinates(string[] raw)
            {
                return raw
                    .Select(s =>
                    {
                        var parts = s.Split('#');
                        //Debug.Log(parts[0]);
                        //Debug.Log(parts[1]);
                        //Debug.Log(parts[2]);
                        return new Vector3Int(
                            int.Parse(parts[0]),
                            int.Parse(parts[1]),
                            int.Parse(parts[2])
                        );
                    })
                    .ToArray();
            }
        
            public static void ResetStatics()
            {
                asteroidWeapon = new Weapon()
                {
                    type = ENUM_WeaponType.None,
                    damage = 1,
                    salvo = 1
                };
            }
        #endregion

        [Space(10)]

        #region Vars
            [SerializeField] private string shipCode = null;

            [field:SerializeField] public int selectedWeaponIndex {get; private set;} = 0;

            //Stats
            [SerializeField] private int moves = 1;
                public int STAT_moves => GetMoves();
            [SerializeField] private int maxMoves = 1;
                public int STAT_maxMoves => maxMoves;

            [SerializeField] private int hp = 1;
                public int STAT_hp => GetHP();
            [SerializeField] private int maxHP = 0;
                public int STAT_maxHP => GetMaxHP();
            [SerializeField] private int ap = 0;
                public int STAT_ap => GetAP();
            [SerializeField] private int maxAP = 0;
                public int STAT_maxAP => GetMaxAP();
            [SerializeField] private int sp = 0;
                public int STAT_sp => GetSP();
            [SerializeField] private int maxSP = 0;
                public int STAT_maxSP => GetMaxSP();

            [SerializeField] private int squad = 1;
                public int STAT_squad => GetSquad();

            [SerializeField] private Weapon[] weapons = new Weapon[3];
                public Weapon[] STAT_weapons => GetWeapons();
                public Weapon STAT_selectedWeapon => GetSelectedWeapon();
                public Weapon STAT_nextWeapon => GetNextWeapon();
                public Weapon STAT_previousWeapon => GetPreviousWeapon();
            [SerializeField] private bool[] magazine = new bool[3];
            public bool[] STAT_magazine => GetMagazine();

            [SerializeField] private Ability ability;
                public Ability STAT_ability => GetAbility();

            //Pointers
            public Vector3Int STAT_coordinates => GetCoordinates();
            public ENUM_TileType STAT_tileType => Map.QueryTileTypeAtCoordinates(STAT_coordinates);
            public bool isLocal => owner == localPlayer;

            //Events

            //Components
            [SerializeField] private SHIP_UI ui;
            [SerializeField] private SpriteRenderer spriteRenderer;
        #endregion

        void Awake(){ ACTION_Setup(shipCode); }
        [ObserversRpc] public void Setup(string code = null)
        {
            ACTION_Setup(code);
        }
            void ACTION_Setup(string code = null)
            {
                    magazine = new bool[3]; //Fix correction by Serialization
                    weapons = new Weapon[3];
                if ((code != null) && (code != "")){ ApplyShipCode(code); }
                
                DEBUG_sideFacing = GetSideFacing();

                    if (STAT_selectedWeapon.weaponName == "NULL") CycleSelectedWeapon(1);
                    if (STAT_selectedWeapon.weaponName == "NULL") CycleSelectedWeapon(1);

                ui.Setup();
                ui.Hide(); //Ensure UI is Hidden on Spawn
            }

        public void OnMouseDown()
        {
                if (GameManager.gameInProgress == false) return;
                if (Player.local.isBusy){ return; }
            Player.local.TrySelectShip(this);
        }

        void Update()
        {
                //if (debug) Debug.Log("Distance from Mouse to Ship: " + Map.Distance(GetCoordinates(),Map.GetCoordinatesAtMouse()));
            if (QueryIsMouseOver()) ui.Show();
            else if (Player.local.selected != this) ui.Hide();
        }

        #region Game
            [ServerRpc(requireOwnership:false)]
            public void Move(Vector3Int coordinates){ UPDATECLIENTS_Move(coordinates); }
            [ObserversRpc] public void UPDATECLIENTS_Move(Vector3Int coordinates)
            {
                int distance = Map.Distance(GetCoordinates(), coordinates);
                moves-=distance;

                transform.position = Map.GetPositionAtCoordinates(coordinates);

                ui.UpdateUI();
            }

            [ServerRpc(requireOwnership:false)]
            public void Rotate(ENUM_RotationDirection dir){ UPDATECLIENTS_Rotate(dir); }
            [ObserversRpc] public void UPDATECLIENTS_Rotate(ENUM_RotationDirection dir)
            {
                if (dir == ENUM_RotationDirection.Left){ transform.Rotate(0f, 0f, 60f); }
                if (dir == ENUM_RotationDirection.Right){ transform.Rotate(0f, 0f, -60f); }
                
                    DEBUG_sideFacing = GetSideFacing();

                moves-=1;

                ui.UpdateUI();
            }

            public void Selected(){ ui.Show(); }
            public void Deselected(){ if (!QueryIsMouseOver()) ui.Hide(); }

            public void RecoverMoves(){ Log("Recharging Engines!"); moves = maxMoves; ui.UpdateUI(); }
            public void Reload(){ Log("Reloading!"); Array.Fill(magazine,true); ui.UpdateUI(); }

            public void TryFire(Vector3Int coordinates)
            {
                    Log("Trying to Fire!");

                Weapon weapon = STAT_selectedWeapon;
                    
                    //Check if Weapon can Fire
                        if (weapon.weaponName == "NULL"){ Log("Unset Weapon no stats!"); return; }
                    if (!QueryWeaponHasAmmo(selectedWeaponIndex)){ Log("No Ammo for Weapon Found!"); return; } //No Ammo
                    if (GetShipAtCoordinates(coordinates) == null){ Log("No Target Found!"); return; } //No target found!
                    if (GetShipAtCoordinates(coordinates).owner == owner){ Log("Friendly Target at Position!"); return; } //Friendly target
                    if (Map.Distance(GetCoordinates(),coordinates) > weapon.range){ Log("Out of Range!"); return; } // out of range

                //Fire
                UseAmmo(selectedWeaponIndex);
                GetShipAtCoordinates(coordinates).Hit(weapon);
                    if (weapon.area == null){}
                else if (weapon.area.Count() > 0) foreach (var offset in weapon.area)
                {
                    Vector3Int targetPosition = coordinates + offset;

                    //Compensate for even‑Q vertical layout when using Offsets
                    if ((coordinates.y & 1) != 0){ if ((targetPosition.y & 1) == 0) targetPosition.x++; } 

                    GetShipAtCoordinates(targetPosition)?.Hit(weapon, QueryOccupyingTileType());
                }

                SFX.PlaySFX(ENUM_SFXTag.WeaponFireDefault);
            }
            void UseAmmo(int indexOfWeapon){ magazine[indexOfWeapon] = false; }

            //Currently just calculating damage so no client side effects or visuals here unless reworked
            [ServerRpc(requireOwnership:false)] public void Hit(Weapon weapon, ENUM_TileType sourceBiome = ENUM_TileType.None)
            {
                    Log("Ship is Hit!");
                    Log("Terrain is: " + QueryOccupyingTileType());
                int finalDamage = weapon.damage;
                int shieldDamage = 0;
                int armorDamage = 0;
                int hullDamage = 0;

                int plasmaGasBonus = 1; //Default Plasma Damage
                if (sourceBiome == ENUM_TileType.Gas){ plasmaGasBonus = 2; }

                for (int shotNumber = 0; shotNumber < weapon.salvo; shotNumber++)
                {
                    for (int i = 0; i < finalDamage; i++)
                    {
                        if (sp > shieldDamage)
                        {
                            if (weapon.type == ENUM_WeaponType.Coil){ shieldDamage += 2; }
                            else if (weapon.type == ENUM_WeaponType.Scatter){ shieldDamage += squad; }
                            else if (weapon.type == ENUM_WeaponType.Plasma){ shieldDamage += plasmaGasBonus; }
                            else { shieldDamage += 1; }
                        }
                        else if (ap > armorDamage)
                        {
                            if (weapon.type == ENUM_WeaponType.Plasma){ armorDamage += 2; }
                            else if (weapon.type == ENUM_WeaponType.Scatter){ armorDamage += squad; }
                            else { armorDamage += 1; }
                        }
                        else if (hp > hullDamage)
                        {
                            if (weapon.type == ENUM_WeaponType.Cannon){ hullDamage += 2; }
                            else if (weapon.type == ENUM_WeaponType.Scatter){ hullDamage += squad; }
                            else if (weapon.type == ENUM_WeaponType.Plasma){ hullDamage += plasmaGasBonus; }
                            else { hullDamage += 1; }
                        }
                    }
                }

                    Log("Final Damage Dealt: " + "\n" +
                    "Shield: " + shieldDamage + "\n" +
                    "Armor: " + armorDamage + "\n" +
                    "Hull: " + hullDamage + "\n"
                    );
                DamageShield(shieldDamage);
                DamageArmor(armorDamage);
                DamageHull(hullDamage);
            }
                void PlayDamageEffect()
                {
                    SFX.PlaySFX(ENUM_SFXTag.ShipDamaged);
                    FindAnyObjectByType<Camera_Effects>().Shake(1,0.15f); //My Ship taken damage play local effect
                }
                [ObserversRpc] public void DamageShield(int damage)
                {
                    if (QueryOccupyingTileType() == ENUM_TileType.Nebulae){ damage*=2; }
                        Log("Taking " + damage + " Shield Damage!");

                    sp = Mathf.Clamp(sp-damage,0,sp); //Set SP

                    ui.UpdateUI();
                    Particle.SpawnParticle(ENUM_ParticleType.ShieldDamaged,transform.position);
                }
                [ObserversRpc] public void DamageArmor(int damage)
                {
                        Log("Taking " + damage + " Armor Damage!");
                    ap = Mathf.Clamp(ap-damage,0,ap); //Set AP

                    ui.UpdateUI();
                    Particle.SpawnParticle(ENUM_ParticleType.ArmorDamaged,transform.position);
                }
                [ObserversRpc] public void DamageHull(int damage)
                {
                        Log("Taking " + damage + " Hull Damage!");
                    hp = Mathf.Clamp(hp-damage,0,hp); //Set HP

                    if (hp < 1){ if (isServer) Perish(); }

                    ui.UpdateUI();
                    Particle.SpawnParticle(ENUM_ParticleType.HullDamaged,transform.position);

                    if (hp > 0) PlayDamageEffect();
                }

            [ServerRpc] void Perish(){ Destroy(gameObject); }
                protected override void OnDespawned(bool asServer){ if (isServer) PlayDestroyedEffect(); FindAnyObjectByType<GameManager>().ShipDestroyed(); }
                [ObserversRpc] void PlayDestroyedEffect()
                {
                    Particle.SpawnParticle(ENUM_ParticleType.ShipDestroyed,transform.position);
                    SFX.PlaySFX(ENUM_SFXTag.ShipDestroyed);
                    FindAnyObjectByType<Camera_Effects>().Shake(1,0.3f);  //Duration, Magnitude
                }

            [ObserversRpc(requireServer:false)] public void Heal(int healedHP, int healedAP = 0, int healedSP = 0)
            {
                    Log(
                        "Healing: \n" +
                        "HP: " + healedHP +
                        "AP: " + healedAP +
                        "SP: " + healedSP
                    );
                hp = Mathf.Clamp(hp+healedHP,1,maxHP);
                ap = Mathf.Clamp(ap+healedAP,1,maxAP);
                sp = Mathf.Clamp(sp+healedSP,1,maxSP);

                ui.UpdateUI();
            }

            public void CheckForTileDamage()
            {
                if (STAT_tileType == ENUM_TileType.Asteroid) Hit(asteroidWeapon, STAT_tileType);
                if (STAT_tileType == ENUM_TileType.Debris) Hit(debrisWeapon, STAT_tileType);
            }
        #endregion

        #region Getters and Setters
            public int GetMoves(){ return moves; }

            public int GetHP(){ return hp; }
            public int GetMaxHP(){ return maxHP; }
            public int GetAP(){ return ap; }
            public int GetMaxAP(){ return maxAP; }
            public int GetSP(){ return sp; }
            public int GetMaxSP(){ return maxSP; }

            public int GetSquad(){ return squad; }

            public Weapon[] GetWeapons(){ Weapon[] copy = new Weapon[3]; Array.Copy(weapons,copy,3); return copy; }

            public Weapon GetSelectedWeapon(){ return weapons[selectedWeaponIndex]; }
            public int GetNextWeaponIndex()
            {
                int index = selectedWeaponIndex + 1;
                    if (index > 2) index = 0;
                return index;
            }
            public Weapon GetNextWeapon(){ return weapons[GetNextWeaponIndex()]; }
            public int GetPreviousWeaponIndex()
            {
                int index = selectedWeaponIndex - 1;
                    if (index < 0) index = 2;
                return index;
            }
            public Weapon GetPreviousWeapon(){ return weapons[GetPreviousWeaponIndex()]; }

            public bool[] GetMagazine(){ bool[] copy = new bool[3]; Array.Copy(magazine,copy,3); return copy; }

            public Ability GetAbility(){ return ability; }

            public int GetSideFacing()
            {
                return Mathf.RoundToInt(Mathf.Repeat(transform.eulerAngles.z, 360f) / 60f) % 6;
            }

            public Vector3Int GetCoordinates(){ return Map.GetCoordinatesAtPosition(transform.position); }

            public Vector3Int[] GetCoordinatesInRange(){ return Map.GetCoordinatesInRange(GetCoordinates(), STAT_selectedWeapon.range, true); }
        
            public Sprite GetSkin(){ return LoadSkinFromResources(DecryptSkinCode(shipCode)); }

            public void SetSelectedWeapon(int index){ if (weapons[index].weaponName == "NULL"){ return; } selectedWeaponIndex = index; ui.UpdateUI(); }

            public void CycleSelectedWeapon(int amount)
            {
                    if (amount > 0){ if (STAT_nextWeapon.weaponName == "NULL") return; }
                    if (amount < 0){ if (STAT_previousWeapon.weaponName == "NULL") return; }
                selectedWeaponIndex += amount;
                    if (selectedWeaponIndex > 2) selectedWeaponIndex = 0;
                    if (selectedWeaponIndex < 0) selectedWeaponIndex = 2;
                ui.UpdateUI();
            }
        #endregion

        #region Commands
            public static string GenerateShipCode(ShipData ship)
            {
                if (ship.Squad <= 0){ ship.Squad = 1; } //Must always be above 0

                List<Weapon> weaponData = new List<Weapon>(){};
                foreach (var item in ship.Weapons)
                {
                        if (item == null){ Debug.Log("Null weapon data Found! Skipping!"); continue; }
                    weaponData.Add(item.data);
                }
                string weapons = string.Join(",", weaponData.Select(w =>
                    $"{w.weaponName}.{w.damage}.{((int)w.type).ToString()}.{w.salvo}.{w.range}.{string.Join("+", w.area.Select(a => $"{a.x}#{a.y}#{a.z}"))}"
                ));

                int hp = ship.HP
                    + (ship.LanceHull?.hp ?? 0)
                    + (ship.LarboardHull?.hp ?? 0)
                    + (ship.StarboardHull?.hp ?? 0)
                    + (ship.FlankHull?.hp ?? 0);

                int ap = ship.AP
                    + (ship.LanceHull?.ap ?? 0)
                    + (ship.LarboardHull?.ap ?? 0)
                    + (ship.StarboardHull?.ap ?? 0)
                    + (ship.FlankHull?.ap ?? 0);

                int sp = ship.SP
                    + (ship.LanceHull?.sp ?? 0)
                    + (ship.LarboardHull?.sp ?? 0)
                    + (ship.StarboardHull?.sp ?? 0)
                    + (ship.FlankHull?.sp ?? 0);

                int moves = ship.Moves
                    + (ship.LanceHull?.speed ?? 0)
                    + (ship.LarboardHull?.speed ?? 0)
                    + (ship.StarboardHull?.speed ?? 0)
                    + (ship.FlankHull?.speed ?? 0);

                return $"{ship.Name}_{hp}_{ap}_{sp}_{ship.Moves}_{ship.Squad}_{weapons}_{ship.SkinCode}";
            }

            public void ApplyShipCode(string code)
            {
                shipCode = code;

                string[] codes = shipCode.Split('_', StringSplitOptions.None);
                    if (codes.Length < 8)
                    {
                        Debug.LogError($"Invalid ship code: {code}");
                        return;
                    }
                    foreach (var splitCode in codes)
                    {
                        //Debug.Log(splitCode);
                    }
                //Ship Codes
                //_ = Spacer
                //Order = Name, HP, AP, SP, Moves, Squad, Weapons. last is a skin code
                //Weapons = weaponName.damage.type.salvo.range.area#area#areaetc
                //Areas = Coordinate+Coordinate
                //Coordinate = X#Y#Z
                if (isOwner) gameObject.name = codes[0];
                    else { gameObject.name = "[" + GameManager.instance.opponentUsername + "] " + codes[0]; }
                hp = int.Parse(codes[1]);
                ap = int.Parse(codes[2]);
                sp = int.Parse(codes[3]);
                maxSP = sp;
                maxMoves = int.Parse(codes[4]);
                moves = maxMoves;
                squad = int.Parse(codes[5]);
                string[] weaponCodes = codes[6].Split(",", StringSplitOptions.None);
                string skinCode = codes[codes.Length-1];
                    if ((skinCode != null) && (skinCode != "Null")) spriteRenderer.sprite = LoadSkinFromResources(skinCode);
                        else { spriteRenderer.sprite = LoadSkinFromResources("DEFAULT"); }
                
                weapons = new Weapon[3];
                for (int i = 0; i < 3; i++)
                {
                    weapons[i] = new Weapon();
                    weapons[i].weaponName = "NULL";
                        if (weaponCodes.Length < i+1) break;
                        Log(weaponCodes[i]);
                    string[] weaponStatCodes = weaponCodes[i].Split('.');
                    weapons[i].weaponName = weaponStatCodes[0];
                    weapons[i].damage = int.Parse(weaponStatCodes[1]);
                    weapons[i].type   = (ENUM_WeaponType)int.Parse(weaponStatCodes[2]);
                    weapons[i].salvo  = int.Parse(weaponStatCodes[3]);
                    weapons[i].range  = int.Parse(weaponStatCodes[4]);

                    string areaField = weaponStatCodes.Length > 4 ? weaponStatCodes[5] : "";

                    if (string.IsNullOrWhiteSpace(areaField)){ continue; }

                    string[] areaCodes = areaField.Split('+', StringSplitOptions.RemoveEmptyEntries);

                    if (areaCodes.Length > 0)
                    {
                        Log(areaCodes[0]);
                        weapons[i].area = CoordinateCodesToCoordinates(areaCodes);
                    }
                }

                //Setup magazine
                Array.Fill(magazine,false);
            }

            public void FaceHexSide(int sideIndex)
            {
                // sideIndex: 0–5
                float angle = sideIndex * 60f;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
        #endregion

        #region Queries
            public bool QueryCanTravelTo(Vector3Int coordinates)
            {
                if (Map.Distance(GetCoordinates(),coordinates) > moves){ Log("Too far"); return false; }

                    Log("Can Travel to!");
                return true;
            }
        
            public bool QueryWeaponHasAmmo(int index){ return magazine[index]; }

            public bool QueryIsMouseOver(){ return GetComponent<Collider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)); }

            public ENUM_TileType QueryOccupyingTileType(){ return Map.QueryTileTypeAtCoordinates(GetCoordinates()); }
        #endregion

        #region Debug
            [Button] public void DEBUG_MoveToGiven()
            {
                Move(DEBUG_givenCoordinates);
            }

            [Button] public void DEBUG_RotateRight()
            {
                Rotate(ENUM_RotationDirection.Right);
            }
            [Button] public void DEBUG_RotateLeft()
            {
                Rotate(ENUM_RotationDirection.Left);
            }

            [Button] public void DEBUG_PrintSideFacing()
            {
                Debug.Log(GetSideFacing());
            }
        #endregion
    }
}