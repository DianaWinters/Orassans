using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace OrassanBombardment
{
    class Building_BombardmentBeacon2 : Building
    {
        public Map map;
        public bool check;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.map = map;
            this.check = !map.IsPlayerHome && map.info.parent is Settlement && !Find.FactionManager.AllFactionsVisible.ToList().Any((Faction x) => x.def.defName.Equals("Orassan") && x.HostileTo(Faction.OfPlayer));
            BombardmentOrassanChecker.beacons.Add(this);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref this.map, "OrassanBombardmentReference");
            Scribe_Values.Look(ref this.check, "OrassanBombardmentCheck");
        }
    }

    [StaticConstructorOnStartup]
    class BombardmentOrassanChecker : MonoBehaviour
    {
        public static List<Building_BombardmentBeacon2> beacons = new List<Building_BombardmentBeacon2>();

        static BombardmentOrassanChecker()
        {
            GameObject initializer = new UnityEngine.GameObject("Bombardment2");
            initializer.AddComponent<BombardmentOrassanChecker>();
            UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object)initializer);
        }

        public void FixedUpdate() => beacons.ForEach(delegate (Building_BombardmentBeacon2 beacon)
                                   {
                                       if (beacon.Map == null && beacon.map != null)
                                       {
                                           if (beacon.check)
                                           {
                                               DestroyedSettlement destroyed = (DestroyedSettlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.DestroyedSettlement);
                                               destroyed.Tile = beacon.map.Tile;
                                               Find.WorldObjects.Add(destroyed);
                                               Settlement factionBase = (Settlement)beacon.map.info.parent;
                                               beacon.map.info.parent = destroyed;
                                               Find.WorldObjects.Remove(factionBase);
                                               beacon.map = null;
                                           }
                                       }
                                   });
    }
}