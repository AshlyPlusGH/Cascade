using UnityEngine;

namespace Ash{
    namespace SOEffectSystem{
        [CreateAssetMenu(fileName = "[Effect] New [ENDPOINT][SFX][SOUNDTRACK]", menuName = "EFFECT/ENDPOINT/SFX/++SOUNDTRACK || Play tracks using the Effect System!")]
        public class EFFECT_SFX_Soundtrack : soDATA_Effect
        {
            [Space(10)]

            [Header("-Explicit-")]
            [SerializeField] ENUM_SountrackTag tag;

            public override void Apply(GameObject target = null)
            {
                if (tag != ENUM_SountrackTag.None){ Soundtrack.PlayTrack(tag); }
            }
        }
    }
}