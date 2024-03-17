using UnityEngine;
using Verse;

namespace BetharianPower
{
    /// <summary>
    /// Helper class for generating glows.
    /// </summary>
    public static class GlowUtility
    {
        public static void MakeGlowAt(Map map, IntVec3 position, float size = 1f)
        {
            //0.666 ... 0.833f
            Color newColor = Color.HSVToRGB(Rand.Range(0.666f, 0.833f), 0.375f, 0.375f);

            ThrowBetharianGlow(position, map, size, newColor);
        }

        public static void ThrowBetharianGlow(IntVec3 c, Map map, float size, Color instanceColor)
        {
            Vector3 vector = c.ToVector3Shifted();
            if (!vector.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
            {
                return;
            }
            vector += size * new Vector3(Rand.Value - 0.5f, 0f, Rand.Value - 0.5f);
            if (!vector.InBounds(map))
            {
                return;
            }
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(BetharianDefOf.Mote_BetharianGlow, null);
            moteThrown.Scale = Rand.Range(4f, 6f) * size;
            moteThrown.rotationRate = Rand.Range(-3f, 3f);
            moteThrown.exactPosition = vector;
            moteThrown.SetVelocity((float)Rand.Range(0, 360), 0.03f);
            moteThrown.instanceColor = instanceColor;

            GenSpawn.Spawn(moteThrown, vector.ToIntVec3(), map);
        }
    }
}
