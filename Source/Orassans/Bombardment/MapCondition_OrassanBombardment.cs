using RimWorld;
using Verse;

namespace OrassanBombardment
{
    public class GameCondition_OrassanBombardment : GameCondition
    {
        private static readonly IntRange TicksBetweenStrikes = new IntRange(150, 400);

        private int nextBombardmentStrike;

        public override void GameConditionTick()
        {
            if (Find.TickManager.TicksGame > this.nextBombardmentStrike)
            {
                foreach(Map map in AffectedMaps)
                {
                    if(map.listerBuildings.allBuildingsColonistCombatTargets.Count > 0)
                    {
                        map.weatherManager.eventHandler.AddEvent(new WeatherEvent_OrbitalBombardment(map, Faction.OfPlayer));
                    }
                }
            }

            this.nextBombardmentStrike = Find.TickManager.TicksGame + TicksBetweenStrikes.RandomInRange;
        }

        public override void End()
        {
            foreach (Map map in AffectedMaps)
            {
                map.weatherDecider.DisableRainFor(30000);
            }
            
            base.End();
        }
    }
}