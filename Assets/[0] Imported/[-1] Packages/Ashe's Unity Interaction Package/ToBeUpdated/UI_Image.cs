using UnityEngine;
using UnityEngine.UI;

namespace Ash
{
    [RequireComponent(typeof(Image))]
    public class UI_Image : MonoBehaviour
    {

        [SerializeField] private Image target;
        [SerializeField] private Color visibleColor = Color.white;
        [SerializeField] private Sprite offSprite;
        [SerializeField] private Sprite onSprite;
        [SerializeField] private Sprite[] localSprites;

        void Awake(){ Setup(); }
        public void Setup(){ if (target == null) target = GetComponent<Image>(); }

        public void SetSprite(Sprite newSprite = null)
        {
                if (newSprite == null){ target.color = new Color(0,0,0,0); return; }
            target.sprite = newSprite;
            target.color = visibleColor;
        }

        public void SetSpriteLocal(int index){ target.sprite = localSprites[index]; }

        public void SwitchSpritesBoolean()
        {
            if (target.sprite == onSprite) target.sprite = offSprite;
            else target.sprite = onSprite;
        }

        public void SetColor(Color newColor){ visibleColor = newColor; if (target.color.a != 0) target.color = visibleColor; }
    }
}