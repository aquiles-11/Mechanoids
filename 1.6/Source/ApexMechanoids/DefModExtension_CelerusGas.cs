using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ApexMechanoids
{
    public class DefModExtension_CelerusGas : DefModExtension
    {
        public List<ThingDef> immuneThingDefs = new List<ThingDef>();

        public IntRange effectDelay = new IntRange(120,120);

        public HediffDef hediffToImmunePawn;

        public List<FleckDef> fleckDefs;

        public EffecterDef effecterDef;

        public DamageDef damageDef;

        public float amount;
    }
}
