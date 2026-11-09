using UnityEngine;
using System.Collections.Generic;
using FMODUnity;
using System;
using FMOD.Studio;

namespace Ash {
    [CreateAssetMenu]
    public class SFX : ScriptableObject
    {
        [SerializeField] private static bool debug;
        private static void Log(string contents){ if (debug){ Debug.Log(contents); }}

        private static readonly Dictionary<ENUM_SFXTag, string> lookupTable = new()
        {
            { ENUM_SFXTag.ButtonPress, "event:/SFX/ButtonPressed" },
            { ENUM_SFXTag.Interact, "event:/SFX/Interact" },
            { ENUM_SFXTag.Open, "event:/SFX/Open" },
            { ENUM_SFXTag.ShipSpawned, "event:/SFX/ShipSpawned" },
            { ENUM_SFXTag.KeyPress, "event:/SFX/KeyPressed" },
            { ENUM_SFXTag.Close, "event:/SFX/Close" },
            { ENUM_SFXTag.ShipDamaged, "event:/SFX/ShipDamaged" },
            { ENUM_SFXTag.ShipDestroyed, "event:/SFX/ShipDestroyed" },
            { ENUM_SFXTag.WeaponFireDefault, "event:/SFX/WeaponFireDefault" },
        };

        public static VCA volumeController => RuntimeManager.GetVCA("vca:/SFX");

        public static void PlaySFX(ENUM_SFXTag sfx){ RuntimeManager.PlayOneShot(lookupTable[sfx]); }

        public static void SetVolume(float newVolume){ volumeController.setVolume(newVolume); }

        public ENUM_SFXTag argument;

        public void PlaySFXTriggerIndex(int sfx){ PlaySFX((ENUM_SFXTag)sfx); }
        public void PlaySFXSetArgument(){ PlaySFX(argument); }
        public void PlaySFXTriggerString(string sfx){ if (System.Enum.TryParse<ENUM_SFXTag>(sfx, true, out var tag)){ PlaySFX(tag); }}
    }

    public enum ENUM_SFXTag
    {
        None,
        ButtonPress,
        Interact,
        Open,
        ShipSpawned,
        KeyPress,
        Close,
        ShipDamaged,
        ShipDestroyed,
        WeaponFireDefault,
    }
}