using System.Collections.Generic;
using System.Linq;
using CustomFactionBase;
using HarmonyLib;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace Orassan
{
    [StaticConstructorOnStartup]
    class SymbolResolver_OrassanBase : SymbolResolver
    {
        static SymbolResolver_OrassanBase() => 
            CustomBaseUtility.AddMapResolver(new ResolverStruct(rp => rp.faction.def == OrassanDefOf.Orassan, "orassanBase", 100));

        const int width = 17;
        const int height = 15;

        ResolveParams rp;
        Map map;

        List<Thing> thingList = new List<Thing>();

        public override void Resolve(ResolveParams rp)
        {
            this.rp = rp;
            this.map = BaseGen.globalSettings.map;

            this.rp.rect.Width = width * 4;
            this.rp.rect.Height = height * 4;


            GenerateBaseHex(new IntVec3(this.rp.rect.CenterCell.x + (width / 2 / 3 + 2 + width/2), this.rp.rect.CenterCell.y, this.rp.rect.CenterCell.z + height / 2));
            GenerateBaseHex(new IntVec3(this.rp.rect.CenterCell.x + (width / 2 / 3 + 2 + width/2), this.rp.rect.CenterCell.y, this.rp.rect.CenterCell.z - height / 2));

            GenerateBaseHex(new IntVec3(this.rp.rect.CenterCell.x - (width / 2 / 3 + 2 + width/2), this.rp.rect.CenterCell.y, this.rp.rect.CenterCell.z + height / 2));
            GenerateBaseHex(new IntVec3(this.rp.rect.CenterCell.x - (width / 2 / 3 + 2 + width/2), this.rp.rect.CenterCell.y, this.rp.rect.CenterCell.z - height / 2));

            GenerateBaseHex(new IntVec3(this.rp.rect.CenterCell.x, this.rp.rect.CenterCell.y, this.rp.rect.CenterCell.z - height + 1));
            GenerateBaseHex(new IntVec3(this.rp.rect.CenterCell.x, this.rp.rect.CenterCell.y, this.rp.rect.CenterCell.z + height - 1));

            GenerateCenterHex();

            LongEventHandler.QueueLongEvent(() =>
            {
                Thing.allowDestroyNonDestroyable = true;
                Traverse terrainInfo = Traverse.Create(this.map.terrainGrid);
                Traverse underGridTraverse = terrainInfo.Field("underGrid");
                TerrainDef[] underGrid = underGridTraverse.GetValue<TerrainDef[]>();
                this.rp.rect.Cells.ToList().ForEach(c =>
                {
                    c.GetThingList(this.map).ForEach(t =>
                    {
                        if (!this.thingList.Contains(t))
                            t.Destroy(DestroyMode.Vanish);
                        underGrid[this.map.cellIndices.CellToIndex(c)] = TerrainDefOf.Soil;
                        this.map.terrainGrid.SetTerrain(c, OrassanDefOf.TileGranite);
                    });
                });
                underGridTraverse.SetValue(underGrid);
                Thing.allowDestroyNonDestroyable = false;
            }, "clean up", false, null);
        }

        private void GenerateCenterHex()
        {
            IntVec3 center = this.rp.rect.CenterCell;
            GenerateBaseHex(center);


            int x = center.x - width/2;
            int z = center.z;

            GenSpawn.Spawn(GenerateThing(ThingDefOf.OrbitalTradeBeacon, null, this.rp.faction), center, this.map);

            bool xInverted = false;
            bool zInverted = false;

            for (int i = 1; i <= 2; i++)
            {
                zInverted = false;
                for (int j = 1; j <= 2; j++)
                {
                    IntVec3 door = new IntVec3(x + (xInverted ? -1 : 1) * 1, center.y, z + (zInverted ? -1 : 1) * 2);
                    door.GetFirstThing(map, ThingDefOf.Wall).Destroy();
                    GenSpawn.Spawn(GenerateThing(ThingDefOf.Door, ThingDefOf.BlocksGranite, this.rp.faction),
                        door, this.map);

                    GenSpawn.Spawn(GenerateThing(OrassanDefOf.SculptureLarge, null, this.rp.faction),
                        new IntVec3(x + (xInverted ? -1 : 1) * 2, center.y, z + (zInverted ? -1 : 1) * 1), this.map);
                    GenSpawn.Spawn(GenerateThing(OrassanDefOf.SculptureLarge, null, this.rp.faction),
                        new IntVec3(x + (xInverted ? -1 : 1) * 3, center.y, z + (zInverted ? -1 : 1) * 3), this.map);
                    GenSpawn.Spawn(GenerateWorkbench(this.rp.faction),
                        new IntVec3(x + (xInverted ? -1 : 1) * 6, center.y, z + (zInverted ? -1 : 1) * 6), this.map, zInverted ? Rot4.South : Rot4.North);
                    
                    z = center.z;
                    zInverted = true;
                }
                GenSpawn.Spawn(GenerateThing(ThingDefOf.StandingLamp, ThingDefOf.BlocksGranite, this.rp.faction),
                            new IntVec3(x + (xInverted ? -1 : 1) * -1, center.y, center.z), this.map);

                x = center.x+width/2;
                xInverted = true;
            }
        }

        public void GenerateBaseHex(IntVec3 center)
        {
            int minX = center.x - width / 2;
            int minZ = center.z - height / 2;
            int maxX = center.x + width / 2;
            int maxZ = center.z + height / 2;

            GenSpawn.Spawn(GenerateThing(ThingDefOf.Door, ThingDefOf.BlocksGranite, this.rp.faction), new IntVec3(center.x, center.y, maxZ), this.map);
            GenSpawn.Spawn(GenerateThing(ThingDefOf.Door, ThingDefOf.BlocksGranite, this.rp.faction), new IntVec3(center.x, center.y, minZ), this.map);

            for (int i = 1; i <= width/2/3+1; i++)
            {
                GenSpawn.Spawn(GenerateThing(ThingDefOf.Wall, ThingDefOf.BlocksGranite, this.rp.faction), new IntVec3(center.x + i, center.y, maxZ), this.map);
                GenSpawn.Spawn(GenerateThing(ThingDefOf.Wall, ThingDefOf.BlocksGranite, this.rp.faction), new IntVec3(center.x + i, center.y, minZ), this.map);
                GenSpawn.Spawn(GenerateThing(ThingDefOf.Wall, ThingDefOf.BlocksGranite, this.rp.faction), new IntVec3(center.x - i, center.y, maxZ), this.map);
                GenSpawn.Spawn(GenerateThing(ThingDefOf.Wall, ThingDefOf.BlocksGranite, this.rp.faction), new IntVec3(center.x - i, center.y, minZ), this.map);
            }

            GenSpawn.Spawn(GenerateThing(ThingDefOf.Wall, ThingDefOf.BlocksGranite, this.rp.faction), new IntVec3(minX, center.y, center.z), this.map);
            GenSpawn.Spawn(GenerateThing(ThingDefOf.Wall, ThingDefOf.BlocksGranite, this.rp.faction), new IntVec3(maxX, center.y, center.z), this.map);
            

            int x = minX;
            int z = center.z;
            bool xInverted = false;
            bool zInverted = false;

            for (int i = 1; i <= 2; i++)
            {
                zInverted = false;
                for (int j = 1; j <= 2; j++)
                {
                    int h = 0;
                    int w = 0;

                    for (int counter = 0; h <= height / 2 - 1; counter++)
                    {
                        switch (counter % 5)
                        {
                            case 0:
                            case 2:
                            case 3:
                                h++;
                                break;
                            case 1:
                            case 4:
                                w++;
                                break;
                        }

                        //Log.Message(counter + ": " + (counter % 7).ToString() + "\n" + w + " - " + h);

                        GenSpawn.Spawn(GenerateThing(ThingDefOf.Wall, ThingDefOf.BlocksGranite, this.rp.faction), 
                            new IntVec3(x + (xInverted ? -1 : 1) * w, center.y, z + (zInverted ? -1 : 1) * h), this.map);
                    }
                    z = center.z;
                    zInverted = true;
                }
                x = maxX;
                xInverted = true;
            }
        }

        Thing GenerateWorkbench(Faction faction) => 
            GenerateThing(DefDatabase<ThingDef>.AllDefs.Where(td => td.building != null && !td.AllRecipes.NullOrEmpty() && td.Size.x == 3 && td.Size.z == 1).RandomElement()
                , null, faction);

        Thing GenerateThing(ThingDef def, ThingDef stuff, Faction fac)
        {
            Thing thing = ThingMaker.MakeThing(def, def.MadeFromStuff ? stuff ?? GenStuff.RandomStuffFor(def) : null);
            thing.SetFactionDirect(fac);
            this.thingList.Add(thing);
            return thing;
        }
    }
}