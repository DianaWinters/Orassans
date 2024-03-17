using RimWorld;
using UnityEngine;
using Verse;

namespace Orassans
{
    public class FrostPlant : Plant
    {
        private static readonly FloatRange DyingDamagePerTickBecauseExposedToLight = new FloatRange(0.0001f, 0.001f);

        public override float CurrentDyingDamagePerTick
        {
            get
            {
                float result;
                if (!base.Spawned)
                {
                    result = 0f;
                }
                else
                {
                    float num = 0f;
                    if (this.def.plant.LimitedLifespan && this.ageInt > this.def.plant.LifespanTicks)
                    {
                        num = Mathf.Max(num, 0.005f);
                    }
                    if (!this.def.plant.cavePlant && this.unlitTicks > 450000)
                    {
                        num = Mathf.Max(num, 0.005f);
                    }
                    if (this.DyingBecauseExposedToLight)
                    {
                        float lerpPct = base.Map.glowGrid.GroundGlowAt(base.Position, true);
                        num = Mathf.Max(num, DyingDamagePerTickBecauseExposedToLight.LerpThroughRange(lerpPct));
                    }
                    result = num;
                }
                return result;
            }
        }

        public override float GrowthRate
        {
            get
            {
                float result;
                if (this.Blighted)
                {
                    result = 0f;
                }
                else
                {
                    result = this.GrowthRateFactor_Fertility * this.GrowthRateFactor_TemperatureFrost * this.GrowthRateFactor_Light;
                }

                return result;
            }
        }

        public float GrowthRateFactor_TemperatureFrost
        {
            get
            {
                float num;
                float result;
                if (!GenTemperature.TryGetTemperatureForCell(base.Position, base.Map, out num))
                {
                    result = 1f;
                }
                else if (num > 42f)
                {
                    result = Mathf.InverseLerp(58f, 42f, num);
                }
                else
                {
                    result = 1f;
                }
                return result;
            }
        }
    }
}
