using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ApexMechanoids
{
    public class Verb_Absorb : Verb_CastAbilityTouch
    {
        public override bool TryStartCastOn(LocalTargetInfo castTarg, LocalTargetInfo destTarg, bool surpriseAttack = false, bool canHitNonTargetPawns = true, bool preventFriendlyFire = false, bool nonInterruptingSelfCast = false)
        {
            if (base.TryStartCastOn(castTarg, destTarg, surpriseAttack, canHitNonTargetPawns, preventFriendlyFire, nonInterruptingSelfCast))
            {
                CasterPawn.Drawer.renderer.SetAnimation(ApexDefsOf.APM_EatingIngestor);
                return true;
            }
            return false;
        }
        public override void Reset()
        {
            base.Reset();
            CasterPawn.Drawer.renderer.SetAnimation(null);
        }
        public override void WarmupComplete()
        {
            base.WarmupComplete();
            CasterPawn.Drawer.renderer.SetAnimation(null);
        }
    }
}
