using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ApexMechanoids
{
    public static class Utils
    {
        public static BodyPartRecord GetNonMissingBodyPart(Pawn pawn, BodyPartDef def, BodyPartGroupDef group = null)
        {
            foreach (var notMissingPart in pawn.health.hediffSet.GetNotMissingParts())
            {
                if (notMissingPart.def == def)
                {
                    if (group != null && !notMissingPart.groups.Contains(group))
                    {
                        continue;
                    }
                    return notMissingPart;
                }
            }

            return null;
        }

        public static List<BodyPartRecord> GetNonMissingBodyParts(Pawn pawn, BodyPartDef def)
        {
            List<BodyPartRecord> matchingParts = new List<BodyPartRecord>();

            foreach (var notMissingPart in pawn.health.hediffSet.GetNotMissingParts())
            {
                if (notMissingPart.def == def)
                {
                    matchingParts.Add(notMissingPart);
                }
            }

            return matchingParts;
        }
        public static void TryDoAbility(Pawn pawn, AbilityDef abilityDef, LocalTargetInfo target)
        {
            if (!target.IsValid)
            {
                Log.Error("Invalid ability target: " + target.ToString());
                return;
            }
            Ability ab = pawn.abilities?.GetAbility(abilityDef);
            if (ab == null)
            {
                Log.Error("subAbility is null");
                return;
            }
            if (!ab.CanCast)
            {
                Log.Error("Can't cast subAbility");
                return;
            }
            if (!ab.verb.CanHitTarget(target))
            {
                Log.Error("Could not hit target: " + target + " from " + pawn.Position);
                return;
            }
            ab.QueueCastingJob(target.Pawn, target.Pawn);
        }
    }
}
