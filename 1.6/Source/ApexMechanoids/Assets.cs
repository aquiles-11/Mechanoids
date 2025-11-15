using UnityEngine;
using Verse;

namespace ApexMechanoids
{
    [StaticConstructorOnStartup]
    public static class Assets
    {
        public static readonly Texture2D shieldRepairIcon = ContentFinder<Texture2D>.Get("UI/ShieldRepair");
    }
}
