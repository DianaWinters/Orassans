using Verse;

namespace OrassanBombardment
{
    class CompOrassanShield : ThingComp
    {
        
    }

    class CompProperties_OrassanShield : CompProperties
    {
        public CompProperties_OrassanShield() => this.compClass = typeof(CompOrassanShield);
    }
}