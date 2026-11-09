using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Shadowhulk
{
    public class Map : MonoBehaviour
    {
        [SerializeField] private bool debug;
            private void Log(string contents){ if (debug){ Debug.Log(contents + " at: " + name); }}

        [Space(10)]

        [SerializeField] private Image var1;

        static float perlinNoiseScale = 0.1f;

        void Awake(){ Setup(); }
        public void Setup(){ var1.material.mainTexture = CreateRandomMap().nodes[0].terrain; }

        public MapData CreateRandomMap(int nodeQuantity = 1)
        {
            MapData toReturn = new MapData{};
            toReturn.nodes = new(){};
            
            for (int i = 0; i < nodeQuantity; i++) toReturn.nodes.Add(CreateMapNode(64));


            return toReturn;
        }
        public MapNodeData CreateMapNode(int nodeSize)
        {
            MapNodeData toReturn = new();
            toReturn.terrain = new(nodeSize,nodeSize);
            //For every coord on terrain
            for (int y = 0; y < toReturn.terrain.height; y++) for (int x = 0; x < toReturn.terrain.width; x++)
            {
                //For each Biome
                float weight = Mathf.PerlinNoise(x*perlinNoiseScale,y*perlinNoiseScale);
                //If surpasses Biome Spawn Threshold
                if (weight > 0.5f) toReturn.terrain.SetPixel(x,y,Color.red);
            }
            toReturn.terrain.Apply();

            return toReturn;
        }
    }

    public struct MapData
    {
        public List<MapNodeData> nodes;
    }
    public struct MapNodeData
    {
        public Texture2D terrain;
    }
    public struct BiomeSettings
    {
        public Color color;
        public float spawnThreshold; //How Plentiful Biomes will be
        public Vector2 uniqueSeedOffset;
    }
}