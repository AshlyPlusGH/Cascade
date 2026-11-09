using Ash;
using UnityEngine;

#pragma warning disable 0108
public class PlaySFX : MonoBehaviour
{
    [SerializeField] private ENUM_SFXTag tag;
    
    public void Play(){ SFX.PlaySFX(tag); }
}