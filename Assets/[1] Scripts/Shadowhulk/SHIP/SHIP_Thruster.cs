using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Shadowhulk
{
    public class SHIP_Thruster : MonoBehaviour
    {
        [SerializeField] private bool debug;
            private void Log(string contents){ if (debug){ Debug.Log(contents + " at: " + name); }}

        [Space(10)]

        //Classification Vars
        [SerializeField] private float trailGenerationRate; //Seconds per Trail
        [SerializeField] private float trailDecayRate; //Trail Lifetime

        //Working Vars
        private float generationTimer = 0;
        
        private List<GameObject> generatedTrail = new();

        void Awake(){ Setup(); }
        public void Setup(){  }

        public void Update()
        {
            TryGenerateTrail();
        }

        [Button]
        void TryGenerateTrail()
        {
            generationTimer += Time.deltaTime;

                if (generationTimer > trailGenerationRate) return;
            //Generate Trail
            //Add to List

            generationTimer = 0;
        }
    }
}