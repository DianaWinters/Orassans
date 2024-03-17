using RimWorld;
using System.Collections.Generic;
using Verse;

namespace BetharianPower
{
    /// <summary>
    /// Requires the blueprint to be fully covered by Betharian stone in order to be placed.
    /// </summary>
    public class PlaceWorker_BetharianStoneDepositCoverage : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            if (map.GetComponent<BetharianDeposits>() is BetharianDeposits betharianDeposits && BetharianDefOf.BetharianScanning.IsFinished)
            {
                List<Building> allBuildingsColonist = map.listerBuildings.allBuildingsColonist;
                for (int i = 0; i < allBuildingsColonist.Count; i++)
                {
                    Building thing2 = allBuildingsColonist[i];
                    CompDeepScanner compDeepScanner = thing2.TryGetComp<CompDeepScanner>();
                    if (compDeepScanner != null)
                    {
                        CompPowerTrader compPowerTrader = thing2.TryGetComp<CompPowerTrader>();
                        if (compPowerTrader == null || compPowerTrader.PowerOn)
                        {
                            //Check Betharian stone coverage.
                            CellRect occupiedRect = GenAdj.OccupiedRect(loc, rot, checkingDef.Size);
                            int area = occupiedRect.Width * occupiedRect.Height;
                            int coveredArea = 0;
                            foreach (IntVec3 cell in occupiedRect)
                            {
                                if (betharianDeposits.grid.StrengthAt(cell) > 0)
                                    coveredArea++;
                            }

                            if (coveredArea >= area)
                                return AcceptanceReport.WasAccepted;

                            break;
                        }
                    }
                }
            }

            return AcceptanceReport.WasRejected;
        }

        /*public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null)
        {
            
        }*/
    }
}
