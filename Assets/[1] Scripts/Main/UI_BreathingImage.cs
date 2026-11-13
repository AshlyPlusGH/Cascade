using UnityEngine;
using UnityEngine.UI;

public class UI_BreathingImage : MonoBehaviour
{
    [SerializeField] private Image target;
    [SerializeField] private PrimeTween.TweenSettings<Vector3> tweenSettings;

    public void Awake(){ PrimeTween.Tween.Scale(target.rectTransform,tweenSettings); }
}