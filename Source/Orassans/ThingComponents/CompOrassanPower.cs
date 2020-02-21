using RimWorld;
using System;
using System.Linq;
using Verse;

namespace Orassan
{
    class CompOrassanPower : CompPowerTrader
    {
        Faction faction;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.PowerOn = true;
            this.faction = this.parent.Faction.def.defName.EqualsIgnoreCase("OrassanPlayerColony") ? this.parent.Faction : Find.FactionManager.AllFactionsListForReading.FirstOrDefault((Faction x) => x.def.defName.Contains("Orassan"));
        }

        public override void CompTick()
        {
            base.CompTick();
            if(this.faction == null)
                this.faction = Find.FactionManager.AllFactionsListForReading.FirstOrDefault((Faction x) => x.def.defName.Contains("Orassan"));
            if (!this.parent.IsBrokenDown() && this.faction != null)
                this.PowerOutput = Math.Max(0, (this.faction.IsPlayer ? 100f : this.faction.PlayerGoodwill) *100);
            else
                this.PowerOutput = 0;
        }
    }
}