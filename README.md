# NhxIdolProgression

Private mod for our Valheim server, not published on Thunderstore. It replaces Reforged Potential, keeping only its craftable idols and its idol progression, plus the upgrade announcements. Upgrades at the Forge of Potential no longer break items.

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
| Flametal Battle / Protection Idol | 10 Flametal, 10 Grausten, 2 Proustite Powder |
| Bloodgold Battle / Protection Idol | 10 Bloodgold, 10 Ancient Coin, 1 Frostcore |

## Idol progression

The idol an upgrade at the Forge of Potential takes moves up one tier after every 5 levels of the item, until it reaches Bloodgold idols: the upgrades to levels 2-5 take the idol the item's recipe asks for, the upgrades to levels 6-10 the next tier, and so on. A bronze sword, for example, takes:

| Upgrading to | Idol |
|----|----|
| level 2-5 | Bronze Battle Idol |
| level 6-10 | Iron Battle Idol |
| level 11-15 | Silver Battle Idol |
| level 16-20 | Black Metal Battle Idol |
| level 21-25 | Black Marble Battle Idol |
| level 26-30 | Flametal Battle Idol |
| level 31 and up | Bloodgold Battle Idol |

Bloodgold idols also wait for the upgrade to level 11, unless the item's own recipe asks for Bloodgold idols: until then, the Flametal idol stays. This only changes anything for Ashlands items (Flametal idols), which would otherwise move to Bloodgold idols with the upgrade to level 6. Mistlands items (Black Marble idols) and Ashlands items take:

| Upgrading to | Mistlands item | Ashlands item |
|----|----|----|
| level 2-5 | Black Marble Idol | Flametal Idol |
| level 6-10 | Flametal Idol | Flametal Idol |
| level 11 and up | Bloodgold Idol | Bloodgold Idol |

Weapons take battle idols and armor takes protection idols, as their recipes ask. Each upgrade takes as many idols as the item's recipe asks for, usually 1. Items from other mods follow the same rules when their recipe asks for one of the game's idols.

The Forge of Potential's item list, the requirements and the craft button all use the idol each item needs, so an item is only greyed out when you really can't afford its next upgrade. With AzuCraftyBoxes, idols in nearby containers count too.

## No broken items

Upgrades at the Forge of Potential never break the item: every idol's chance to break it is 0. An upgrade that doesn't succeed loses a level instead, but never takes the item below level 1, so a level 1 item that fails stays at level 1. The chance to succeed is the game's, improved by ImpactfulSkills' forging skill, and ImpactfulSkills' odds in the Forge's panel show 0% to break.

The message in the center of the screen after a failed upgrade shows its text again ("Refining of ... failed, downgraded to level ..."): the game showed [msg_upgrader_failed] instead.

## Upgrade announcements

Every upgrade at the Forge of Potential is announced in every player's chat: upgraded, or failed and lost a level.

## Config

`NhxIdolProgression.cfg`

| Setting | Default | Description |
|----|----|----|
| Announce Upgrades | true | Announce upgrade results in every player's chat. The server's setting applies to every player. |

## Compatibility

- Can't run together with Reforged Potential, which it replaces.
- Must be installed on the server and on every player's game, in the same version.
- Requires Jotunn.
