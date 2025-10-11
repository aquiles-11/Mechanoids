using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ApexMechanoids
{
    public class CompAbilityEffect_SpawnThingBetween : CompAbilityEffect
    {
        public new CompProperties_SpawnThingBetween Props => (CompProperties_SpawnThingBetween)props;
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            var casterPos = this.parent.pawn.Position;
            var pos = casterPos + (target.Cell - casterPos) / 2;
            GenSpawn.Spawn(Props.thing, pos, parent.pawn.Map, wipeMode: WipeMode.VanishOrMoveAside);
        }
    }
    public class CompProperties_SpawnThingBetween : CompProperties_AbilityEffect
    {
        public CompProperties_SpawnThingBetween() : base()
        {
            this.compClass = typeof(CompAbilityEffect_SpawnThingBetween);
        }
        public ThingDef thing;
    }
}
