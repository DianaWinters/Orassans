using UnityEngine;
using Verse;

namespace BetharianPower
{
    /// <summary>
    /// Generates deposits of Batharian on map creation if possible.
    /// </summary>
    public class GenStep_BetharianDeposits : GenStep_Scatterer
    {
        private const float LumpSizeFactor = 1.6f;

        public int minLumpSize = 10;
        public int maxLumpSize = 30;

        public override int SeedPart => 456453584;

        /*protected override void ScatterAt(IntVec3 loc, Map map, int count = 1)
        {
            if(map.GetComponent<BetharianDeposits>() is BetharianDeposits betharianDeposits)
            {
                int numCells = Mathf.CeilToInt((float)this.GetScatterLumpSizeRange().RandomInRange * LumpSizeFactor);
                foreach (IntVec3 c2 in GenAdj.OccupiedRect(loc, Rot4.Random, new IntVec2(5, 5)))
                //foreach (IntVec3 c2 in GridShapeMaker.IrregularLump(loc, map, numCells))
                {
                    //map.deepResourceGrid.SetAt(c2, thingDef, thingDef.deepCountPerCell);
                    betharianDeposits.grid.SetStrengthAt(c2, 1);
                }
            }
        }*/

        protected override void ScatterAt(IntVec3 loc, Map map, GenStepParams parms, int count = 1)
        {
            if (map.GetComponent<BetharianDeposits>() is BetharianDeposits betharianDeposits)
            {
                int numCells = Mathf.CeilToInt((float)this.GetScatterLumpSizeRange().RandomInRange * LumpSizeFactor);
                foreach (IntVec3 c2 in GenAdj.OccupiedRect(loc, Rot4.Random, new IntVec2(5, 5)))
                //foreach (IntVec3 c2 in GridShapeMaker.IrregularLump(loc, map, numCells))
                {
                    //map.deepResourceGrid.SetAt(c2, thingDef, thingDef.deepCountPerCell);
                    betharianDeposits.grid.SetStrengthAt(c2, 1);
                }
            }
        }

        private IntRange GetScatterLumpSizeRange()
        {
            return new IntRange(minLumpSize, maxLumpSize);
        }
    }
}
