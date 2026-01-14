using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ApexMechanoids
{
    public class CompAbilityEffect_FrostivusCryptosleep : CompAbilityEffect
    {
        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            if (!base.Valid(target, throwMessages))
            {
                return false;
            }
            if (target.Thing is Pawn pawn && pawn.Faction == parent.pawn.Faction)
            {
                if (MassUtility.CountToPickUpUntilOverEncumbered(parent.pawn, pawn) > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            var deselected = target.Thing.DeSpawnOrDeselect();
            parent.pawn.inventory.GetDirectlyHeldThings().TryAdd(target.Thing);
            if (deselected)
            {
                Find.Selector.Select(target.Thing, false, false);
            }
        }
    }
}
