# Changelog

## v1.2.0
- The idol tier now moves up after each upgrade to a multiple of 5: the upgrades to levels 2-5 take the recipe's idol, to 6-10 the next tier, to 11-15 the one after, and so on (2-5, 6-9, 10-13... before). A bronze sword now takes Bloodgold idols from the upgrade to level 31 (26 before). Mistlands and Ashlands items are unchanged: Flametal idols up to level 10, Bloodgold idols from the upgrade to level 11

## v1.1.0
- Bloodgold idols are only asked for from the upgrade to level 11 on; the upgrades before it take Flametal idols instead. Mistlands items now reach level 10 with Flametal idols (Bloodgold from level 9 before), and Ashlands items stay on Flametal idols up to level 10 (Bloodgold from level 5 before). Items whose recipe asks for Bloodgold idols still take them from the start

## v1.0.0
- Idol crafting and idol progression taken from Reforged Potential 2.0.7, which this mod replaces
- Every idol is craftable at the artisan table, with one recipe per tier shared by battle and protection idols. The recipes are no longer configurable. They start from Reforged Potential's defaults, with these changes:
  - Iron idols also take 2 Surtling Cores
  - Flametal idols take 2 Charcoal Resin and 5 Proustite Powder instead of 15 Charred Bone
  - Bloodgold idols no longer take 40 Coins, and take 1 Frostcore instead
- The idol an upgrade at the Forge of Potential takes moves up one tier every 4 levels, counted from level 1, up to Bloodgold idols. The number of idols is what the item's recipe asks for
- The Forge of Potential's list, requirements and craft button use the idol each item really needs, counting idols in nearby containers with AzuCraftyBoxes. Reforged Potential changed the recipes for good, so its list checked every item against the idol of the last item selected and greyed them out
- Chat announcements of every upgrade at the Forge of Potential, with separate messages for upgraded, lost a level and destroyed. On by default, set on the server
- The game's own upgrade code runs again: the odds come from the idol and ImpactfulSkills' forging bonus, and Jewelcrafting keeps sockets and gems on upgraded items. Reforged Potential replaced that code, which lost sockets and gems, skipped ImpactfulSkills' refund cap and didn't check that the item and idols were still there when the upgrade finished
- Not taken from Reforged Potential: its upgrade and break chances, boss progression, level cap, cost scaling, upgrade time, idol refunds, keeping the level on failure and EpicLoot support
