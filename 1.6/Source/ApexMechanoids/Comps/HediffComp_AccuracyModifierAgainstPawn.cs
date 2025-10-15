using Verse;

namespace ApexMechanoids
{
    public class HediffCompProperties_AccuracyModifierAgainstPawn : HediffCompProperties
    {
        public float amount; // 0.04 for 4;
        public HediffCompProperties_AccuracyModifierAgainstPawn() => compClass = typeof(HediffComp_AccuracyModifierAgainstPawn);
    }

    public class HediffComp_AccuracyModifierAgainstPawn : HediffComp
    {
        public HediffCompProperties_AccuracyModifierAgainstPawn Props => (HediffCompProperties_AccuracyModifierAgainstPawn)props;
        public override string CompTipStringExtra => $"{Sign}{Amount.ToStringPercent()} {"AM.AccuracyHediff.Label".Translate()}"; // "+4 accuracy against this pawn."
        private string Sign => Amount < 0 ? "" : "+";
        public float Amount => Props.amount;
    }
}
