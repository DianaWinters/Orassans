using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace Orassans
{
    public class JobGiver_OrassanNuzzle : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.RaceProps.nuzzleMtbHours <= 0f)
            {
                return null;
            }

            List<Pawn> source = pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction);
            if (!(from p in source
                  where p.RaceProps.Humanlike && p.Position.InHorDistOf(pawn.Position, 15f) && pawn.GetRoom() == p.GetRoom() && !p.Position.IsForbidden(pawn) && p.CanCasuallyInteractNow(false) && p != pawn && p.relations.OpinionOf(pawn) > 30
                  select p).TryRandomElement(out Pawn t))
            {
                return null;
            }

            return new Job(JobDefOf.Nuzzle, t)
            {
                locomotionUrgency = LocomotionUrgency.Walk,
                expiryInterval = 3000
            };
        }
    }
}
