using RimWorld;
using System.Collections.Generic;
using Verse.AI;
using Verse;

namespace ApexMechanoids
{
    public class CompRemoteControlUplink : CompMannable
    {
        new public CompProperties_RemoteControlUplink Props => (CompProperties_RemoteControlUplink)props;   //overwritten with new, since we inherit from CompMannable. Adding CompMannable seperate will add an extra rightclick option.

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn pawn)
        {
            if (pawn == null || !pawn.RaceProps.ToolUser)
            {
                yield break;
            }

            if (!ModsConfig.BiotechActive)
            {
                yield break;
            }

            if (!MechanitorUtility.IsMechanitor(pawn))
            {
                yield return new FloatMenuOption("CannotManThing".Translate(parent.LabelShort, parent) + " (" + "APM.CommandCasket.FailReason.NotMechanitor".Translate() + ")", null);
                yield break;
            }

            if (!pawn.CanReserveAndReach(parent, PathEndMode.InteractionCell, Danger.Deadly))
            {
                yield return new FloatMenuOption("CannotManThing".Translate(parent.LabelShort, parent) + " (" + "CannotReach".Translate() + ")", null);
                yield break;
            }

            if (!CanBeUsed)
            {
                yield return new FloatMenuOption("CannotManThing".Translate(parent.LabelShort, parent) + " (" + "NoPower".Translate() + ")", null);
                yield break;
            }

            if (Props.manWorkType != WorkTags.None && pawn.WorkTagIsDisabled(Props.manWorkType))
            {
                if (Props.manWorkType == WorkTags.Violent)
                {
                    yield return new FloatMenuOption("CannotManThing".Translate(parent.LabelShort, parent) + " (" + "IsIncapableOfViolenceLower".Translate(pawn.LabelShort, pawn) + ")", null);
                }
                yield break;
            }

            AcceptanceReport canControl = pawn.mechanitor.CanControlMechs;
            if (!canControl)
            {
                yield return new FloatMenuOption("CannotManThing".Translate(parent.LabelShort, parent) + " (" + canControl.Reason + ")", null);
                yield break;
            }


            yield return new FloatMenuOption("OrderManThing".Translate(parent.LabelShort, parent), delegate
            {
                Job job = JobMaker.MakeJob(ApexDefsOf.APM_RemoteControlUplink, parent);
                pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            });
        }

        public CompPowerTrader Powercomp => parent.GetComp<CompPowerTrader>();

        public bool CanBeUsed
        {
            get
            {
                if(Powercomp != null)
                {
                    return Powercomp.PowerOn;
                }
                return true;
            }
        }


        public override string CompInspectStringExtra()
        {
            if (ManningPawn != null && ManningPawn.mechanitor != null)
            {
                return "APM.CommandCasket.Inspection".Translate(ManningPawn.Named("PAWN")).CapitalizeFirst();
            }
            return null;
        }
    }

}
