# Changelog

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
