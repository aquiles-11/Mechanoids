﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ApexMechanoids
{
    public class CompAbilityEffect_SpawnThingDuelAttached : CompAbilityEffect
    {
        public new CompProperties_SpawnThingDuelAttached Props => (CompProperties_SpawnThingDuelAttached)props;
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            var casterPos = this.parent.pawn.Position;
            var pos = casterPos + (target.Cell - casterPos) / 2;
            var thing = GenSpawn.Spawn(Props.thing, pos, parent.pawn.Map, wipeMode: WipeMode.VanishOrMoveAside);
            if (target.Pawn?.MentalState is MentalState_Duel duel)
            {
                duel.attachedThing = thing;
                if (duel.causedByPawn?.MentalState is MentalState_Duel duel1)
                {
                    duel1.attachedThing = thing;
                }
            }
            else
            {
                Log.Error($"Cannot attach {thing} to duel.");
            }
        }
    }
    public class CompProperties_SpawnThingDuelAttached : CompProperties_AbilityEffect
    {
        public CompProperties_SpawnThingDuelAttached() : base()
        {
            this.compClass = typeof(CompAbilityEffect_SpawnThingDuelAttached);
        }
        public ThingDef thing;
    }
}
