using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ApexMechanoids
{
    public class MentalState_Duel : MentalState
    {
        public Thing attachedThing;
        public override void PostStart(string reason)
        {
            base.PostStart(reason);
            pawn.mindState.enemyTarget = this.causedByPawn;
            if (!(this.causedByPawn.MentalState is MentalState_Duel))
            {
                this.causedByPawn.mindState.mentalStateHandler.TryStartMentalState(this.def, reason: reason, forced: true, forceWake: true, causedByMood: false, otherPawn: this.pawn);
                this.causedByPawn.mindState.mentalStateHandler.CurState.forceRecoverAfterTicks = this.forceRecoverAfterTicks;
            }
            pawn.health.AddHediff(ApexDefsOf.APM_InDuel);
        }
        public override RandomSocialMode SocialModeMax() => RandomSocialMode.Off;
        public override void MentalStateTick(int delta)
        {
            base.MentalStateTick(delta);
            if (this.causedByPawn.DeadOrDowned)
            {
                this.RecoverFromState();
            }
        }
        const float severityPerWin = 1f / 8f; // 7 stages
        public override void PostEnd()
        {
            base.PostEnd();
            if (causedByPawn.DeadOrDowned)
            {
                HealthUtility.AdjustSeverity(pawn, ApexDefsOf.APM_DuelWinner, severityPerWin);
            }
            else if (!pawn.DeadOrDowned)
            {
                pawn.health.AddHediff(ApexDefsOf.APM_DuelDraw);
            }
            pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(ApexDefsOf.APM_InDuel));
            if (!attachedThing.DestroyedOrNull())
            {
                attachedThing.Destroy(DestroyMode.KillFinalize);
            }
            if (pawn.drafter?.ShowDraftGizmo ?? false)
            {
                pawn.drafter.Drafted = true;
            }
        }
        public override TaggedString GetBeginLetterText()
        {
            if (this.causedByPawn == null)
            {
                Log.Error("No target. This should have been checked in this mental state's worker.");
                return "";
            }
            return this.def.beginLetter.Formatted(this.pawn.NameShortColored, this.causedByPawn.NameShortColored, this.pawn.Named("PAWN"), this.causedByPawn.Named("TARGET")).AdjustedFor(this.pawn, "PAWN", true).Resolve().CapitalizeFirst();
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref attachedThing, nameof(attachedThing));
        }
    }
}
