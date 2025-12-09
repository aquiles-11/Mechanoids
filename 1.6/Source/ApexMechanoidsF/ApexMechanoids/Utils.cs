using RimWorld;
using Verse;

namespace ApexMechanoidsF
{
    [StaticConstructorOnStartup]
    public static class Utils
    {
        #region -- Logs --
        public static void LogMessage(string str) => Log.Message("<color=#9ba08c>[ApexMechanoids]</color> " + str);
        public static void LogWarning(string str) => Log.Warning("<color=#9ba08c>[ApexMechanoids]</color> " + str);
        public static void LogError(string str) => Log.Error("<color=#9ba08c>[ApexMechanoids]</color> " + str);
        public static void LogErrorOnce(string str, int key) => Log.ErrorOnce("<color=#9ba08c>[ApexMechanoids]</color> " + str, key);
        #endregion

        public static void TryDoAbility(Pawn pawn, AbilityDef abilityDef, LocalTargetInfo target)
        {
            if (!target.IsValid)
            {
                LogError("Invalid ability target: " + target.ToString());
                return;
            }
            Ability ab = pawn.abilities?.GetAbility(abilityDef);
            if (ab == null)
            {
                LogError("subAbility is null");
                return;
            }
            if (!ab.CanCast)
            {
                LogError("Can't cast subAbility");
                return;
            }
            if (!ab.verb.CanHitTarget(target))
            {
                LogError("Could not hit target: " + target + " from " + pawn.Position);
                return;
            }
            ab.QueueCastingJob(target.Pawn, target.Pawn);
        }
    }
}
