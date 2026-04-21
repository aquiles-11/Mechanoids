using UnityEngine;
using Verse;

namespace ApexMechanoids
{
    public class Verb_StarfallAbility : Verb_LaunchProjectile
    {
        private const int CooldownTicks = 18000;
        private int lastCastTickInternal = -99999;

        private static readonly ProjectileHitFlags HitFlags = ProjectileHitFlags.IntendedTarget | ProjectileHitFlags.NonTargetPawns;

        public override ThingDef Projectile => DefDatabase<ThingDef>.GetNamed("APM_ArtilleryProjectile");

        public override bool Available()
        {
            if (!base.Available()) return false;
            return Find.TickManager.TicksGame - lastCastTickInternal >= CooldownTicks;
        }

        public override bool TryCastShot()
        {
            if (currentTarget.HasThing && currentTarget.Thing.Map != caster.Map)
                return false;

            IntVec3 dest = currentTarget.Cell;
            float angle = Rand.Range(0f, 360f);
            float dist = Rand.Range(0f, verbProps.forcedMissRadius > 0f ? verbProps.forcedMissRadius : 8.9f);
            IntVec3 scattered = dest + new IntVec3(
                Mathf.RoundToInt(Mathf.Cos(angle * Mathf.Deg2Rad) * dist),
                0,
                Mathf.RoundToInt(Mathf.Sin(angle * Mathf.Deg2Rad) * dist)
            );
            if (!scattered.InBounds(caster.Map))
                scattered = dest;

            Projectile proj = (Projectile)GenSpawn.Spawn(Projectile, caster.Position, caster.Map);
            proj.Launch(caster, new LocalTargetInfo(scattered), new LocalTargetInfo(scattered), HitFlags);

            if (CasterPawn != null)
                CasterPawn.rotationTracker.FaceCell(scattered);

            if (burstShotsLeft == ShotsPerBurst)
                lastCastTickInternal = Find.TickManager.TicksGame;

            lastShotTick = Find.TickManager.TicksGame;
            return true;
        }
    }
}
