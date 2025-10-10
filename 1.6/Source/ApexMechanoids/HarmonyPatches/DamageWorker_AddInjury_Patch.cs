using Verse;
using Verse.Noise;

namespace ApexMechanoids
{
    internal static class DamageWorker_AddInjury_Patch
    {
        public static bool pickShield;
        public static BodyPartGroupDef whichShield;
        private const float DAMAGE_SIDE_CHANCE = 0.2f;
        [HarmonyLib.HarmonyPatch(typeof(DamageWorker_AddInjury), "GetExactPartFromDamageInfo")]
        internal static class GetExactPartFromDamageInfo
        {
            private static void Prefix(DamageInfo dinfo, Pawn pawn)
            {
                if (!(dinfo.Instigator is Pawn pawn2) || pawn.kindDef != ApexDefsOf.Mech_Aegis)
                {
                    return;
                }

                var angle = (pawn2.DrawPos - pawn.DrawPos).AngleFlat();
                var rot = Pawn_RotationTracker.RotFromAngleBiased(angle);

                if (rot == pawn.Rotation)
                {
                    TryPickShieldForFrontAttack(pawn);
                }
                else if (IsSideAttack(rot, pawn.Rotation) && Rand.Chance(DAMAGE_SIDE_CHANCE))
                {
                    TryPickShieldForSideAttack(pawn, rot);
                }
            }

            private static void TryPickShieldForFrontAttack(Pawn pawn)
            {
                bool checkRightFirst = Rand.Chance(0.5f);
                var firstShield = checkRightFirst ? ApexDefsOf.RightAegisShield : ApexDefsOf.LeftAegisShield;
                var secondShield = checkRightFirst ? ApexDefsOf.LeftAegisShield : ApexDefsOf.RightAegisShield;

                if (TryPickShield(pawn, firstShield) || TryPickShield(pawn, secondShield))
                {
                    return;
                }
            }

            private static void TryPickShieldForSideAttack(Pawn pawn, Rot4 attackRot)
            {
                bool isRightSide = IsRightSideAttack(attackRot, pawn.Rotation);
                var shieldDef = isRightSide ? ApexDefsOf.RightAegisShield : ApexDefsOf.LeftAegisShield;
                TryPickShield(pawn, shieldDef);
            }

            private static bool TryPickShield(Pawn pawn, BodyPartGroupDef shieldDef)
            {
                var targetBodyPart = Utils.GetNonMissingBodyPart(pawn, ApexDefsOf.AegisShield, shieldDef);
                if (targetBodyPart != null)
                {
                    whichShield = shieldDef;
                    pickShield = true;
                    return true;
                }
                return false;
            }

            private static bool IsSideAttack(Rot4 attackRot, Rot4 pawnRot)
            {
                int rotDiff = (attackRot.AsInt - pawnRot.AsInt + 4) % 4;
                return rotDiff == 1 || rotDiff == 3;
            }

            private static bool IsRightSideAttack(Rot4 attackRot, Rot4 pawnRot)
            {
                int rotDiff = (attackRot.AsInt - pawnRot.AsInt + 4) % 4;
                return rotDiff == 1;
            }

            private static void Postfix()
            {
                pickShield = false;
            }
        }
    }
}