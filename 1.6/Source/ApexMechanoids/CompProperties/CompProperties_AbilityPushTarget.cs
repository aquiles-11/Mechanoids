using RimWorld;
using Verse;

namespace ApexMechanoids
{
    public class CompProperties_AbilityPushTarget : CompProperties_EffectWithDest
    {
        public float maxBodySize = 999f;

        public SoundDef soundLanding;

        public EffecterDef flightEffecterDef;

        public bool flyWithCarriedThing = true;

        public float successChance = 1f;

        public CompProperties_AbilityPushTarget()
        {
            compClass = typeof(CompAbilityEffect_PushTarget);
        }
    }
}
