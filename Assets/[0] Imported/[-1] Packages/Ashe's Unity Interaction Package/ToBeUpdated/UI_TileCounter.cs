using NaughtyAttributes;
using UnityEngine;

namespace Ash
{
    public class UI_TileCounter : MonoBehaviour
    {
        [SerializeField] private bool debug;
            private void Log(string contents){ if (debug){ Debug.Log(contents + " at: " + name); }}

        [Space(10)]

        [SerializeField] private RectTransform target;
        [SerializeField] private float unitWidth = 16;
        [SerializeField] private float maxWidth = 64;

        [ReadOnly] public int shownValue = -1;

        public void SetValueDirty(int newValue){ shownValue = newValue; TransformOperations.SetRectWidth(target,Mathf.Clamp((unitWidth*shownValue),0,maxWidth)); }

        //DEBUG
            [SerializeField] private int DEBUG_setValue = 1;
        [Button] public void DEBUG_SetValueDirty(){ SetValueDirty(DEBUG_setValue); }
    }
}