using RimWorld;
using System.Text;
using Verse;

namespace BetharianPower
{
    /// <summary>
    /// Building doing the actual boring of a hole.
    /// </summary>
    public class Building_BetharianBorer : Building
    {
        public int ticksPassed;

        public bool finishedBoring;

        private CompPowerTrader powerTrader;

        public float BoringProgress
        {
            get
            {
                return (float)ticksPassed / (float)GenDate.TicksPerDay;
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            powerTrader = GetComp<CompPowerTrader>();

            if (!respawningAfterLoad)
            {
                ticksPassed = 0;
                finishedBoring = false;
            }
        }

        public override void Tick()
        {
            base.Tick();

            if (!finishedBoring)
            {
                if(ticksPassed >= GenDate.TicksPerDay)
                {
                    finishedBoring = true;
                    FinishBoring();
                }

                if(powerTrader.PowerOn)
                    ticksPassed++;
            }
        }

        public void FinishBoring()
        {
            //Spawn bore hole.
            Thing boreHole = ThingMaker.MakeThing(BetharianDefOf.BetharianBoreHole);
            GenSpawn.Spawn(boreHole, Position, Map);

            //Send letter.
            Letter letter = LetterMaker.MakeLetter("BetharianBorerFinishedLabel".Translate(), "BetharianBorerFinishedText".Translate(), LetterDefOf.PositiveEvent, new TargetInfo(this));
            Find.LetterStack.ReceiveLetter(letter);
        }

        public override string GetInspectString()
        {
            StringBuilder builder = new StringBuilder(base.GetInspectString());
            builder.AppendLine();

            if(finishedBoring)
            {
                builder.AppendLine("BetharianBorerFinished".Translate());
            }
            else
            {
                builder.AppendLine("BetharianBorerDrilling".Translate(BoringProgress.ToStringPercent()));
            }

            return builder.ToString().TrimEndNewlines();
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref ticksPassed, "ticksPassed");

            Scribe_Values.Look(ref finishedBoring, "finishedBoring");
        }
    }
}
