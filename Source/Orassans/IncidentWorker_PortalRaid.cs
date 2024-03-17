using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Orassan
{
    public class IncidentWorker_PortalRaid : IncidentWorker_RaidEnemy
    {
        protected override bool FactionCanBeGroupSource(Faction f, Map map, bool desperate = false) =>
            base.FactionCanBeGroupSource(f, map, desperate) && f.def == OrassanDefOf.Orassan;

        /*protected override bool CanFireNowSub(IIncidentTarget target) => 
            base.CanFireNowSub(target) && Find.FactionManager.AllFactionsVisible.Any((Faction x) => x.def.defName.Equals("Orassan") && x.HostileTo(Faction.OfPlayer));*/

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return base.CanFireNowSub(parms) && Find.FactionManager.AllFactionsVisible.Any((Faction x) => x.def.defName.Equals("Orassan") && x.HostileTo(Faction.OfPlayer));
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            this.ResolveRaidPoints(parms);
            if (!this.TryResolveRaidFaction(parms))
            {
                return false;
            }
            this.ResolveRaidStrategy(parms, PawnGroupKindDefOf.Combat);
            parms.raidArrivalMode = PawnsArrivalModeDefOf.CenterDrop;
            this.ResolveRaidPoints(parms);

            //IncidentParmsUtility.AdjustPointsForGroupArrivalParams(parms);
            PawnGroupMakerParms defaultPawnGroupMakerParms = IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Combat, parms);
            List<Pawn> list = PawnGroupMakerUtility.GeneratePawns(defaultPawnGroupMakerParms, true).ToList<Pawn>();
            if (list.Count == 0)
            {
                Log.Error("Got no pawns spawning raid from parms " + parms);
                return false;
            }
            TargetInfo target = new TargetInfo(parms.spawnCenter, map, false);

            Thing portal = GenSpawn.Spawn(new ThingDef(), target.Cell, map);
            FleckMaker.ThrowHeatGlow(target.Cell, map, 5f);
            FleckMaker.ThrowHeatGlow(target.Cell, map, 5f);
            FleckMaker.ThrowFireGlow(target.Cell.ToVector3(), map, 5f);
            FleckMaker.ThrowFireGlow(target.Cell.ToVector3(), map, 5f);
            FleckMaker.ThrowMetaPuffs(target);
			FleckMaker.ThrowMetaPuffs(target);

			foreach (Pawn current in list)
            {
                GenSpawn.Spawn(current, target.Cell, map);
                target = current;
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Points = " + parms.points.ToString("F0"));
            foreach (Pawn current2 in list)
            {
                string str = (current2.equipment == null || current2.equipment.Primary == null) ? "unarmed" : current2.equipment.Primary.LabelCap;
                stringBuilder.AppendLine(current2.KindLabel + " - " + str);
            }
            TaggedString letterLabel = this.GetLetterLabel(parms);
            TaggedString letterText = this.GetLetterText(parms, list);
            PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter(list, ref letterLabel, ref letterText, this.GetRelatedPawnsInfoLetterText(parms), true);
            //Find.LetterStack.ReceiveLetter(letterLabel, letterText, LetterDefOf.BadUrgent, target, stringBuilder.ToString());
            //if (this.GetLetterType() == LetterType.BadUrgent)
            /*{
                TaleRecorder.RecordTale(TaleDefOf., new object[0]);
            }*/

            parms.raidStrategy.Worker.MakeLords(parms, list);

            //Lord lord = LordMaker.MakeNewLord(parms.faction, parms.raidStrategy.Worker.MakeLordJob(parms, map), map, list);
            //AvoidGridMaker.RegenerateAvoidGridsFor(parms.faction, map);
            LessonAutoActivator.TeachOpportunity(ConceptDefOf.EquippingWeapons, OpportunityType.Critical);
            if (!PlayerKnowledgeDatabase.IsComplete(ConceptDefOf.ShieldBelts))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    Pawn pawn = list[i];
                    if (pawn.apparel.WornApparel.Any(ap => ap.TryGetComp<CompShield>() != null))
                    {
                        LessonAutoActivator.TeachOpportunity(ConceptDefOf.ShieldBelts, OpportunityType.Critical);
                        break;
                    }
                }
            }

            /*if (DebugViewSettings.drawStealDebug && parms.faction.HostileTo(Faction.OfPlayer))
            {
                Log.Message(string.Concat(new object[]
                {
                    "Market value threshold to start stealing: ",
                    StealAIUtility.StartStealingMarketValueThreshold(lord),
                    " (colony wealth = ",
                    map.wealthWatcher.WealthTotal,
                    ")"
                }));
            }*/



            Find.TickManager.slower.SignalForceNormalSpeedShort();
            Find.StoryWatcher.statsRecord.numRaidsEnemy++;
            return true;
        }
    }
}
