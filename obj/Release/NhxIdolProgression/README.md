# NhxIdolProgression

Private mod for our Valheim server, not published on Thunderstore. It replaces Reforged Potential, keeping only its craftable idols and its idol progression, plus the upgrade announcements.

## Craftable idols

Every idol of the Forge of Potential can be crafted at the artisan table. Battle and protection idols of the same tier share a recipe, so you can pick either with the same materials. The metals are bars, not ores.

| Idols | Ingredients |
|----|----|
| Wooden Battle / Protection Idol | 50 Wood, 10 Greydwarf Eye |
| Bronze Battle / Protection Idol | 10 Bronze, 1 Surtling Core |
| Iron Battle / Protection Idol | 10 Iron, 5 Ancient Bark, 2 Surtling Core |
| Silver Battle / Protection Idol | 10 Silver, 5 Freeze Gland, 5 Obsidian |
| Black Metal Battle / Protection Idol | 10 Black Metal, 2 Needle, 5 Tar |
| Black Marble Battle / Protection Idol | 20 Black Marble, 10 Seeker Meat, 10 Carapace |
| Flametal Battle / Protection Idol | 10 Flametal, 2 Charcoal Resin, 5 Proustite Powder |
| Bloodgold Battle / Protection Idol | 10 Bloodgold, 10 Ancient Coin, 1 Frostcore |

## Idol progression

The idol an upgrade at the Forge of Potential takes moves up one tier every 4 levels of the item, counted from level 1, until it reaches Bloodgold idols. A bronze sword, for example, takes:

| Upgrading from | Idol |
|----|----|
| level 1-4 | Bronze Battle Idol |
| level 5-8 | Iron Battle Idol |
| level 9-12 | Silver Battle Idol |
| level 13-16 | Black Metal Battle Idol |
| level 17-20 | Black Marble Battle Idol |
| level 21-24 | Flametal Battle Idol |
| level 25 and up | Bloodgold Battle Idol |

Weapons take battle idols and armor takes protection idols, as their recipes ask. Each upgrade takes as many idols as the item's recipe asks for, usually 1. Items from other mods follow the same rules when their recipe asks for one of the game's idols.

The Forge of Potential's item list, the requirements and the craft button all use the idol each item needs, so an item is only greyed out when you really can't afford its next upgrade. With AzuCraftyBoxes, idols in nearby containers count too.

The chance to succeed, lose a level or break the item is the game's, improved by ImpactfulSkills' forging skill.

## Upgrade announcements

Every upgrade at the Forge of Potential is announced in every player's chat: upgraded, lost a level, or destroyed.

## Config

`NhxIdolProgression.cfg`

| Setting | Default | Description |
|----|----|----|
| Announce Upgrades | true | Announce upgrade results in every player's chat. The server's setting applies to every player. |

## Compatibility

- Can't run together with Reforged Potential, which it replaces.
- Must be installed on the server and on every player's game, in the same version.
- Requires Jotunn.
