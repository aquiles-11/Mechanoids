using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ApexMechanoids
{
    public class Gas_SmokescreenCelerus : Gas
    {
        public const int refreshTicks = 120;

        public const int severityPerTick = 1;

        public int effectDelay;

        public DefModExtension_CelerusGas modExtension => def.GetModExtension<DefModExtension_CelerusGas>();

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad: true);
            destroyTick = Find.TickManager.TicksGame + def.gas.expireSeconds.RandomInRange.SecondsToTicks();
            effectDelay = modExtension.effectDelay.RandomInRange;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref destroyTick, "destroyTick", 0);
        }

        protected override void Tick()
        {
            if (destroyTick <= Find.TickManager.TicksGame)
            {
                Destroy();
            }
            graphicRotation += graphicRotationSpeed;
            if (!Destroyed)
            {
                if (this.IsHashIntervalTick(effectDelay))
                {
                    DoEffect();
                }
                if (this.IsHashIntervalTick(120))
                {
                    Map map = base.Map;
                    IntVec3 position = base.Position;
                    List<Thing> thingList = position.GetThingList(map);
                    if (thingList.Count <= 0)
                    {
                        return;
                    }
                    for (int i = 0; i < thingList.Count; i++)
                    {
                        if (thingList[i] is Pawn)
                        {
                            Pawn pawn = thingList[i] as Pawn;
                            if (pawn != null && !pawn.Dead && pawn.Position == position)
                            {
                                if (!modExtension.immuneThingDefs.NullOrEmpty())
                                {
                                    if (modExtension.immuneThingDefs.Contains(pawn.def)) continue;
                                }
                                DoDamage(pawn);
                            }
                        }
                    }
                }

            }
        }
        public void DoEffect()
        {
            if (Rand.Chance(0.5f)) return;
            if (!modExtension.fleckDefs.NullOrEmpty())
            {
                FleckMaker.Static(PositionHeld, MapHeld, modExtension.fleckDefs.RandomElement());
            }
            if (modExtension.effecterDef != null)
            {
                Effecter effecter = modExtension.effecterDef.Spawn(PositionHeld, MapHeld);
                effecter.Cleanup();
            }
        }
        public void DoDamage(Pawn pawn)
        {
            DamageInfo damageInfo = new DamageInfo(modExtension.damageDef,modExtension.amount,instigator: this);
            pawn.TakeDamage(damageInfo);
        }
    }
}
