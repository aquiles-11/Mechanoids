using RimWorld;
using Verse;

namespace ApexMechanoids
{
    public class CompProperties_GazerShockwaveController : CompProperties
    {
        public AbilityDef shockwaveAbilityDef;
        public float healthThreshold = 0.2f;
        public int checkIntervalTicks = 30;

        public CompProperties_GazerShockwaveController()
        {
            compClass = typeof(CompGazerShockwaveController);
        }
    }

    public class CompGazerShockwaveController : ThingComp
    {
        public CompProperties_GazerShockwaveController Props => (CompProperties_GazerShockwaveController)props;

        private Pawn Pawn => parent as Pawn;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            EnsureAbility();
        }

        public override void CompTick()
        {
            base.CompTick();

            Pawn pawn = Pawn;
            if (pawn == null || !pawn.Spawned || pawn.Dead || pawn.Downed)
            {
                return;
            }

            if (!pawn.IsHashIntervalTick(Props.checkIntervalTicks > 0 ? Props.checkIntervalTicks : 30))
            {
                return;
            }

            EnsureAbility();

            if (pawn.Faction == Faction.OfPlayer)
            {
                return;
            }

            if (!ShouldAutoCast(pawn))
            {
                return;
            }

            Ability ability = pawn.abilities?.GetAbility(Props.shockwaveAbilityDef);
            if (ability == null || !ability.CanCast)
            {
                return;
            }

            if (pawn.CurJobDef == ability.def.jobDef)
            {
                return;
            }

            ability.QueueCastingJob(pawn, pawn);
        }

        private void EnsureAbility()
        {
            Pawn pawn = Pawn;
            if (pawn?.abilities == null || Props.shockwaveAbilityDef == null)
            {
                return;
            }

            if (pawn.abilities.GetAbility(Props.shockwaveAbilityDef) == null)
            {
                pawn.abilities.GainAbility(Props.shockwaveAbilityDef);
            }
        }

        private bool ShouldAutoCast(Pawn pawn)
        {
            return pawn.health?.summaryHealth != null && pawn.health.summaryHealth.SummaryHealthPercent <= Props.healthThreshold;
        }
    }
}