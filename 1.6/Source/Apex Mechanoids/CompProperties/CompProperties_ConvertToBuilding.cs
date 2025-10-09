using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ApexMechanoids
{
    public class CompProperties_ConvertToBuilding : CompProperties_AbilityEffect
    {
        public ThingDef thingDef;
        public CompProperties_ConvertToBuilding() : base()
        {
            compClass = typeof(CompAbilityEffect_ConvertToBuilding);
        }
    }
}
