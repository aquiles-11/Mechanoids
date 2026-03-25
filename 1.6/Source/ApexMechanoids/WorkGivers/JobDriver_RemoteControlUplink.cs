using System.Collections.Generic;
using Verse.AI;
using Verse;
using RimWorld;
using static RimWorld.Planet.WorldGenStep_Roads;

namespace ApexMechanoids
{
    public class JobDriver_RemoteControlUplink : JobDriver
    {
        private const TargetIndex BuildingInd = TargetIndex.A;

        private Thing BuildingThing => job.GetTarget(BuildingInd).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }


        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(BuildingInd);

            this.FailOn(delegate
            {
                if (BuildingThing?.TryGetComp<CompRemoteControlUplink>()?.CanBeUsed == false)
                {
                    return true;
                }
                return false;
            });

            CompRemoteMechCasketAbilities compAbilities = BuildingThing.TryGetComp<CompRemoteMechCasketAbilities>();

            if(compAbilities != null)
            {
                compAbilities.User = pawn;
            }


            yield return Toils_Goto.GotoThing(BuildingInd, PathEndMode.InteractionCell);

            Toil man = ToilMaker.MakeToil("MakeNewToils");
            man.tickAction = delegate
            {
                Pawn actor = man.actor;
                Thing building = actor.CurJob.GetTarget(BuildingInd).Thing;
                CompRemoteControlUplink uplink = building.TryGetComp<CompRemoteControlUplink>();
                if (building == null)
                {
                    actor.jobs.EndCurrentJob(JobCondition.Incompletable);
                    return;
                }

                uplink = building.TryGetComp<CompRemoteControlUplink>();

                if (uplink == null)
                {
                    actor.jobs.EndCurrentJob(JobCondition.Incompletable);
                    return;
                }

                uplink.ManForATick(actor);
                actor.rotationTracker.FaceCell(building.Position);
            };
            man.handlingFacing = true;
            man.defaultCompleteMode = ToilCompleteMode.Never;
            man.FailOnCannotTouch(BuildingInd, PathEndMode.InteractionCell);
   

            yield return man;
        }



    }
}
