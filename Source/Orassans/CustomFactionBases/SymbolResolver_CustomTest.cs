using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace CustomFactionBase
{
    /*
    [StaticConstructorOnStartup]
    class SymbolResolver_CustomTest : SymbolResolver
    {
        
        static SymbolResolver_CustomTest() => 
            CustomBaseUtility.AddMapResolver(new ResolverStruct(rp => true, "customTest", 100));

        public override void Resolve(ResolveParams rp)
        {
            Map map = BaseGen.globalSettings.map;
            foreach(IntVec3 c in rp.rect.Cells)
                GenSpawn.Spawn(ThingDefOf.Wall, c, map);
        }
        
    }
    */
}
