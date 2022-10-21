// Orassan.HarmonyPatches
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Orassans;
using RimWorld;
using UnityEngine;
using Verse;

[StaticConstructorOnStartup]
internal static class HarmonyPatches
{
	static HarmonyPatches()
	{
		Harmony harmony = new Harmony("chjees.orassan");
		Type typeFromHandle = typeof(Plant);
		PropertyInfo propertyInfo = AccessTools.Property(typeFromHandle, "GrowthRateFactor_Temperature");
		MethodInfo getMethod = propertyInfo.GetGetMethod();
		HarmonyMethod prefix = new HarmonyMethod(typeof(HarmonyPatches), nameof(Patch_Plant_GrowthRateFactor_Temperature_get));
		harmony.Patch(getMethod, prefix);
		Type typeFromHandle2 = typeof(Zone_Growing);
		MethodInfo method = typeFromHandle2.GetMethod("GetInspectString");
		HarmonyMethod prefix2 = new HarmonyMethod(typeof(HarmonyPatches), nameof(Patch_Zone_Growing_GetInspectString));
		harmony.Patch(method, prefix2);
		Type typeFromHandle3 = typeof(PlantUtility);
		MethodInfo method2 = typeFromHandle3.GetMethod("GrowthSeasonNow");
		HarmonyMethod prefix3 = new HarmonyMethod(typeof(HarmonyPatches), nameof(Patch_GrowthSeasonNow));
		harmony.Patch(method2, prefix3);
	}

	public static bool Patch_GrowthSeasonNow(ref bool __result, ref IntVec3 c, Map map, bool forSowing)
	{
		ThingDef val = WorkGiver_Grower.CalculateWantedPlantDef(c, map);
		if (val != null && val.thingClass == typeof(FrostPlant))
		{
			__result = GrowthSeasonNowFrostPlant(c, map);
			return false;
		}
		return true;
	}

	public static IEnumerable<CodeInstruction> JobOnCellTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
	{
		MethodInfo growthSeason = AccessTools.Method(typeof(PlantUtility), "GrowthSeasonNow");
		foreach (CodeInstruction instruction in instructions)
		{
            if (instruction.Calls(growthSeason))
                instruction.operand = AccessTools.Method(typeof(HarmonyPatches), "FrostPlantSeason");
            yield return instruction;
        }
	}

	public static bool FrostPlantSeason(IntVec3 c, Map map)
	{
		ThingDef val = WorkGiver_Grower.CalculateWantedPlantDef(c, map);
		if (val != null && val.thingClass == typeof(FrostPlant))
		{
			return GrowthSeasonNowFrostPlant(c, map);
		}
		return false;
	}

	public static bool Patch_Zone_Growing_GetInspectString(ref Zone_Growing __instance, ref string __result)
	{
		ThingDef plantDefToGrow = __instance.GetPlantDefToGrow();
		if (plantDefToGrow.thingClass != typeof(FrostPlant))
		{
			return true;
		}
		string text = "";
		if (!__instance.cells.NullOrEmpty())
		{
			IntVec3 val = __instance.cells.First();
			if (GridsUtility.UsesOutdoorTemperature(val, __instance.Map))
			{
				string text2 = text;
				text = text2 + Translator.Translate("OutdoorGrowingPeriod") +": " + Zone_Growing.GrowingQuadrumsDescription(__instance.Map.Tile) + " (" + Translator.Translate("OrassansColdImmunePlants") + ")\n";
			}
			text = (__result = (!GrowthSeasonNowFrostPlant(val, __instance.Map) ? text + Translator.Translate("CannotGrowBadSeasonTemperature") : text + Translator.Translate("GrowSeasonHereNow")));
		}
		return false;
	}

	public static bool GrowthSeasonNowFrostPlant(IntVec3 c, Map map)
	{
		Room roomOrAdjacent = GridsUtility.GetRoomOrAdjacent(c, map, (RegionType)7);
		if (roomOrAdjacent == null)
		{
			return false;
		}
		float temperature = GridsUtility.GetTemperature(c, map);
		return temperature < 58f;
	}

	public static bool Patch_GenPlant_GrowthSeasonNow(ref bool __result, ref IntVec3 c, ref Map map)
	{
		Thing val = map.thingGrid.ThingAt(c, (ThingCategory)4);
		if (val == null)
		{
			return true;
		}
		if (((object)val).GetType() != typeof(FrostPlant))
		{
			return true;
		}
		__result = GrowthSeasonNowFrostPlant(c, map);
		return false;
	}

	public static bool Patch_Plant_GrowthRateFactor_Temperature_get(ref Plant __instance, ref float __result)
	{
		if (__instance is FrostPlant)
		{
			float num = default(float);
			float num2 = __result = (!GenTemperature.TryGetTemperatureForCell(__instance.Position, __instance.Map, out num)) ? 1f : ((!(num > 42f)) ? 1f : Mathf.InverseLerp(58f, 42f, num));
			return false;
		}
		return true;
	}
}