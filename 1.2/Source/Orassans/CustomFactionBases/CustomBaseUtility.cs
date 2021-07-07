using RimWorld;
using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using Verse;

namespace CustomFactionBase
{
    //[StaticConstructorOnStartup]
    public static partial class CustomBaseUtility
    {
        internal static List<ResolverStruct> resolvers = new List<ResolverStruct>();

        //static CustomBaseUtility() => new Harmony("rimworld.erdelf.customBaseUtility").
        //    Patch(AccessTools.Method(typeof(SymbolResolver_Settlement), nameof(SymbolResolver_Settlement.Resolve)), new HarmonyMethod(typeof(CustomBaseUtility), nameof(FactionBasePrefix)), null);

        static bool FactionBasePrefix(ref ResolveParams rp)
        {            
            rp.faction = rp.faction ?? Find.FactionManager.RandomEnemyFaction(false, false, true);

            foreach(ResolverStruct rs in resolvers)
            {
                if(rs.Active(rp))
                {
                    BaseGen.symbolStack.Push(rs.RuleDefName, rp);
                    return false;
                }
            }
            
            return true;
        }

        public static void AddMapResolver(ResolverStruct struc) => resolvers.Add(struc);

        public static void ReplaceFloor(ResolveParams rp, TerrainDef floor)
        {
            Map map = BaseGen.globalSettings.map;
            LongEventHandler.QueueLongEvent(delegate
            {
                TerrainGrid terrainGrid = map.terrainGrid;
                TerrainDef newTerr = floor;
                //CellRect.CellRectIterator iterator = rp.rect.GetIterator();
                foreach (var item in rp.rect)
                {
                    terrainGrid.SetTerrain(item, newTerr);
                }
                /*while (!iterator.Done())
                {
                    terrainGrid.SetTerrain(iterator.Current, newTerr);
                    iterator.MoveNext();
                }*/
            }, "floor" + DateTime.Now.GetHashCode(), false, null);
        }

        public static void ReplaceEdgeWalls(ResolveParams rp, ThingDef wall, ThingDef door = null)
        {
            Map map = BaseGen.globalSettings.map;
            rp.rect = rp.rect.ExpandedBy(1);
            LongEventHandler.QueueLongEvent(delegate
            {
                foreach (IntVec3 current in rp.rect.EdgeCells)
                {
                    //Thing regBar = ThingMaker.MakeThing(ThingDefOf.TemporaryRegionBarrier);
                    //GenSpawn.Spawn(regBar, current, map);
                    if (current.GetFirstThing(map, ThingDefOf.Wall) != null)
                    {
                        GenSpawn.Spawn(ThingMaker.MakeThing(ThingDefOf.Wall, wall), current, map);
                    }
                    else if (current.GetFirstThing(map, ThingDefOf.Door) != null)
                    {
                        GenSpawn.Spawn(ThingMaker.MakeThing(ThingDefOf.Door, door ?? wall), current, map);
                    }
                    //regBar.Destroy(DestroyMode.Vanish);
                }
            }, "wall" + DateTime.Now.GetHashCode(), false, null);
        }
    }
}
