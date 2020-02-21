using RimWorld;
using UnityEngine;
using Verse;

namespace Orassan
{
    class CompFoodOrassan : ThingComp
    {
        private float orassanPrepared;

        public float OrassanPrepared
        {
            get => this.orassanPrepared;
            set => this.orassanPrepared = Mathf.Clamp01(value);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            Thing thing = GenClosest.ClosestThing_Global(this.parent.PositionHeld, this.parent.Map.listerThings.ThingsInGroup(ThingRequestGroup.Pawn), 5);
            this.OrassanPrepared = (thing != null && thing is Pawn && (thing as Pawn).kindDef.race.Equals(OrassanDefOf.Alien_Orassan)) ? 1f : 0f;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref this.orassanPrepared, "orassanPrepared", 0f);
        }

        public override void PostSplitOff(Thing piece)
        {
            base.PostSplitOff(piece);
            CompFoodOrassan compFoodOrassan = piece.TryGetComp<CompFoodOrassan>();
            compFoodOrassan.OrassanPrepared = this.OrassanPrepared;
        }

        public override void PreAbsorbStack(Thing otherStack, int count)
        {
            base.PreAbsorbStack(otherStack, count);
            CompFoodOrassan compFoodOrassan = otherStack.TryGetComp<CompFoodOrassan>();
            this.OrassanPrepared = GenMath.WeightedAverage(this.OrassanPrepared, (float)this.parent.stackCount, compFoodOrassan.OrassanPrepared, (float)count);
        }

        public override void PostIngested(Pawn ingester)
        {
            base.PostIngested(ingester);
            if (ingester.RaceProps.Humanlike && !ingester.def.defName.Contains("Orassan"))
                if (Rand.Value < this.OrassanPrepared && !ingester.kindDef.race.Equals(OrassanDefOf.Alien_Orassan))
                {
                    ingester.needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named("OrassanFoodOnHuman"));

                    /*if (PawnUtility.ShouldSendNotificationAbout(ingester))
                    {
                        Messages.Message(ingester.LabelShort + " ate food prepared by an Orassan.", ingester, MessageSound.Negative);
                    }*/
                }
        }
    }


    class CompProperties_FoodOrassan : CompProperties
    {
        public CompProperties_FoodOrassan() => this.compClass = typeof(CompFoodOrassan);
    }
}