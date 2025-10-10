using Verse;
using System.Collections.Generic;

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
    }
}
