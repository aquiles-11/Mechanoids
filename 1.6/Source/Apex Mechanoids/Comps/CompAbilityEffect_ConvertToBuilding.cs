using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ApexMechanoids
{
    public class CompAbilityEffect_ConvertToBuilding : CompAbilityEffect
    {
        public new CompProperties_ConvertToBuilding Props => (CompProperties_ConvertToBuilding)props;

        public Pawn pawn => parent.pawn;
        public override void PostApplied(List<LocalTargetInfo> targets, Map map)
        {
            base.PostApplied(targets, map);
            Thing thing = ThingMaker.MakeThing(Props.thingDef);
            thing.SetFactionDirect(pawn.Faction);
            GenSpawn.Spawn(thing, pawn.Position, pawn.Map, thing.def.defaultPlacingRot);
            CompPowerPlantDynamo compPowerPlantDynamo = thing.TryGetComp<CompPowerPlantDynamo>();
            if (compPowerPlantDynamo != null)
            {
                compPowerPlantDynamo.SwithcToPowerPlantMode(pawn);
            }
        }
    }
}
