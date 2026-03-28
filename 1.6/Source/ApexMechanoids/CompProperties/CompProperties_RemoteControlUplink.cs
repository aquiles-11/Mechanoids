using Verse;

namespace ApexMechanoids
{
    public class CompProperties_RemoteControlUplink : CompProperties
    {
        public WorkTags manWorkType = WorkTags.None;

        public CompProperties_RemoteControlUplink()
        {
            compClass = typeof(CompRemoteControlUplink);
        }
    }
}
