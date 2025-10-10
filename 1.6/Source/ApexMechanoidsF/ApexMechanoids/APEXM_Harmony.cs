using Verse;
using HarmonyLib;

namespace ApexMechanoidsF
{
    [StaticConstructorOnStartup]
    public static class APEXM_Harmony
    {
        static APEXM_Harmony()
        {
            Harmony harmony = new Harmony("flangopink.ApexMechanoidsF");
            harmony.PatchAll();
        }
    }
}
