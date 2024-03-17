using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Orassans
{
    [StaticConstructorOnStartup]
    public class Placeworker_Orassan : PlaceWorker
    {
        public static List<TerrainDef> acceptableTerrains = new List<TerrainDef>();

        static Placeworker_Orassan()
        {
            //Fill out our list.
            //Vanilla
            acceptableTerrains.Add(TerrainDefOf.Soil);
            acceptableTerrains.Add(TerrainDefOf.Gravel);
            acceptableTerrains.Add(TerrainDefOf.Ice);

            acceptableTerrains.Add(TerrainDef.Named("MarshyTerrain"));
            acceptableTerrains.Add(TerrainDef.Named("MossyTerrain"));
            acceptableTerrains.Add(TerrainDef.Named("SoilRich"));

            //Orassan
            acceptableTerrains.Add(TerrainDef.Named("OrassanSoil"));
        }

        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            //Get Terrain under designation location.
            TerrainDef terrainAt = map.terrainGrid.TerrainAt(loc);

            //Check if acceptableTerrains have terrainAt, if it do not have then reject.
            if (acceptableTerrains.Contains(terrainAt))
                return AcceptanceReport.WasAccepted;

            return AcceptanceReport.WasRejected;
        }

        /*public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null)
        {
            
        }*/
    }
}
