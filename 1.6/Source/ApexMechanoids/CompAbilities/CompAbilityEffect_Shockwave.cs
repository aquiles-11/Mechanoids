using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ApexMechanoids
{
    public class CompAbilityEffect_Shockwave : CompAbilityEffect
    {
        public new CompProperties_Shockwave Props => (CompProperties_Shockwave)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            Pawn caster = parent.pawn;
            Map map = caster?.MapHeld;
            if (caster == null || map == null || !caster.Spawned)
            {
                return;
            }

            IntVec3 center = caster.PositionHeld;
            SpawnBurstVisuals(caster, map, center);
            SpawnEmitter(caster, map, center);
            DoExplosion(caster, map, center);
            ApplySecondaryEffects(caster, map, center);

            if (Props.killCaster)
            {
                caster.Kill(null);
            }
        }

        private void SpawnBurstVisuals(Pawn caster, Map map, IntVec3 center)
        {
            if (Props.flashThingDef != null && center.InBounds(map))
            {
                Thing flashThing = ThingMaker.MakeThing(Props.flashThingDef);
                GenSpawn.Spawn(flashThing, center, map, WipeMode.Vanish);
            }

            if (Props.burstEffecterDef != null)
            {
                Effecter effecter = Props.burstEffecterDef.Spawn(center, map);
                if (effecter != null)
                {
                    TargetInfo targetInfo = new TargetInfo(center, map);
                    effecter.EffectTick(targetInfo, TargetInfo.Invalid);
                    effecter.Cleanup();
                }
            }

            FleckMaker.Static(caster.DrawPos, map, FleckDefOf.ExplosionFlash, 4.6f);
            FleckMaker.ThrowDustPuffThick(caster.DrawPos, map, 3.8f, Color.gray);

            if (!Props.castSoundDefName.NullOrEmpty())
            {
                SoundDef castSound = DefDatabase<SoundDef>.GetNamedSilentFail(Props.castSoundDefName);
                castSound?.PlayOneShot(new TargetInfo(center, map));
            }
        }

        private void SpawnEmitter(Pawn caster, Map map, IntVec3 center)
        {
            if (Props.emitterDef == null)
            {
                return;
            }

            Mote_ShockwaveEmitter emitter = ThingMaker.MakeThing(Props.emitterDef) as Mote_ShockwaveEmitter;
            if (emitter == null)
            {
                return;
            }

            emitter.Initialize(caster.DrawPos, center, Props.radius, Props.ringIntervalTicks, Props.visualScale, Props.ringFleckDefName);
            GenSpawn.Spawn(emitter, center, map);
        }

        private void DoExplosion(Pawn caster, Map map, IntVec3 center)
        {
            DamageDef damageDef = Props.damageDef ?? DamageDefOf.Bomb;
            GenExplosion.DoExplosion(center, map, Props.radius, damageDef, caster, Props.damageAmount, Props.armorPenetration, null, null, null);
        }

        private void ApplySecondaryEffects(Pawn caster, Map map, IntVec3 center)
        {
            IReadOnlyList<Pawn> pawns = map.mapPawns.AllPawnsSpawned;
            for (int i = 0; i < pawns.Count; i++)
            {
                Pawn pawn = pawns[i];
                if (!ShouldAffectPawn(caster, pawn, center))
                {
                    continue;
                }

                if (pawn.stances?.stunner != null)
                {
                    pawn.stances.stunner.StunFor(Props.stunTicks, caster, addBattleLog: false);
                }

                if (Props.pushDamageDef != null)
                {
                    Vector3 angleVec = pawn.DrawPos - caster.DrawPos;
                    float angle = angleVec.AngleFlat();
                    DamageInfo pushDamage = new DamageInfo(Props.pushDamageDef, Props.pushDamageAmount, 0f, angle, caster);
                    pawn.TakeDamage(pushDamage);
                }
            }
        }

        private bool ShouldAffectPawn(Pawn caster, Pawn pawn, IntVec3 center)
        {
            if (pawn == null || pawn == caster || pawn.Dead || !pawn.Spawned)
            {
                return false;
            }

            if (pawn.Position.DistanceTo(center) > Props.radius)
            {
                return false;
            }

            return true;
        }
    }

    public class CompProperties_Shockwave : CompProperties_AbilityEffect
    {
        public CompProperties_Shockwave()
        {
            compClass = typeof(CompAbilityEffect_Shockwave);
        }

        public ThingDef emitterDef;
        public ThingDef flashThingDef;
        public EffecterDef burstEffecterDef;
        public DamageDef damageDef;
        public DamageDef pushDamageDef;
        public float radius = 8.9f;
        public int damageAmount = 72;
        public float armorPenetration = 1.4f;
        public int stunTicks = 240;
        public int pushDamageAmount = 1;
        public int ringIntervalTicks = 2;
        public float visualScale = 1.22f;
        public string ringFleckDefName = "PsycastPsychicEffect";
        public string castSoundDefName = "PsycastPsychicPulse";
        public bool killCaster = true;
    }

    public class CompAbility_ShockwaveWarmup : AbilityComp
    {
        private Effecter chargeEffecter;
        private int ticksUntilNextSpark;

        public CompProperties_ShockwaveWarmup Props => (CompProperties_ShockwaveWarmup)props;

        public override void CompTick()
        {
            base.CompTick();

            Pawn caster = parent.pawn;
            Map map = caster?.MapHeld;
            if (caster == null || map == null || !caster.Spawned)
            {
                Cleanup();
                return;
            }

            if (!parent.wasCastingOnPrevTick)
            {
                Cleanup();
                return;
            }

            if (chargeEffecter == null && Props.chargeEffecterDef != null)
            {
                chargeEffecter = Props.chargeEffecterDef.Spawn(caster.PositionHeld, map);
            }

            if (chargeEffecter != null)
            {
                TargetInfo targetInfo = new TargetInfo(caster.PositionHeld, map);
                chargeEffecter.offset = caster.DrawPos - caster.PositionHeld.ToVector3Shifted();
                chargeEffecter.EffectTick(targetInfo, TargetInfo.Invalid);
                chargeEffecter.ticksLeft--;
            }

            if (ticksUntilNextSpark > 0)
            {
                ticksUntilNextSpark--;
                return;
            }

            ticksUntilNextSpark = Mathf.Max(Props.sparkIntervalTicks, 1);
            SpawnChargeFlecks(caster, map);
        }

        private void SpawnChargeFlecks(Pawn caster, Map map)
        {
            FleckDef fleckDef = DefDatabase<FleckDef>.GetNamedSilentFail(Props.chargeFleckDefName);
            if (fleckDef == null || !caster.PositionHeld.ShouldSpawnMotesAt(map))
            {
                return;
            }

            int burstCount = 3;
            for (int i = 0; i < burstCount; i++)
            {
                float angle = Rand.Range(0f, 360f);
                Vector3 radial = Quaternion.AngleAxis(angle, Vector3.up) * (Vector3.forward * Rand.Range(0.22f, Props.sparkRadius));
                Vector3 pos = caster.DrawPos + radial;
                FleckCreationData data = FleckMaker.GetDataStatic(pos, map, fleckDef, Props.visualScale * Rand.Range(0.9f, 1.25f));
                data.rotation = angle;
                data.rotationRate = Rand.Range(-35f, 35f);
                data.velocityAngle = angle;
                data.velocitySpeed = Rand.Range(0.05f, 0.14f);
                map.flecks.CreateFleck(data);
            }
        }

        private void Cleanup()
        {
            ticksUntilNextSpark = 0;
            if (chargeEffecter != null)
            {
                chargeEffecter.Cleanup();
                chargeEffecter = null;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref ticksUntilNextSpark, nameof(ticksUntilNextSpark));
        }
    }

    public class CompProperties_ShockwaveWarmup : AbilityCompProperties
    {
        public CompProperties_ShockwaveWarmup()
        {
            compClass = typeof(CompAbility_ShockwaveWarmup);
        }

        public EffecterDef chargeEffecterDef;
        public string chargeFleckDefName = "PsycastPsychicEffect";
        public int sparkIntervalTicks = 5;
        public float sparkRadius = 1.25f;
        public float visualScale = 0.58f;
    }

    public class Mote_ShockwaveEmitter : Mote
    {
        private IntVec3 originCell;
        private float radius;
        private int ringIntervalTicks;
        private float visualScale;
        private string ringFleckDefName;
        private int ticksUntilNextRing;
        private int currentRing;
        private int maxRing;
        private FleckDef cachedRingFleck;

        public void Initialize(Vector3 exactPos, IntVec3 originCell, float radius, int ringIntervalTicks, float visualScale, string ringFleckDefName)
        {
            this.exactPosition = exactPos;
            this.originCell = originCell;
            this.radius = radius;
            this.ringIntervalTicks = Mathf.Max(ringIntervalTicks, 1);
            this.visualScale = visualScale;
            this.ringFleckDefName = ringFleckDefName;
            this.currentRing = 0;
            this.maxRing = Mathf.CeilToInt(radius);
            this.ticksUntilNextRing = 0;
        }

        public override void Tick()
        {
            base.Tick();

            if (Destroyed || MapHeld == null)
            {
                return;
            }

            if (ticksUntilNextRing > 0)
            {
                ticksUntilNextRing--;
                return;
            }

            EmitRing(currentRing);
            currentRing++;
            if (currentRing > maxRing)
            {
                Destroy();
                return;
            }

            ticksUntilNextRing = ringIntervalTicks;
        }

        private void EmitRing(int ringIndex)
        {
            if (!originCell.ShouldSpawnMotesAt(MapHeld))
            {
                return;
            }

            if (cachedRingFleck == null)
            {
                cachedRingFleck = DefDatabase<FleckDef>.GetNamedSilentFail(ringFleckDefName);
            }

            if (cachedRingFleck == null)
            {
                return;
            }

            if (ringIndex <= 0)
            {
                FleckCreationData centerData = FleckMaker.GetDataStatic(exactPosition, MapHeld, cachedRingFleck, visualScale * 1.35f);
                centerData.rotationRate = Rand.Range(-18f, 18f);
                centerData.velocitySpeed = 0f;
                MapHeld.flecks.CreateFleck(centerData);
                return;
            }

            float bandMin = ringIndex - 0.7f;
            float bandMax = ringIndex + 0.5f;
            foreach (IntVec3 cell in GenRadial.RadialCellsAround(originCell, ringIndex + 0.45f, true))
            {
                if (!cell.InBounds(MapHeld) || !cell.ShouldSpawnMotesAt(MapHeld))
                {
                    continue;
                }

                float distance = cell.DistanceTo(originCell);
                if (distance < bandMin || distance > bandMax)
                {
                    continue;
                }

                Vector3 pos = cell.ToVector3Shifted() + new Vector3(Rand.Range(-0.14f, 0.14f), 0f, Rand.Range(-0.14f, 0.14f));
                float angle = (cell - originCell).AngleFlat;
                FleckCreationData data = FleckMaker.GetDataStatic(pos, MapHeld, cachedRingFleck, visualScale * Rand.Range(0.95f, 1.25f));
                data.rotation = angle;
                data.rotationRate = Rand.Range(-40f, 40f);
                data.velocityAngle = angle;
                data.velocitySpeed = Rand.Range(0.12f, 0.28f);
                MapHeld.flecks.CreateFleck(data);

                if (Rand.Chance(0.28f))
                {
                    FleckMaker.ThrowDustPuffThick(pos, MapHeld, Rand.Range(0.75f, 1.25f), Color.gray);
                }
            }
        }
    }
}