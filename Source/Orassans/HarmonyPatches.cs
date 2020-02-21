using System.Linq;
using RimWorld;
using Verse;
using System;
using System.Reflection;
using Orassans;
using UnityEngine;
using Verse.AI;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Orassan
{
    //[StaticConstructorOnStartup]
    static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            //Harmony Patching
            Harmony harmony = new Harmony("chjees.orassan");

            {
                Type type = typeof(Plant);

                //Property
                PropertyInfo property = AccessTools.Property(type, "GrowthRateFactor_Temperature");
                MethodInfo propertyGet = property.GetGetMethod();

                HarmonyMethod patchMethod = new HarmonyMethod(typeof(HarmonyPatches), nameof(Patch_Plant_GrowthRateFactor_Temperature_get));
                harmony.Patch(propertyGet, patchMethod, null);
            }

            {
                Type type = typeof(Zone_Growing);

                //Method
                MethodInfo method = type.GetMethod("GetInspectString");

                HarmonyMethod patchMethod = new HarmonyMethod(typeof(HarmonyPatches), "Patch_Zone_Growing_GetInspectString");
                harmony.Patch(method, patchMethod, null);
            }

            {
                Type type = typeof(PlantUtility);

                //Method
                MethodInfo method = type.GetMethod("GrowthSeasonNow");

                HarmonyMethod patchMethod = new HarmonyMethod(typeof(HarmonyPatches), nameof(Patch_GrowthSeasonNow));
                harmony.Patch(method, patchMethod, null);
            }
        }

        public static bool Patch_GrowthSeasonNow(ref bool __result, ref IntVec3 c, Map map, bool forSowing)
        {
            ThingDef wantedPlantDef = WorkGiver_Grower.CalculateWantedPlantDef(c, map);
            if(wantedPlantDef != null && wantedPlantDef.thingClass == typeof(FrostPlant))
            {
                __result = GrowthSeasonNowFrostPlant(c, map);
                return false;
            }

            return true;
        }

        public static IEnumerable<CodeInstruction> JobOnCellTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            //Log.Message("Injecting at WorkGiver_GrowerSow.JobOnCell");
            
            //Replace all functions calls with our own.
            MethodInfo growthSeason = AccessTools.Method(typeof(PlantUtility), nameof(PlantUtility.GrowthSeasonNow));

            foreach (CodeInstruction instruction in instructions)
            {
                //Replace instructions.
#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
                if (instruction.opcode == OpCodes.Call && instruction.operand == growthSeason)
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
                {
                    instruction.operand = AccessTools.Method(typeof(HarmonyPatches), nameof(FrostPlantSeason));
                    yield return instruction;
                    //yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Injector), nameof(FrostPlantSeason)));
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        public static bool FrostPlantSeason(IntVec3 c, Map map)
        {
            //Log.Message("IsFrostPlant: " + c.ToString() + ", " + map?.ToString());

            ThingDef wantedPlantDef = WorkGiver_Grower.CalculateWantedPlantDef(c, map);

            if (wantedPlantDef != null && wantedPlantDef.thingClass == typeof(FrostPlant))
            {
                bool growthSeason = GrowthSeasonNowFrostPlant(c, map);
                //Log.Message("Is a Frost Plant, growthSeason: " + growthSeason);
                return growthSeason;
            }

            return false;
        }

        public static bool Patch_Zone_Growing_GetInspectString(ref Zone_Growing __instance, ref string __result)
        {
            ThingDef defToGrow = __instance.GetPlantDefToGrow();

            //If not a zone growing frost plants return control back to original method.
            if (defToGrow.thingClass != typeof(FrostPlant))
                return true;

            {
                string text = "";
                if (!__instance.Cells.NullOrEmpty<IntVec3>())
                {
                    IntVec3 c = __instance.Cells.First<IntVec3>();
                    if (c.UsesOutdoorTemperature(__instance.Map))
                    {
                        string text2 = text;
                        text = string.Concat(new string[]
                        {
                        text2,
                        "OutdoorGrowingPeriod".Translate(),
                        ": ",
                        Zone_Growing.GrowingQuadrumsDescription(__instance.Map.Tile),
                        " (" + "OrassansColdImmunePlants".Translate() + ")\n"
                        });
                    }
                    if (GrowthSeasonNowFrostPlant(c, __instance.Map))
                    {
                        text += "GrowSeasonHereNow".Translate();
                    }
                    else
                    {
                        text += "CannotGrowBadSeasonTemperature".Translate();
                    }

                    __result = text;
                }
            }

            //Override
            return false;
        }

        public static bool GrowthSeasonNowFrostPlant(IntVec3 c, Map map)
        {
            Room roomOrAdjacent = c.GetRoomOrAdjacent(map, RegionType.Set_All);
            bool result;
            if (roomOrAdjacent == null)
            {
                result = false;
            }
            else
            {
                float temperature = c.GetTemperature(map);
                result = (temperature < 58f);
            }

            return result;
        }

        public static bool Patch_GenPlant_GrowthSeasonNow(ref bool __result, ref IntVec3 c, ref Map map)
        {
            //Get plant.
            Thing plantThing = map.thingGrid.ThingAt(c, ThingCategory.Plant);
            //ThingDef wantedPlantDef = WorkGiver_Grower.CalculateWantedPlantDef(c, map);

            //If not plant is found return to default method.
            //if (plantThing == null || (wantedPlantDef != null && wantedPlantDef.thingClass != typeof(FrostPlant)))
            //    return true;
            if (plantThing == null)
                return true;

            //Original method.
                if (plantThing.GetType() != typeof(FrostPlant))
                return true;

            {
                __result = GrowthSeasonNowFrostPlant(c, map);
            }

            //Override
            return false;
        }

        public static bool Patch_Plant_GrowthRateFactor_Temperature_get(ref Plant __instance, ref float __result)
        {
            //Original method.
            if (__instance.GetType() != typeof(FrostPlant))
                return true;

            //Injection
            {
                float num;
                float result;
                if (!GenTemperature.TryGetTemperatureForCell(__instance.Position, __instance.Map, out num))
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
                __result = result;
            }

            //Override
            return false;
        }
    }
}
