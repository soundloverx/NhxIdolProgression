using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using System.Linq;

namespace NhxIdolProgression
{
    // Makes every idol craftable at the artisan table. The ingredients started from Reforged Potential's default recipes.
    internal static class IdolRecipes
    {
        // Per tier, the same for its battle (Weapon) and protection (Armor) idol, so players can pick either kind with the
        // same materials. The metals are bars, not ores: FlametalNew is the Flametal bar (Flametal is the old Ancient
        // Metal) and Gold is the Bloodgold bar (GoldOre is Petrified Tissue). FrostCore is the Frostcore.
        private static readonly (string Item, int Amount)[][] TierIngredients =
        {
            new[] { ("Wood", 50), ("GreydwarfEye", 10) },                                  // Wooden
            new[] { ("Bronze", 10), ("SurtlingCore", 1) },                                 // Bronze
            new[] { ("Iron", 10), ("ElderBark", 5), ("SurtlingCore", 2) },                 // Iron
            new[] { ("Silver", 10), ("FreezeGland", 5), ("Obsidian", 5) },                 // Silver
            new[] { ("BlackMetal", 10), ("Needle", 2), ("Tar", 5) },                       // Black Metal
            new[] { ("BlackMarble", 20), ("BugMeat", 10), ("Carapace", 10) },              // Black Marble
            new[] { ("FlametalNew", 10), ("CharcoalResin", 2), ("ProustitePowder", 5) },   // Flametal
            new[] { ("Gold", 10), ("AncientCoin", 10), ("FrostCore", 1) }                  // Bloodgold
        };

        internal static void Register()
        {
            int added = 0;

            for (int tier = 0; tier < TierIngredients.Length; tier++)
            {
                foreach (string kind in Idols.Kinds)
                {
                    string idol = Idols.PrefabName(tier, kind);

                    RecipeConfig recipe = new RecipeConfig
                    {
                        Name = $"Recipe_{Plugin.PluginGuid}_{idol}",
                        Item = idol,
                        CraftingStation = CraftingStations.ArtisanTable,
                        MinStationLevel = 1,
                        Requirements = TierIngredients[tier].Select(ingredient => new RequirementConfig(ingredient.Item, ingredient.Amount)).ToArray()
                    };

                    if (ItemManager.Instance.AddRecipe(new CustomRecipe(recipe)))
                    {
                        added++;
                    }
                }
            }

            Plugin.Log.LogInfo($"Added {added} idol recipes at the artisan table.");
        }
    }
}
