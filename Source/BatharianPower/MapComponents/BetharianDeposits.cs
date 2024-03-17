using Verse;

namespace BetharianPower
{
    /// <summary>
    /// This map component tracks the deposits of Batharian in the map.
    /// </summary>
    public class BetharianDeposits : MapComponent
    {
        /// <summary>
        /// The Betharian stone grid.
        /// </summary>
        public BetharianGrid grid;

        public BetharianDeposits(Map map) : base(map)
        {
            grid = new BetharianGrid(map);
        }

        public void MarkForDraw()
        {
            grid.MarkForDraw();
        }

        public override void MapComponentUpdate()
        {
            grid.GridUpdate();
        }

        public override void ExposeData()
        {
            Scribe_Deep.Look(ref grid, "grid", new object[] { map });
        }
    }
}
