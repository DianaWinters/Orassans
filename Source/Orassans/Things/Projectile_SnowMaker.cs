using System.Collections.Generic;
using Verse;

namespace Orassan
{
    class Projectile_SnowMaker : Projectile
    {
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            GenExplosion.DoExplosion(base.Position, this.Map, this.def.projectile.explosionRadius, this.def.projectile.damageDef, this.launcher, def.projectile.GetDamageAmount(1f),
                -1, null, this.def, this.equipmentDef, null);
            SnowCreation(base.Position, this.def.projectile.explosionRadius, this.Map);
            CellRect cellRect = CellRect.CenteredOn(base.Position, 3);
            cellRect.ClipInsideMap(this.Map);
            for (int i = 0; i < Rand.RangeInclusive(3, 6); i++)
            {
                IntVec3 randomCell = cellRect.RandomCell;
                this.IceExplosion(randomCell, 2.9f, this.Map);
            }

            base.Impact(hitThing);
        }

        protected void IceExplosion(IntVec3 pos, float radius, Map map)
        {
            ThingDef def = this.def;
            GenExplosion.DoExplosion(pos, map, radius, this.def.projectile.damageDef, this.launcher, def.projectile.GetDamageAmount(1f), 
                -1, null, def, this.equipmentDef, null);
        }

        protected void SnowCreation(IntVec3 pos, float radius, Map map)
        {
            float depth = 15f;
            IEnumerable<IntVec3> iceCells = GenRadial.RadialPatternInRadius(radius);
            foreach (IntVec3 cell in iceCells)
            {
                IntVec3 cellReal = cell + pos;

                if (cellReal.InBounds(map))
                    map.snowGrid.AddDepth(cellReal, depth);
            }
            map.snowGrid.AddDepth(pos, depth);
        }
    }
}
