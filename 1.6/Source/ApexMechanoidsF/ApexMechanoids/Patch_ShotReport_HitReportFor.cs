using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ApexMechanoidsF
{
    [HarmonyPatch(typeof(ShotReport), "AimOnTargetChance_StandardTarget", MethodType.Getter)]
    public static class Patch_ShotReport_AimOnTargetChance_StandardTarget
    {
        public static List<Hediff> modifierHediffs;
        public static float resultOffset = 0;

        [HarmonyPostfix]
        public static void Postfix(ref float __result, TargetInfo ___target)
        {
            if (___target.HasThing)
            {
                if (___target.Thing is Pawn targetPawn)
                {
                    float val = 0;
                    modifierHediffs = targetPawn.health.hediffSet.hediffs.Where(x => x.def.HasComp(typeof(HediffComp_AccuracyModifierAgainstPawn))).ToList();
                    foreach (Hediff h in modifierHediffs)
                    {
                        val += h.TryGetComp<HediffComp_AccuracyModifierAgainstPawn>().Amount;
                    }
                    resultOffset = val;
                    __result += resultOffset;
                }
            }
        }
    }

    [HarmonyPatch(typeof(ShotReport), "GetTextReadout")]
    public static class Patch_ShotReport_GetTextReadout
    {
        private static string Text => "\n   " + "Hediffs" + ": " + Patch_ShotReport_AimOnTargetChance_StandardTarget.resultOffset.ToStringPercent();

        [HarmonyPostfix]
        public static void Postfix(ref string __result)
        {
            __result += Text;
        }
    }
}
