using HarmonyLib;

namespace NhxIdolProgression.Patches
{
    // The three places where the Forge of Potential deals with an upgrade's requirements. Each one swaps in the idol
    // the upgrade takes (see IdolSwap) for its whole duration: the prefixes run first and the finalizers last, so other
    // mods' patches on the same methods see that idol too (AzuCraftyBoxes counting and taking it from nearby containers,
    // ImpactfulSkills reading its odds).
    internal static class IdolProgressionPatches
    {
        private static bool AtUpgrader(Player player)
        {
            CraftingStation station = player != null ? player.GetCurrentCraftingStation() : null;
            return station != null && station.m_upgrader;
        }

        // Whether you can afford an upgrade: for each item of the recipe list, which is greyed out when you can't, for the
        // craft button, and once more when the upgrade is done. qualityLevel is the level the item is upgraded to.
        [HarmonyPatch(typeof(Player), nameof(Player.HaveRequirements), typeof(Recipe), typeof(bool), typeof(int), typeof(int))]
        private static class HaveRequirementsPatch
        {
            [HarmonyPriority(Priority.First)]
            private static void Prefix(Player __instance, Recipe recipe, bool discover, int qualityLevel, out IdolSwap __state)
            {
                __state = !discover && AtUpgrader(__instance) ? IdolSwap.Begin(recipe, qualityLevel) : null;
            }

            [HarmonyPriority(Priority.Last)]
            private static void Finalizer(IdolSwap __state)
            {
                __state?.End();
            }
        }

        // The crafting panel of the selected item: its requirement list, the craft button and the odds ImpactfulSkills shows
        [HarmonyPatch(typeof(InventoryGui), "UpdateRecipe")]
        private static class UpdateRecipePatch
        {
            [HarmonyPriority(Priority.First)]
            private static void Prefix(InventoryGui __instance, Player player, out IdolSwap __state)
            {
                ItemDrop.ItemData item = __instance.m_selectedRecipe.ItemData;
                __state = item != null && AtUpgrader(player) ? IdolSwap.Begin(__instance.m_selectedRecipe.Recipe, item.m_quality + 1) : null;
            }

            [HarmonyPriority(Priority.Last)]
            private static void Finalizer(IdolSwap __state)
            {
                __state?.End();
            }
        }

        // The upgrade itself: the game takes its odds from the idol and then takes the idols from you
        [HarmonyPatch(typeof(InventoryGui), "DoCrafting")]
        private static class DoCraftingPatch
        {
            [HarmonyPriority(Priority.First)]
            private static void Prefix(InventoryGui __instance, Player player, out IdolSwap __state)
            {
                ItemDrop.ItemData item = __instance.m_craftUpgradeItem;
                __state = item != null && AtUpgrader(player) ? IdolSwap.Begin(__instance.m_craftRecipe, item.m_quality + 1) : null;
            }

            [HarmonyPriority(Priority.Last)]
            private static void Finalizer(IdolSwap __state)
            {
                __state?.End();
            }
        }
    }
}
