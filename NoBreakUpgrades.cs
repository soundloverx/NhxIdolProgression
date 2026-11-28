using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace NhxIdolProgression
{
    // Upgrades at the Forge of Potential never destroy the item: every idol's break chance is 0, so an upgrade that
    // doesn't succeed always loses a level instead, and never takes the item below level 1. The user's (2026-11-28)
    internal static class NoBreakUpgrades
    {
        // The largest float below 1
        private const float HighestRoll = 0.99999994f;

        // The prefab name of the item being upgraded at the Forge of Potential, while DoCrafting runs
        private static string s_upgradingPrefab;

        // The game reads an upgrade's odds from the idol prefab's shared data (the idol the upgrade takes, see IdolSwap),
        // and so does ImpactfulSkills, which shows them in the Forge's panel and changes them with its forging bonus. With
        // a break chance of 0, its odds have none either, and the rest of the failures lose a level. Jotunn calls this
        // each time the game fills its item list, on joining a world; the prefabs keep the change for the whole session.
        internal static void RemoveIdolBreakChance()
        {
            if (ObjectDB.instance == null)
            {
                return;
            }

            SortedSet<float> previous = new SortedSet<float>();
            int changed = 0;

            for (int tier = 0; tier <= Idols.FinalTier; tier++)
            {
                foreach (string kind in Idols.Kinds)
                {
                    string prefabName = Idols.PrefabName(tier, kind);
                    GameObject prefab = ObjectDB.instance.GetItemPrefab(prefabName);
                    ItemDrop idol = prefab != null ? prefab.GetComponent<ItemDrop>() : null;

                    if (idol == null)
                    {
                        Plugin.Log.LogWarning($"Idol {prefabName} not found, so its break chance couldn't be set to 0.");
                        continue;
                    }

                    ItemDrop.ItemData.SharedData shared = idol.m_itemData.m_shared;

                    if (shared.m_breakChance != 0f)
                    {
                        previous.Add(shared.m_breakChance);
                        shared.m_breakChance = 0f;
                        changed++;
                    }
                }
            }

            if (changed > 0)
            {
                string was = string.Join(", ", previous.Select(chance => chance.ToString(CultureInfo.InvariantCulture)));
                Plugin.Log.LogInfo($"Set the break chance of {changed} idols to 0 (it was {was}).");
            }
        }

        // The game breaks the item when breakChance >= 1 - roll, and the roll (Random.value) can be exactly 1, which would
        // break it even with a break chance of 0. Keeping the roll below 1 makes that impossible; a roll that high
        // succeeds or fails just the same. ImpactfulSkills' RollRefinementsWhenAttempted replaces the roll with a prefix
        // that skips this method, but postfixes still run after such a prefix. Named by a string: the publicized
        // assemblies predate the method.
        [HarmonyPatch(typeof(Player), "GetItemUpgradeChance")]
        private static class UpgradeRollPatch
        {
            [HarmonyPriority(Priority.Last)]
            private static void Postfix(ref float __result)
            {
                if (__result > HighestRoll)
                {
                    __result = HighestRoll;
                }
            }
        }

        [HarmonyPatch(typeof(InventoryGui), "DoCrafting")]
        private static class WatchUpgrade
        {
            [HarmonyPriority(Priority.First)]
            private static void Prefix(InventoryGui __instance, Player player)
            {
                CraftingStation station = player != null ? player.GetCurrentCraftingStation() : null;
                Recipe recipe = __instance.m_craftRecipe;

                s_upgradingPrefab = __instance.m_craftUpgradeItem != null && recipe != null && recipe.m_item != null && station != null && station.m_upgrader
                    ? recipe.m_item.gameObject.name
                    : null;
            }

            [HarmonyPriority(Priority.Last)]
            private static void Finalizer()
            {
                s_upgradingPrefab = null;
            }
        }

        // A failed upgrade gives the item back at its level minus 1, through this overload: level 0 for a level 1 item.
        // ImpactfulSkills raises it to 1 too, but only with its forging skill on.
        [HarmonyPatch(typeof(Inventory), nameof(Inventory.AddItem), typeof(string), typeof(int), typeof(int), typeof(int), typeof(long), typeof(string), typeof(Vector2i), typeof(bool), typeof(bool), typeof(bool))]
        private static class LevelFloorPatch
        {
            private static void Prefix(string name, ref int quality)
            {
                if (quality < 1 && s_upgradingPrefab != null && name == s_upgradingPrefab)
                {
                    quality = 1;
                }
            }
        }

        // The failure message in the center of the screen. The game asks for the text "$msg_upgrader_failed", which
        // doesn't exist (the text is "msg_upgrader_fail": "Refining of $1 failed, downgraded to level $2"), so it showed
        // "[msg_upgrader_failed]". It also gives the item's level minus 1 as the new level, which LevelFloorPatch keeps
        // at 1 or more.
        [HarmonyPatch(typeof(InventoryGui), "DoCrafting")]
        private static class FailureMessagePatch
        {
            private const string MissingKey = "$msg_upgrader_failed";
            private const string Key = "$msg_upgrader_fail";

            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> code = instructions.ToList();
                MethodInfo localize = AccessTools.Method(typeof(Localization), nameof(Localization.Localize), new[] { typeof(string), typeof(string[]) });

                int keyIndex = code.FindIndex(instruction => instruction.opcode == OpCodes.Ldstr && (instruction.operand as string == MissingKey || instruction.operand as string == Key));
                int localizeIndex = keyIndex >= 0 ? code.FindIndex(keyIndex, instruction => instruction.Calls(localize)) : -1;

                if (localizeIndex < 0)
                {
                    Plugin.Log.LogWarning("The Forge of Potential's failure message wasn't found in InventoryGui.DoCrafting, so it's left as the game shows it.");
                    return code;
                }

                code[keyIndex].operand = Key;

                // Its level argument, (the item's level + 1) - 2, as the game builds the message's words
                int subIndex = code.FindIndex(keyIndex, localizeIndex - keyIndex, instruction => instruction.opcode == OpCodes.Sub);

                if (subIndex > 0 && code[subIndex - 1].opcode == OpCodes.Ldc_I4_2)
                {
                    code.Insert(subIndex + 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(NoBreakUpgrades), nameof(LevelAfterFailure))));
                }
                else
                {
                    Plugin.Log.LogWarning("The level in the Forge of Potential's failure message wasn't found, so it may show level 0 for a level 1 item.");
                }

                return code;
            }
        }

        private static int LevelAfterFailure(int level)
        {
            return Math.Max(1, level);
        }
    }
}
