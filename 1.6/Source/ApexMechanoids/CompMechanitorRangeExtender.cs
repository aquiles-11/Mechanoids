using UnityEngine;
using Verse;

namespace ApexMechanoids
{
    public class CompProperties_MechanitorRangeExtender : Verse.CompProperties
    {
        public float maxRange;
        public float minRange;

        public CompProperties_MechanitorRangeExtender() => compClass = typeof(CompMechanitorRangeExtender);
    }

    public class CompMechanitorRangeExtender : ThingComp
    {
        public CompProperties_MechanitorRangeExtender Props => (CompProperties_MechanitorRangeExtender)props;
        private Pawn Pawn => parent as Pawn;

        private float cachedDistance;

        public float currentRange;

        public float SquaredDistance => cachedDistance == 0f ? GetCacheDistance() : cachedDistance;

        private float GetCacheDistance() => cachedDistance = Mathf.Pow(currentRange, 2f);

        public override void PostDraw()
        {
            base.PostDraw();
            if (!Pawn.Drafted) return;
            Pawn overseer = Pawn.GetOverseer();
            if (overseer == null) return;
            if (overseer.MapHeld == Pawn.MapHeld)
            {
                currentRange = Props.maxRange;
            }
            else if (!overseer.Spawned)
            {
                currentRange = Props.minRange;
            }
            GenDraw.DrawRadiusRing(parent.Position, currentRange, Color.cyan);
        }
    }
}
