using RimWorld;
using Verse;

namespace BetharianPower
{
    /// <summary>
    /// Power plant component which is similar to geothermal power.
    /// </summary>
    public class CompPowerPlantBetharian : CompPowerPlant
    {
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            //steamSprayer = new IntermittentSteamSprayer(parent);
        }

        public override void CompTick()
        {
            base.CompTick();
            if (boreHole == null)
            {
                boreHole = (Building_BetharianBoreHole)parent.Map.thingGrid.ThingAt(parent.Position, BetharianDefOf.BetharianBoreHole);
            }
            if (boreHole != null && parent.IsHashIntervalTick(Building_BetharianBoreHole.glowInterval))
            {
                boreHole.harvester = (Building)parent;
                //steamSprayer.SteamSprayerTick();
                GlowUtility.MakeGlowAt(boreHole.Map, boreHole.OccupiedRect().RandomCell, Rand.Range(2f, 5f));
            }
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            if (boreHole != null)
            {
                boreHole.harvester = null;
            }
        }

        //private IntermittentSteamSprayer steamSprayer;

        private Building_BetharianBoreHole boreHole;
    }
}
