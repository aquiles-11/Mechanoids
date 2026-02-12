using RimWorld;
using Verse;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ApexMechanoids
{
	public class CompProperties_StatFactorsFromHealth : CompProperties
	{
		public class StatFactorFromHealth
		{
			public StatFactorFromHealth() { }

			public StatDef stat;

			public SimpleCurve curve;
		}

		public List<StatFactorFromHealth> statFactors;

		public CompProperties_StatFactorsFromHealth()
		{
			compClass = typeof(CompStatFactorsFromHealth);
		}
	}
	public class CompStatFactorsFromHealth : ThingComp
	{
		public Pawn Pawn => parent as Pawn;

		public CompProperties_StatFactorsFromHealth Props => (CompProperties_StatFactorsFromHealth)props;

		public override float GetStatFactor(StatDef stat)
		{
			float num = base.GetStatFactor(stat);
			foreach(CompProperties_StatFactorsFromHealth.StatFactorFromHealth item in Props.statFactors)
			{
				if(item.stat == null)
				{
					Log.Error("E");
				}
				if(item.stat == stat)
				{
					float num2 = Pawn.health.summaryHealth.SummaryHealthPercent;
					num = item.curve.Evaluate(num2);
					Log.Warning(num2.ToStringPercentEmptyZero() + "/" +num.ToStringPercentEmptyZero() + "/" + item.curve.Evaluate(0.1f));
					return num;
				}
			}
			return num;
		}
	}
}