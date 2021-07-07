using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace OrassanBombardment
{
    class Building_BombardmentBeacon : Building
    {
        bool firing = false;
        float progress;
        Effecter bar;

        public override void PostMake()
        {
            base.PostMake();
            this.progress = 0f;
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            List<Gizmo> gizmos = base.GetGizmos().ToList();

            if (!this.firing && !this.Map.IsPlayerHome && this.Map.info.parent is Settlement && 
                !Find.FactionManager.AllFactionsVisible.ToList().Any((Faction x) => x.def.defName.Equals("Orassan") && x.HostileTo(Faction.OfPlayer)))
            {
                Command_Action fire = new Command_Action()
                {
                    action = delegate { this.firing = true; }
                };
                gizmos.Add(fire);
            }
            return gizmos;
        }

        public override void Tick()
        {
            base.Tick();
            if (this.firing)
            {
                if (this.bar == null)
                    this.bar = EffecterDefOf.ProgressBar.Spawn();
                this.bar.EffectTick(this, TargetInfo.Invalid);
                ((SubEffecter_ProgressBar)this.bar.children[0]).mote.progress = Mathf.Clamp01(this.progress);
                ((SubEffecter_ProgressBar)this.bar.children[0]).mote.offsetZ = -0.5f;
                if (GenTicks.TicksAbs % 50 == 0)
                    if (Rand.Bool)
                    {
                        this.Map.weatherManager.eventHandler.AddEvent(new WeatherEvent_OrbitalBombardment(this.Map, this.Map.ParentFaction));
                    }
                this.progress += 0.001f;
            }
            if (this.progress >= 1)
            {
                IntVec3 pos = this.Position;
                Map map = this.Map;
                this.bar.Cleanup();
                this.Destroy(DestroyMode.Vanish);
                GenExplosion.DoExplosion(pos, map, 5f, DamageDefOf.Bomb, null);
            }
        }

        

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.firing, "firingOrassanBomb", false);
            Scribe_Values.Look(ref this.progress, "progressOrassanBomb", 0f);
        }
    }
}
