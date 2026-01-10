using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;

namespace ApexMechanoids
{
    public class CompProperties_RepairAura : CompProperties
    {
        public float radius = 6f;            // How far the aura reaches
        public float healAmount = 2f;        // HP healed per tick cycle
        public int tickInterval = 60;        // How often it pulses
        public float idlePower = 500f;       // Power when not healing
        public float activePower = 2000f;    // Power when healing

        public CompProperties_RepairAura()
        {
            this.compClass = typeof(CompRepairAura);
        }
    }

    public class CompRepairAura : ThingComp
    {
        public CompProperties_RepairAura Props => (CompProperties_RepairAura)props;

        private CompPowerTrader powerComp;
        private int tickCounter = 0;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            powerComp = parent.GetComp<CompPowerTrader>();
        }

        public override void CompTick()
        {
            base.CompTick();

            tickCounter++;
            if (tickCounter < Props.tickInterval)
                return;

            tickCounter = 0;

            if (powerComp != null && !powerComp.PowerOn)
                return;

            bool healedAny = false;

            // Find mechanoids in radius
            IEnumerable<Pawn> mechanoids = parent.Map.mapPawns.AllPawnsSpawned
                .Where(p =>
                    p.RaceProps.IsMechanoid &&
                    p.Faction == parent.Faction &&
                    p.Position.DistanceTo(parent.Position) <= Props.radius &&
                    p.health.hediffSet.hediffs.Any(h => h is Hediff_Injury));

            foreach (Pawn mech in mechanoids)
            {
                Hediff_Injury injury = mech.health.hediffSet.hediffs
                    .OfType<Hediff_Injury>()
                    .Where(h => !h.IsPermanent())
                    .FirstOrDefault();

                if (injury != null)
                {
                    injury.Heal(Props.healAmount);
                    healedAny = true;
                }
            }

            // Adjust power draw
            if (powerComp != null)
            {
                powerComp.PowerOutput = healedAny ? -Props.activePower : -Props.idlePower;
            }
        }

        public override string CompInspectStringExtra()
        {
            return $"Repair radius: {Props.radius}m";
        }
    }
}