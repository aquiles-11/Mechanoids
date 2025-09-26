using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ApexMechanoids
{
    public class FloatMenuOptionProvider_PickUp_Frostivus : FloatMenuOptionProvider
    {
        protected override bool Drafted => true;

        protected override bool Undrafted => true;

        protected override bool Multiselect => true;

        protected override bool MechanoidCanDo => true;

        public override bool SelectedPawnValid(Pawn pawn, FloatMenuContext context)
        {
            return base.SelectedPawnValid(pawn, context) && pawn.def == ApexDefsOf.Mech_Frostivus;
        }

        public override IEnumerable<FloatMenuOption> GetOptionsFor(Thing clickedThing, FloatMenuContext context)
        {
            if (!clickedThing.def.EverHaulable)
            {
                yield break;
            }
            if (clickedThing.def.ingestible == null || !clickedThing.HasComp<CompRottable>())
            {
                yield break;
            }
            if (!context.FirstSelectedPawn.CanReach(clickedThing, Verse.AI.PathEndMode.ClosestTouch, Danger.Deadly))
            {
                yield return new FloatMenuOption("CannotPickUp".Translate(clickedThing.Label, clickedThing) + ": " + "NoPath".Translate().CapitalizeFirst(), null);
            }
            else if (MassUtility.WillBeOverEncumberedAfterPickingUp(context.FirstSelectedPawn, clickedThing, 1))
            {
                yield return new FloatMenuOption("CannotPickUp".Translate(clickedThing.Label, clickedThing) + ": " + "TooHeavy".Translate().CapitalizeFirst(), null);
            }
            else
            {
                if (MassUtility.WillBeOverEncumberedAfterPickingUp(context.FirstSelectedPawn, clickedThing, clickedThing.stackCount))
                {
                    yield return new FloatMenuOption("CannotPickUpAll".Translate(clickedThing.Label, clickedThing) + ": " + "TooHeavy".Translate(), null);
                }
                else
                {
                    yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("PickUpAll".Translate(clickedThing.Label, clickedThing), () =>
                    {
                        clickedThing.SetForbidden(false, false);
                        Job job = JobMaker.MakeJob(JobDefOf.TakeInventory, clickedThing);
                        job.count = clickedThing.stackCount;
                        job.checkEncumbrance = true;
                        job.takeInventoryDelay = 120;
                        context.FirstSelectedPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc, false);
                    }, MenuOptionPriority.High), context.FirstSelectedPawn, clickedThing, "ReservedBy");
                }
                yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("PickUpSome".Translate(clickedThing.LabelNoCount, clickedThing), () =>
                {
                int to = Mathf.Min(MassUtility.CountToPickUpUntilOverEncumbered(context.FirstSelectedPawn, clickedThing), clickedThing.stackCount);
                string text = "PickUpCount".Translate(clickedThing.LabelNoCount, clickedThing);
                int from = 1;

                Dialog_Slider window = new Dialog_Slider(text, from, to, (int count) =>
                {
                    clickedThing.SetForbidden(false, false);
                    Job job = JobMaker.MakeJob(JobDefOf.TakeInventory, clickedThing);
                    job.count = count;
                    job.checkEncumbrance = true;
                    job.takeInventoryDelay = 120;
                    context.FirstSelectedPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc, false);
                });
                Find.WindowStack.Add(window);
            }, MenuOptionPriority.High), context.FirstSelectedPawn, clickedThing, "ReservedBy", null);
        }
        }
    }
}
