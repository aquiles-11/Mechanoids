using RimWorld;
using Verse;
using Verse.AI;

namespace ApexMechanoidsF
{
    // Copied from Rimpact
    public class Verb_CastAbilityJumpExt : Verb_CastAbilityJump
    {
        public override ThingDef JumpFlyerDef => verbProps.spawnDef ?? base.JumpFlyerDef;

        public override void OrderForceTarget(LocalTargetInfo target)
        {
            OrderJump(CasterPawn, target, this, EffectiveRange);
        }

        public void OrderJump(Pawn pawn, LocalTargetInfo target, Verb verb, float range)
        {
            Map map = pawn.Map;
            IntVec3 intVec = RCellFinder.BestOrderedGotoDestNear(target.Cell, pawn, (IntVec3 c) => JumpUtility.ValidJumpTarget(pawn, map, c) && JumpUtility.CanHitTargetFrom(pawn, pawn.Position, c, range), reachable: false);
            Job job = JobMaker.MakeJob(JobDefOf.CastJump, intVec);
            job.ability = Ability;
            job.verbToUse = verb;
            if (pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc))
            {
                FleckMaker.Static(intVec, map, FleckDefOf.FeedbackGoto); 
            }
        }
    }
}
