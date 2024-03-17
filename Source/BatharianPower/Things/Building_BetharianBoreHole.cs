using RimWorld;
using Verse;
using Verse.Sound;

namespace BetharianPower
{
    /// <summary>
    /// Variant of steam geyser which throw glowing particles up instead of steam.
    /// </summary>
    public class Building_BetharianBoreHole : Building
    {
        public static int glowInterval = 50;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            /*steamSprayer = new IntermittentSteamSprayer(this);
            steamSprayer.startSprayCallback = new Action(this.StartSpray);
            steamSprayer.endSprayCallback = new Action(this.EndSpray);*/
        }

        private void StartSpray()
        {
            //SnowUtility.AddSnowRadial(this.OccupiedRect().RandomCell, base.Map, 4f, -0.06f);
            spraySustainer = SoundDefOf.GeyserSpray.TrySpawnSustainer(new TargetInfo(base.Position, base.Map, false));
            spraySustainerStartTick = Find.TickManager.TicksGame;
        }

        private void EndSpray()
        {
            if (spraySustainer != null)
            {
                spraySustainer.End();
                spraySustainer = null;
            }
        }

        public override void Tick()
        {
            if (harvester == null && this.IsHashIntervalTick(glowInterval))
            {
                //steamSprayer.SteamSprayerTick();
                GlowUtility.MakeGlowAt(Map, this.OccupiedRect().RandomCell, Rand.Range(3f, 6f));
            }
            if (spraySustainer != null && Find.TickManager.TicksGame > this.spraySustainerStartTick + 1000)
            {
                Log.Message("Geyser spray sustainer still playing after 1000 ticks. Force-ending.");
                spraySustainer.End();
                spraySustainer = null;
            }
        }

        //private IntermittentSteamSprayer steamSprayer;

        public Building harvester;

        private Sustainer spraySustainer;

        private int spraySustainerStartTick = -999;
    }
}
