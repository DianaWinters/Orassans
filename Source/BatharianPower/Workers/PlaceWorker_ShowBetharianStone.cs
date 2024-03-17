using RimWorld;
using System.Collections.Generic;
using Verse;
using UnityEngine;

namespace BetharianPower
{
    /// <summary>
    /// Shows Betharian stone on the map when placing.
    /// </summary>
    public class PlaceWorker_ShowBetharianStone : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            if (Find.CurrentMap is Map map && map.GetComponent<BetharianDeposits>() is BetharianDeposits betharianDeposits && BetharianDefOf.BetharianScanning.IsFinished)
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
                            betharianDeposits.MarkForDraw();
                            break;
                        }
                    }
                }
            }
        }

        /*public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol)
        {
            
        }*/
    }
}
