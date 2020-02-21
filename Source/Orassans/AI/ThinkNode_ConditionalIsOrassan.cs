using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace Orassan
{
    public class ThinkNode_ConditionalIsOrassan : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn) => pawn.def == OrassanDefOf.Alien_Orassan;
    }
}