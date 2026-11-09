using UnityEngine;

namespace Ash{
    namespace SOEffectSystem{
        [CreateAssetMenu(fileName = "[Effect] New [ENDPOINT][SFX]", menuName = "EFFECT/ENDPOINT/SFX/++SFX || Play SFX using the Effect System!")]
        public class soDATA_EFFECT_SFX : soDATA_Effect
        {
            [Space(10)]

            [Header("-Explicit-")]
            [SerializeField] ENUM_SFXTag tag;

            public override void Apply(GameObject target = null)
            {
                if (tag != ENUM_SFXTag.None){ SFX.PlaySFX(tag); }
            }
        }
    }
}