using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace ApexMechanoids
{
    public class MentalState_Duel : MentalState
    {
        public override void PostStart(string reason)
        {
            base.PostStart(reason);
            pawn.mindState.enemyTarget = this.causedByPawn;
            if (!(this.causedByPawn.MentalState is MentalState_Duel))
            {
                this.causedByPawn.mindState.mentalStateHandler.TryStartMentalState(this.def, reason: reason, forced: true, forceWake: true, causedByMood: false, otherPawn: this.pawn);
                this.causedByPawn.mindState.mentalStateHandler.CurState.forceRecoverAfterTicks = this.forceRecoverAfterTicks;
            }
        }
        public override RandomSocialMode SocialModeMax() => RandomSocialMode.Off;
        public override void PostEnd()
        {
            base.PostEnd();
            
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
    }
}
