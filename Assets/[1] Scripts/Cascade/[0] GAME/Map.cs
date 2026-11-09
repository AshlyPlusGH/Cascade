using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class Map
{
    static bool debug = false;

    static Tilemap terrain => UnityEngine.Object.FindAnyObjectByType<TAG_Tilemap>().GetComponent<Tilemap>();
    static Tilemap borders => UnityEngine.Object.FindAnyObjectByType<TAG_TilemapBorders>().GetComponent<Tilemap>();

    static TAG_TilemapDeploymentZone[] deploymentZones => UnityEngine.Object.FindObjectsByType<TAG_TilemapDeploymentZone>();

    static TileBase asteroidTile => Resources.Load<TileBase>("Tiles/Asteroid");
    static TileBase debrisTile => Resources.Load<TileBase>("Tiles/Debris");
    static TileBase gasTile => Resources.Load<TileBase>("Tiles/Gas");
    static TileBase nebulaeTile => Resources.Load<TileBase>("Tiles/Nebulae");

    static Dictionary<Vector3Int, ENUM_TileType> generatedTiles = new();

    //999 999 999
    static Int32 seed = 205620562;
    static int seedPicker = 0;

    #region Commands
        static public void ResetStatics(){ seedPicker = 0; generatedTiles = new(); }

        static public void ClearAllTiles()
        {
            generatedTiles = new();
            terrain.ClearAllTiles();
        }

        static public bool CreateRandom(Int32 setSeed)
        {
                if (debug) Debug.Log("Creating Random Map!");
            ClearAllTiles();

            seed = setSeed;

            int variationMod = 0;
            seedPicker = 0; 
            foreach (Vector3Int tilePos in borders.cellBounds.allPositionsWithin)
            {
                    if (borders.GetTile(tilePos) == null){ continue; }
                int randInt = GetRandomFromSeed() + variationMod;
                if (randInt > 9){ randInt -= 10; }

                //Cases
                //0-5 Empty
                //6 Asteroid
                //7 Debris
                //8 Gas
                //9 Nebulae
                switch (randInt)
                {
                    case 6:
                        terrain.SetTile(tilePos,asteroidTile);
                        generatedTiles.Add(tilePos,ENUM_TileType.Asteroid);
                        break;
                    case 7:
                        terrain.SetTile(tilePos,debrisTile);
                        generatedTiles.Add(tilePos,ENUM_TileType.Debris);
                        break;
                    case 8:
                        terrain.SetTile(tilePos,gasTile);
                        generatedTiles.Add(tilePos,ENUM_TileType.Gas);
                        break;
                    case 9:
                        terrain.SetTile(tilePos,nebulaeTile);
                        generatedTiles.Add(tilePos,ENUM_TileType.Nebulae);
                        break;
                }

                variationMod++;
                if (variationMod > 10){ variationMod -= 10; }
            }

            return true;
        }
        static int GetRandomFromSeed()
        {
            seedPicker++;
                if (seedPicker > 9){ seedPicker = 1; }
            return (seed / (int)Mathf.Pow(10, seedPicker)) % 10;
        }
    #endregion

    #region Getters and Setters
        static public  Int32 GetRandomSeed(){ return UnityEngine.Random.Range(100000000,999999999); }

        static public Vector2 GetPositionAtCoordinates(Vector3Int coordinates){ return terrain.GetCellCenterWorld(new Vector3Int(coordinates.x,coordinates.y,0)); }

        static public Vector3Int GetCoordinatesAtPosition(Vector3 worldPos)
        {
            Vector3Int cell = terrain.WorldToCell(worldPos);
            return new Vector3Int(cell.x, cell.y);
        }

        static public Vector2 GetTilePositionAtMouse(){ return GetPositionAtCoordinates(GetCoordinatesAtPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition))); }

        static public Vector3Int GetCoordinatesAtMouse(){ return GetCoordinatesAtPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition)); }

        static public Vector3Int[] GetCoordinatesInBounds()
        {
            List<Vector3Int> coords = new List<Vector3Int>();

            // Tilemap bounds are already compressed to only include used tiles
            BoundsInt bounds = terrain.cellBounds;

            foreach (var pos in bounds.allPositionsWithin)
            {
                if (borders.HasTile(pos))
                    coords.Add(pos);
            }

            return coords.ToArray();
        }

        public static Vector3Int[] GetPath(Vector3Int start, Vector3Int goal, int pathLengthLimiter = 25)
        {
            List<Vector3Int> workingPath = new();

            Vector3Int currentPos = start;
            workingPath.Add(currentPos);

            int limiter = pathLengthLimiter;
            while (limiter > 0){
                //Find all Movement Options
                Vector3Int[] neighbours = new Vector3Int[6];
                if (currentPos.y % 2 == 0) { Array.Copy(EvenQ_Neighbors_EvenRow, neighbours, 6); }
                else { Array.Copy(EvenQ_Neighbors_OddRow, neighbours, 6); }
                for (int i = 0; i < neighbours.Length; i++){ neighbours[i] += currentPos; }

                if (neighbours.Contains(goal)){ break; }
                else
                {
                    int[] neighbourValues = new int[6];

                    for (int i = 0; i < neighbours.Length; i++) neighbourValues[i] = Distance(neighbours[i],goal);

                    currentPos = neighbours[Array.IndexOf(neighbourValues, neighbourValues.Min())];
                }

                //Add movement Step
                workingPath.Add(currentPos);

                limiter--;
            }

            workingPath.Add(goal);

            return workingPath.ToArray();
        }

        public static Vector3Int[] GetCoordinatesInRange(Vector3Int centre, int range, bool isInverted = false)
        {
                if (debug) Debug.Log("Getting all coordinates in range of " + centre + " with range of " + range + ". Inverted Rules are " + isInverted);
            List<Vector3Int> toReturn = new();

            foreach (var tileCoords in GetCoordinatesInBounds())
            {
                if (isInverted)
                {
                    if (Distance(tileCoords,centre) > range)
                    {
                            if (debug) Debug.Log("Is Inverted rules and Distance of: " + Distance(tileCoords,centre) + ". Exceeds range of: " + range);
                        toReturn.Add(tileCoords);
                    } 
                }
                else
                {
                    if (Distance(tileCoords,centre) <= range) toReturn.Add(tileCoords); if (debug) Debug.Log
                    ("Is Not Inverted rules and Distance of: " + Distance(tileCoords,centre) + ". Is within range of: " + range);
                }
            }

                if (debug) Debug.Log("Coordinates are: ");
                if (debug) foreach (var item in toReturn) Debug.Log(item);
            return toReturn.ToArray();
        }
    #endregion

    #region Queries
        static public int Distance(Vector3Int a, Vector3Int b)
        {
            int x = Mathf.Abs(b.x - a.x);  // absolute deltas
            int y = Mathf.Abs(b.y - a.y);
            // special case if we start on an odd row or if we move into negative x direction
            if (((b.x - a.x) < 0)^((a.y&1)==1))
                x = Mathf.Max(0, x - (y + 1) / 2);
            else
                x = Mathf.Max(0, x - y / 2);
            return x + y;
        }

        static public bool QueryIsTileInBounds(Vector3Int tileCoordinates)
        {
            if (!borders.HasTile(tileCoordinates)){ return false; } //Out of Bounds

            return true;
        }

        static public bool QueryIsTileOccupied(Vector3Int tileCoordinates)
        {
            foreach (var ship in UnityEngine.Object.FindObjectsByType<Ship>()){ if (ship.GetCoordinates() == tileCoordinates){ return true; }} //Tile Occupied

            return false;
        }

        static public bool QueryIsTileValidMovement(Vector3Int tileCoordinates)
        {
                if (!QueryIsTileInBounds(tileCoordinates)){ return false; } //Out of Bounds
                if (QueryIsTileOccupied(tileCoordinates)){ return false; } //Tile Occupied
            return true;
        }

        static public bool QueryIsTileValidDeployment(Vector3Int tileCoordinates, int[] zoneCodes)
        {
                if (!QueryIsTileInBounds(tileCoordinates)){ return false; }
                if (QueryIsTileOccupied(tileCoordinates)){ return false; }
            foreach (var zoneCode in zoneCodes){ if (QueryIsTileInDeploymentZone(tileCoordinates,zoneCode) == true){ return true; }}

            return false;
        }
    
        static public bool QueryIsTileInDeploymentZone(Vector3Int tileCoordinates, int zoneCode)
        {
            //Get Deployment Zone
            Tilemap deploymentZone = null;
            foreach (TAG_TilemapDeploymentZone zone in deploymentZones)
            {
                if (zone.STAT_code == zoneCode){ deploymentZone = zone.GetComponent<Tilemap>(); }
            }
            if (deploymentZone == null){ return false; }
            if (!deploymentZone.HasTile(tileCoordinates)){ return false; }

            return true;
        }

        static public ENUM_TileType QueryTileTypeAtCoordinates(Vector3Int tileCoordinates)
        {
                if (!generatedTiles.ContainsKey(tileCoordinates)) return ENUM_TileType.None;
            return generatedTiles[tileCoordinates];
        }
    #endregion

    //Pathfind Reqs.
    static readonly Vector3Int[] EvenQ_Neighbors_EvenRow =
    {
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(-1, 1, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(-1, -1, 0),
        new Vector3Int(0, -1, 0),
    };
    static readonly Vector3Int[] EvenQ_Neighbors_OddRow =
    {
        new Vector3Int(1, 0, 0),
        new Vector3Int(1, 1, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(1, -1, 0),
    };

    class Node
    {
        public Vector3Int pos;
        public int g;
        public int h;
        public int f => g + h;
        public int dirIndex;
        public Node parent;
    }
}