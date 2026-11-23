using System;
using UnityEngine;

namespace NhxIdolProgression
{
    // The Forge of Potential's idols are the prefabs Upgrader<tier><kind>: tier 0 (Wooden) to 7 (Bloodgold), and kind
    // Weapon (battle idols) or Armor (protection idols)
    internal static class Idols
    {
        internal const int FinalTier = 7;
        internal const int LevelsPerTier = 4;

        // Bloodgold idols are only asked for upgrades from this level on (to level 11 and up), unless the item's recipe
        // asks for them itself; before it, the Flametal idol stays. The user's (2026-11-23), so that Mistlands gear
        // reaches level 10 with Flametal idols instead of stopping at 9
        internal const int FinalTierFromLevel = 10;
        internal static readonly string[] Kinds = { "Weapon", "Armor" };

        private const string Prefix = "Upgrader";

        internal static string PrefabName(int tier, string kind)
        {
            return $"{Prefix}{tier}{kind}";
        }

        internal static bool TryParse(string prefabName, out int tier, out string kind)
        {
            tier = -1;
            kind = null;

            if (prefabName == null || prefabName.Length < Prefix.Length + 2 || !prefabName.StartsWith(Prefix, StringComparison.Ordinal))
            {
                return false;
            }

            int digit = prefabName[Prefix.Length] - '0';
            string rest = prefabName.Substring(Prefix.Length + 1);

            if (digit < 0 || digit > FinalTier || Array.IndexOf(Kinds, rest) < 0)
            {
                return false;
            }

            tier = digit;
            kind = rest;
            return true;
        }

        // The idol tier an upgrade to targetQuality takes when the item's recipe asks for a baseTier idol: the next tier
        // every LevelsPerTier levels, counted from level 1 (upgrades from levels 1-4 take the recipe's idol, 5-8 the next
        // tier, ...), up to Bloodgold, which waits for FinalTierFromLevel
        internal static int TierForUpgrade(int baseTier, int targetQuality)
        {
            int currentLevel = Math.Max(1, targetQuality - 1);
            int tier = Math.Min(FinalTier, baseTier + (currentLevel - 1) / LevelsPerTier);

            if (tier == FinalTier && currentLevel < FinalTierFromLevel)
            {
                tier = Math.Max(baseTier, FinalTier - 1);
            }

            return tier;
        }

        // The idol an upgrade to targetQuality takes, or null when recipeIdol isn't one of the game's idols (another
        // mod's), which leaves the recipe's requirement as it is
        internal static ItemDrop IdolForUpgrade(ItemDrop recipeIdol, int targetQuality)
        {
            if (recipeIdol == null || !TryParse(recipeIdol.name, out int baseTier, out string kind))
            {
                return null;
            }

            int tier = TierForUpgrade(baseTier, targetQuality);

            if (tier == baseTier)
            {
                return recipeIdol;
            }

            GameObject prefab = ObjectDB.instance != null ? ObjectDB.instance.GetItemPrefab(PrefabName(tier, kind)) : null;
            return prefab != null ? prefab.GetComponent<ItemDrop>() : null;
        }
    }
}
