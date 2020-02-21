using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace BetharianPower
{
    /// <summary>
    /// Forces buildings you want to place to go over Betharian Bore Holes.
    /// </summary>
    public class PlaceWorker_BetharianBoreHole : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            Thing thing2 = map.thingGrid.ThingAt(loc, BetharianDefOf.BetharianBoreHole);
            if (thing2 == null || thing2.Position != loc)
            {
                return "MustPlaceOnBetharianBoreHole".Translate();
            }
            return true;
        }

        /*public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null)
        {
            
        }*/

        public override bool ForceAllowPlaceOver(BuildableDef otherDef)
        {
            return otherDef == BetharianDefOf.BetharianBoreHole;
        }
    }
}
