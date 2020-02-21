using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace OrassanBombardment
{
    public class IncidentWorker_OrassanBombardment : IncidentWorker
    {
        /*protected override bool CanFireNowSub(IIncidentTarget target) => 
            Find.FactionManager.AllFactionsVisible.Any((Faction x) => x.def.defName.Equals("Orassan") && x.HostileTo(Faction.OfPlayer) &&
                                                                                   !((Map)target).listerBuildings.ColonistsHaveBuilding((Thing thing) => thing.TryGetComp<CompOrassanShield>() != null && thing.TryGetComp<CompPowerTrader>().PowerOn));*/

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return Find.FactionManager.AllFactionsVisible.Any((Faction x) => x.def.defName.Equals("Orassan") && x.HostileTo(Faction.OfPlayer) &&
                                                                                   !((Map)parms.target).listerBuildings.ColonistsHaveBuilding((Thing thing) => thing.TryGetComp<CompOrassanShield>() != null && thing.TryGetComp<CompPowerTrader>().PowerOn)) &&
                                                                                   ((Map)parms.target).listerBuildings.allBuildingsColonistCombatTargets.Count > 0;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            int duration = Mathf.RoundToInt(this.def.durationDays.RandomInRange * 60000f);
            GameCondition_OrassanBombardment mapCondition_OrassanBombardment = (GameCondition_OrassanBombardment)GameConditionMaker.MakeCondition(GameConditionDef.Named("OrassanBombardmentCondition"), duration);
            Find.LetterStack.ReceiveLetter(this.def.letterLabel, this.def.letterText, LetterDefOf.ThreatBig, null);
            ((Map)parms.target).gameConditionManager.RegisterCondition(mapCondition_OrassanBombardment);
            return true;
        }
    }
}