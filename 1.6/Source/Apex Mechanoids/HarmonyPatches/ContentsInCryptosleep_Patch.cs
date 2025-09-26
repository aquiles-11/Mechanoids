using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ApexMechanoids
{
    [HarmonyLib.HarmonyPatch()]
    internal static class ContentsInCryptosleep_Patch
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(ThingOwnerUtility), nameof(ThingOwnerUtility.ContentsInCryptosleep));
            yield return AccessTools.Method(typeof(ThingOwnerUtility), nameof(ThingOwnerUtility.ContentsSuspended));
        }

        // Insert
        // if (IsCryptosleepContainer(holder)) return true;
        // 
        // before
        // if (holder is Building_CryptosleepCasket) return true;
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            bool patched = false;
            var label = generator.DefineLabel();
            foreach (var instruction in instructions)
            {
                if (!patched && instruction.opcode == OpCodes.Isinst && (Type)instruction.operand == typeof(Building_CryptosleepCasket))
                {

                    yield return CodeInstruction.Call(typeof(ContentsInCryptosleep_Patch), nameof(ContentsInCryptosleep_Patch.IsCryptosleepContainer));
                    yield return new CodeInstruction(OpCodes.Brfalse_S, label);
                    yield return new CodeInstruction(OpCodes.Ldc_I4_1);
                    yield return new CodeInstruction(OpCodes.Ret);


                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return instruction.WithLabels(label);
                    patched = true;
                }
                else
                {
                    yield return instruction;
                }
            }
            if (!patched)
            {
                Log.Error("Patch failed!");
            }
        }
        public static bool IsCryptosleepContainer(IThingHolder holder)
        {
            return (holder as Pawn_InventoryTracker)?.pawn.def == ApexDefsOf.Mech_Frostivus;
        }
    }
}
