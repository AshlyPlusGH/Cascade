using System.Linq;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Cascade {
    public class UI_Cursor : MonoBehaviour
    {
        [SerializeField] private bool debug;
            private void Log(string contents){ if (debug){ Debug.Log(contents + " at: " + name); }}

        public static UI_Cursor instance => FindAnyObjectByType<UI_Cursor>();

        [Space(10)]

        #region Vars
            //Inspector Settings
            [SerializeField] private GenericDictionary<ENUM_CursorSprite,Color> spriteDictionary = new();

            //Components
            [SerializeField] private SpriteRenderer spriteRenderer;
            [SerializeField] private Tilemap highlightTilemap;
            [SerializeField] private Tilemap outOfRangeTilesTilemap;
            [SerializeField] private TextMeshProUGUI tooltipper;

            //Data
            [field:SerializeField][ReadOnly] private ENUM_CursorType mode = ENUM_CursorType.None;
            [field:SerializeField][ReadOnly] private Vector3Int[] highlightedArea;
            [field:SerializeField][ReadOnly] private Vector3Int[] outOfRangeArea;
        #endregion

        void Awake(){ Setup(); }
        public void Setup(){ UpdateTooltipper(); if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>(); }

        public void Update()
        {
            switch (mode)
            {
                case ENUM_CursorType.None: UpdatePosition_Standard(); break;
                case ENUM_CursorType.Movement: UpdatePosition_Movement(); break;
                case ENUM_CursorType.Combat: UpdatePosition_Combat(); break;
            }

            UpdateHighlightedArea();
            
            UpdateTooltipper();
        }

        #region Logic
            void UpdatePosition_Standard()
            {
                    DEBUG_PrintMouseTilePos();
                if (spriteRenderer.color != spriteDictionary[ENUM_CursorSprite.Standard]){ SwitchSprite(ENUM_CursorSprite.Standard); }

                Vector3 mousePosition = Map.GetTilePositionAtMouse();
                Vector3Int mouseCoordinates = Map.GetCoordinatesAtMouse();

                if (Map.QueryIsTileInBounds(mouseCoordinates)) transform.position = mousePosition;
            }
            void UpdatePosition_Movement()
            {
                Vector3 mouseTilePosition = Map.GetTilePositionAtMouse();
                Vector3Int mouseCoords = Map.GetCoordinatesAtMouse();
                Vector3Int shipCoords = Player.local.selected.GetCoordinates();

                //Update Cursor
                if (Map.QueryIsTileValidMovement(mouseCoords))
                {
                    if (Player.local.selected.QueryCanTravelTo(mouseCoords)) SwitchSprite(ENUM_CursorSprite.Valid);
                    else SwitchSprite(ENUM_CursorSprite.Invalid);

                    transform.position = mouseTilePosition;
                }

                //Update Highlights
                var path = Map.GetPath(shipCoords, mouseCoords, Player.local.selected.STAT_moves);
                    foreach (var p in path) Log($"Path step: {p}");

                //Remove Given Coords
                Vector3Int[] coordinateFilter = {
                    shipCoords //For Clarity
                };
                path = path.Where(toRemove => !coordinateFilter.Contains(toRemove)).ToArray();

                //Convert absolute positions → offsets relative to centreCoords
                //highlightedArea is used as an offset so this is required
                highlightedArea = path.Select(p => p - shipCoords).ToArray();
            }
            void UpdatePosition_Combat()
            {
                Vector3 newPosition = Map.GetTilePositionAtMouse();
                Vector3Int newMouseCoords = Map.GetCoordinatesAtMouse();

                Ship targetShip = Ship.GetShipAtCoordinates(newMouseCoords);

                if (targetShip == null){ SwitchSprite(ENUM_CursorSprite.Invalid); }
                else if (Map.Distance(Player.local.selected.STAT_coordinates, targetShip.STAT_coordinates) > Player.local.selected.STAT_selectedWeapon.range){ SwitchSprite(ENUM_CursorSprite.Invalid); }
                else if (targetShip.owner == Player.local.owner){ SwitchSprite(ENUM_CursorSprite.Invalid); }
                else { SwitchSprite(ENUM_CursorSprite.Valid); }

                if (Map.QueryIsTileInBounds(Map.GetCoordinatesAtPosition(newPosition))) transform.position = newPosition;
            }

            void UpdateHighlightedArea()
            {
                    if (highlightedArea == null) return;
                if (mode == ENUM_CursorType.Combat) SetHighlightedTiles(highlightedArea, spriteDictionary[ENUM_CursorSprite.Aiming], true);
                else if (mode == ENUM_CursorType.Movement) SetHighlightedTiles(highlightedArea, spriteDictionary[ENUM_CursorSprite.Valid]);
                else SetHighlightedTiles(highlightedArea, spriteDictionary[ENUM_CursorSprite.Standard]);
            }

            void UpdateOutOfRangeTiles()
            {
                ClearTilemapColors(outOfRangeTilesTilemap);

                    if (outOfRangeArea == null) return;
                foreach (var coordinate in outOfRangeArea) SetTileColor(outOfRangeTilesTilemap,coordinate,spriteDictionary[ENUM_CursorSprite.Invalid]);
            }

            void UpdateTooltipper()
            {
                    int highestPriorityFound = -1;
                    string toPrint = "";
                    GameObject[] tooltipObjects = MouseOperations.GetGameObjectsWithTriggerCollidersUnderMouse<UI_Tooltip>();
                if (tooltipObjects != null) foreach (var tooltipObject in tooltipObjects)
                {
                    UI_Tooltip tooltip = tooltipObject.GetComponent<UI_Tooltip>();
                    if (tooltip.priority > highestPriorityFound) toPrint = tooltip.contents; highestPriorityFound = tooltip.priority;
                }
                if (toPrint == "") tooltipper.color = new Color(0,0,0,0);
                else tooltipper.color = Color.white;
                tooltipper.text = toPrint;
            }

            void ClearTilemapColors(Tilemap target){ foreach (Vector3Int coords in target.cellBounds.allPositionsWithin){ SetTileColor(target, coords, new Color (0,0,0,0)); }}
        #endregion

        void SetHighlightedTiles(Vector3Int[] targetCoords, Color color, bool evenRowPresethighlightedArea = false)
        {
            ClearTilemapColors(highlightTilemap);
            //Find Centre Coords
            Vector3Int centreCoords = Map.GetCoordinatesAtMouse();
            if (mode == ENUM_CursorType.Movement) centreCoords = Player.local.selected.GetCoordinates();
            //Highlight Targets
            foreach (Vector3Int offset in targetCoords)
            {
                Vector3Int tile = centreCoords + offset;

                if (evenRowPresethighlightedArea) if ((centreCoords.y & 1) != 0){ if ((tile.y & 1) == 0) tile.x++; } 

                SetTileColor(highlightTilemap, tile, color);
            }
        }

        public void SetOutOfRangeTiles(Vector3Int[] targetCoords)
        {
            outOfRangeArea = targetCoords;
            UpdateOutOfRangeTiles();
        }

        void SetTileColor(Tilemap tilemap, Vector3Int coords, Color color){ tilemap.SetColor(coords, color); }

        #region Commands
            public static void Show(){ instance.INSTANCE_Show(); }
                private void INSTANCE_Show(){ spriteRenderer.enabled = true; }
            public static void Hide(){ instance.INSTANCE_Hide(); }
                private void INSTANCE_Hide(){ spriteRenderer.enabled = false; }

            public static void ShowWeaponRange(){ instance.INSTANCE_ShowWeaponRange(); }
                private void INSTANCE_ShowWeaponRange(){ outOfRangeTilesTilemap.gameObject.SetActive(true); }
            public static void HideWeaponRange(){ instance.INSTANCE_HideWeaponRange(); }
                private void INSTANCE_HideWeaponRange(){ outOfRangeTilesTilemap.gameObject.SetActive(false); }

            public static void SwitchSprite(ENUM_CursorSprite sprite){ instance.INSTANCE_SwitchSprite(sprite); }
                private void INSTANCE_SwitchSprite(ENUM_CursorSprite sprite){ spriteRenderer.color = spriteDictionary[sprite]; }
            public static void SwitchMode(ENUM_CursorType newMode, Vector3Int[] highlightedArea = null){ instance.INSTANCE_SwitchMode(newMode, highlightedArea); }
                private void INSTANCE_SwitchMode(ENUM_CursorType newMode, Vector3Int[] newhighlightedArea = null)
                {
                    mode = newMode;
                    highlightedArea = newhighlightedArea;
                        if (highlightedArea == null)  ClearTilemapColors(highlightTilemap);
                }
        #endregion

        #region DEBUG
            public void DEBUG_PrintMouseTilePos()
            {
                    if (!MouseOperations.IsMousePositionValid()){ Log("Mouse Position Invalid!"); return; }
                    if (Camera.main == null){ Log("Main Camera not Found!"); return; }
                Log("Mouse Pos: " + Map.GetCoordinatesAtPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
            }

            [Button]
            public void DEBUG_SwitchToNormal(){ SwitchMode(ENUM_CursorType.None); }
            [Button]
            public void DEBUG_SwitchToMovement(){ SwitchMode(ENUM_CursorType.Movement); }
            [Button]
            public void DEBUG_SwitchToCombat(){ SwitchMode(ENUM_CursorType.Combat); }
        #endregion
    }
}