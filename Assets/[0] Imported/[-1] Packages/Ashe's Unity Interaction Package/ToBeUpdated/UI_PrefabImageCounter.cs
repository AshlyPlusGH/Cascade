using System;
using NaughtyAttributes;
using UnityEngine;

namespace Ash
{
    public class UI_PrefabImageCounter : MonoBehaviour
    {
        [SerializeField] private bool debug;
            private void Log(string contents){ if (debug){ Debug.Log(contents + " at: " + name); }}

        [Space(10)]

        [SerializeField] private RectTransform target;
        [SerializeField] private GameObject imagePrefab;

        [ReadOnly] public int shownValue = -1;
        [SerializeField] private int maxValue = 256;

        public void SetValueDirty(int newValue)
        {
                newValue = Math.Clamp(newValue,-1,maxValue);
            //Set Value
            shownValue = newValue;

            //Cleanup
            TransformOperations.DestroyChildren(target);

            //Reprint
            for (int i = 0; i < shownValue; i++){ Instantiate(imagePrefab,target); }
        }

        //DEBUG
            [SerializeField] private int DEBUG_setValue = 1;
        [Button] public void DEBUG_SetValueDirty(){ SetValueDirty(DEBUG_setValue); }
    }
}