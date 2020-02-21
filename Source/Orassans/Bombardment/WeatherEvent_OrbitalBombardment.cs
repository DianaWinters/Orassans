using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace OrassanBombardment
{
    public class WeatherEvent_OrbitalBombardment : WeatherEvent_LightningFlash
    {
        Faction faction;
        public WeatherEvent_OrbitalBombardment(Map map, Faction faction) : base(map) => this.faction = faction;

        public override void FireEvent()
        {
            base.FireEvent();
            IntVec3 strikeLoc = IntVec3.Invalid;

            if (this.faction == null || this.faction.def.defName.EqualsIgnoreCase("Orassans") || this.map.mapPawns.SpawnedPawnsInFaction(this.faction).Count <= 0)
                return;

            //Bombard hostile buildings.
            if(map.listerBuildings.allBuildingsColonistCombatTargets.Count > 0)
            {
                while (!CellFinder.TryFindRandomCellNear(map.listerBuildings.allBuildingsColonistCombatTargets.RandomElement().Position, this.map, 3,
                ((IntVec3 sq) => sq.Standable(map)), out strikeLoc)) { }

                GenExplosion.DoExplosion(strikeLoc, this.map, this.map.roofGrid.RoofAt(strikeLoc) == RoofDefOf.RoofRockThick ? 5f : 10f, DamageDefOf.Bomb, null, 
                    damAmount: 800,
                    explosionSound: SoundDefOf.PlanetkillerImpact);

                this.map.roofGrid.SetRoof(strikeLoc, null);
                Vector3 loc = strikeLoc.ToVector3Shifted();
                MoteMaker.ThrowSmoke(loc, this.map, 1.5f);
                MoteMaker.ThrowMicroSparks(loc, this.map);
                MoteMaker.ThrowLightningGlow(loc, this.map, 1.5f);
                MoteMaker.ThrowDustPuff(loc, this.map, 1.5f);
                MoteMaker.ThrowFireGlow(strikeLoc, this.map, 1.5f);
                MoteMaker.ThrowHeatGlow(strikeLoc, this.map, 1.5f);
            }
        }
    }
}