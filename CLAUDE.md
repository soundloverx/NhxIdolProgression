This is a custom mod for Valheim: craftable Forge of Potential idols, idol progression, and chat announcements of upgrade results. It replaces Reforged Potential 2.0.7 by Akuichi on our private server, keeping only those parts of it. It's for personal use with friends, never published, and this folder is not a git repository.

For reference, the game is installed here: c:\Program Files (x86)\Steam\steamapps\common\Valheim\

# Status (as of 2026-10-09)

- v1.0.0 is built but not installed or tested in-game yet.
- The user removed Reforged Potential from HeavyMods on 2026-10-09: its plugin folder is gone. If it ever came back, NhxIdolProgression wouldn't load (BepInEx logs that it's incompatible with akuichi.ReforgedPotential).
- Its config, akuichi.ReforgedPotential.cfg, was deleted on 2026-10-09 at the user's request.

## Where we left off

The project was created in a session started from the NhxExtraJewels folder. That session:
1. Reviewed Reforged Potential's code (see "Reforged Potential, reviewed" below).
2. Settled with the user what to keep (see "Decisions made with the user").
3. Wrote and built v1.0.0.

The project was first going to be called NhxReforgedPotential; the user renamed it before any code was written.

Next steps:
1. **Idol recipes:** done. The user is satisfied with them as of 2026-10-09 (see "Decisions made with the user").
2. **Install.** The user deploys the mod themselves, on the server and on every player's game; don't copy it into HeavyMods. After a test, offer to read HeavyMods\BepInEx\LogOutput.log.
3. **First in-game test,** with the list below.
4. **Tweaks.** The user said they may want to tweak more once it runs.

Verified so far, outside the game:
- The build has no warnings.
- The tier math gives the agreed table. It was checked by calling `Idols.TierForUpgrade` and `Idols.TryParse` through reflection on the built DLL from PowerShell (`[Reflection.Assembly]::LoadFrom(...)`, `GetMethod(name, NonPublic|Static).Invoke(...)`). Pass arguments as `[object[]]@(1, $target)`: with `@(1, $from + 1)` PowerShell builds three arguments.
- All 21 ingredient prefabs exist (checked again after the 2026-10-09 recipe changes), and so do the idol prefabs. Check: extract the prefab names from IdolRecipes.cs and grep the SoftRef manifest for `/<name>.prefab`.
- The README's English item names were checked against the game's text (see "Looking things up in the game's files").
- **Audit on 2026-10-09,** against Valheim 1.0.17 and the HeavyMods versions of ImpactfulSkills, AzuCraftyBoxes and Jewelcrafting. No bugs found.
  - The game was updated on 2026-10-06, after the publicized assemblies were made (2026-09-11). Every game member the DLL references (listed with `ilspycmd -il` on the DLL) exists in the current game with the same signature, and so do the patched methods.
  - The game reads idol requirements in only five places: `HaveRequirementItems` (called only from `HaveRequirements`), `SetupRequirementList`, `DoCrafting`, `ConsumeResources` and `GetFirstRequiredItem`. The last is only for one-ingredient recipes. All the others run inside a swap.
  - ImpactfulSkills keeps the `SharedData` it changed and restores that same object, so the order of the finalizers doesn't matter. Its odds display is a postfix, and postfixes run before any finalizer, so it sees the swapped idol.
  - AzuCraftyBoxes reads the requirements when called, and takes container items synchronously inside `DoCrafting` (its "later consumption" lasts only from its `DoCrafting` prefix to its finalizer).
  - Jewelcrafting's `StopCorruptedItemUpgrade` skips `DoCrafting` for corrupted items. The swap is still undone and nothing is announced.

Things to check during the first in-game test:
- The log shows "Added 16 idol recipes at the artisan table." and "NhxIdolProgression v1.0.0 loaded.", and no errors from NhxIdolProgression.
- The 16 idols can be crafted at the artisan table, each listed once. No game recipe for idols was found, but recipes are inside compressed bundles, so a duplicate would only show here.
- ImpactfulSkills' odds in the Forge's panel come from the idol the upgrade takes, so they may change when an item moves to the next idol tier, if the game gives tiers different odds.
- At the Forge of Potential, a bronze item at level 5 or more asks for iron idols, and a level 4 one for bronze idols (see "Idol progression" below). With `devcommands`: `spawn SwordBronze 1 6`.
- An item isn't greyed out in the Forge's list when the idols it needs are in your inventory, or in a nearby container (AzuCraftyBoxes), and is greyed out when they aren't. Two copies of the same item at different levels each show their own idol.
- The upgrade takes the idols shown, also from containers.
- Jewelcrafting sockets and gems stay on an upgraded item, and on an item that lost a level.
- ImpactfulSkills shows its odds in the Forge's panel, and its log no longer mentions Reforged Potential.
- The announcement shows in every player's chat for each outcome. The server's "Announce Upgrades" turns it off for everyone. With NhxBetterChat it should also get a `[HH:mm:ss]` stamp and the chat sound (see "Other mods").
- When an item breaks, check the refunded materials. With the game's own upgrade code, Jewelcrafting's custom data transfer (see "Other mods") may put the broken item's sockets on the first refunded material. That's Jewelcrafting with the plain game, not this mod, but it's newly reachable now that Reforged Potential no longer replaces the upgrade code.

# Where things are

- **Server mod list:** the Gale profile **HeavyMods**. Its plugins folder is c:\Users\sound\AppData\Roaming\com.kesomannen.gale\valheim\profiles\HeavyMods\BepInEx\plugins\. Our Nhx mods are copied in by hand, each as a folder with the DLL, manifest.json, README.md and CHANGELOG.md.
- **Config:** HeavyMods\BepInEx\config\NhxIdolProgression.cfg.
- **Reforged Potential source:** c:\Users\sound\OneDrive\Projects\Visual Studio\ReforgedPotential\, pulled from its git. The installed 2.0.7 is on `origin/develop` (183aa9d); the local checkout is `master`, which is 2.0.6. Its license is MIT No Attribution. Only its default idol recipes and the idea of idol progression were taken; the code is new.
- **Installed Reforged Potential:** HeavyMods\BepInEx\plugins\Akuichi-ReforgedPotential\.
- **ImpactfulSkills** (MidnightMods, 0.21.1 when this was written): HeavyMods\BepInEx\plugins\MidnightMods-ImpactfulSkills\. No source; decompile the DLL.
- **Decompiler:** `ilspycmd` (c:\Users\sound\.dotnet\tools\), e.g. `ilspycmd -t InventoryGui <assembly_valheim.dll>` for one type or `-p -o <dir>` for a whole project. Decompile into a temporary folder, not into a project.
- **Other Nhx projects:** in c:\Users\sound\OneDrive\Projects\Visual Studio\. This project's layout and csproj are modeled on NhxSimpleDeposit.

# Building and installing

- Old-style .NET Framework 4.8 csproj, like the other Nhx mods. Build with Visual Studio's MSBuild:
  `& "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" NhxIdolProgression.csproj /p:Configuration=Release`
  The output goes to obj\Release\NhxIdolProgression\: the DLL plus manifest.json, README.md and CHANGELOG.md.
- To install, copy everything in obj\Release\NhxIdolProgression\ into HeavyMods\BepInEx\plugins\NhxIdolProgression\. The server and every player need the same version: Jotunn's NetworkCompatibility (EveryoneMustHaveMod, Minor) enforces it.
- The build has no warnings.
- **References:** the shared libraries under c:\Users\sound\OneDrive\Projects\ (bepinex_mod_libs, publicized_assemblies\1.0-release, valheim_mod_libraries), netstandard.dll and Splatform.dll from the game's valheim_Data\Managed\, and Jotunn from the HeavyMods profile. Splatform is needed because `Chat.AddString` has an overload taking a `PlatformUserID`.
- `AllowUnsafeBlocks` is true, though there's no unsafe code: it makes the compiler add `SecurityPermission(SkipVerification)`, which lets the code read private game members (`InventoryGui.m_selectedRecipe`, `m_craftUpgradeItem`, `Chat.m_hideTimer`...) through the publicized assemblies.
- The csproj lists every file explicitly, so add new files to it.

# How it works

## Files

- **Plugin.cs:** the BepInEx plugin, GUID "NhxIdolProgression". Hard dependency on Jotunn, `[BepInIncompatibility("akuichi.ReforgedPotential")]`, the "Announce Upgrades" setting, then registers the recipes and calls `Harmony.PatchAll`.
- **IdolRecipes.cs:** the 16 idol recipes, registered through Jotunn's ItemManager at the artisan table (`piece_artisanstation`), station level 1. Hardcoded on purpose.
- **Idols.cs:** idol prefab names and the progression rule.
- **IdolSwap.cs:** puts the idol an upgrade takes into the recipe's requirement for the duration of one call, then puts the recipe's own idol back.
- **Patches\IdolProgressionPatches.cs:** where the swap happens.
- **UpgradeAnnouncements.cs:** the chat announcements.

## Idols

The Forge of Potential's idols are the prefabs `Upgrader<tier><kind>`: tier 0 (Wooden) to 7 (Bloodgold), kind `Weapon` (battle idols) or `Armor` (protection idols). An item's recipe asks for one of them through a requirement with `m_upgraderResource` set, which only counts at an upgrader station (`CraftingStation.m_upgrader`). The game's gear, and Therzie's, Wild Weapons' and ValheimArmory's, ask for 1 idol with no increase per level. Requirements asking for anything else (another mod's idol) are left alone.

## Idol progression

- An upgrade from level L takes the idol `min(7, recipe tier + (L - 1) / 4)`: upgrades from levels 1-4 take the recipe's idol, 5-8 the next tier, and so on up to Bloodgold. That's what Reforged Potential did on our server, and also its rule with boss progression off. Only the idol changes, never the amount.
- **Why a swap, and not a change to the recipe:** a recipe and its requirement objects are shared by every copy of the item, whatever the copy's level. Reforged Potential rewrote the requirement each time an item was selected, so the Forge's list checked every item against the last selected item's idol and greyed them out wrongly.
- The swap is active, in each case for the level the item is upgraded to:
  - `Player.HaveRequirements(Recipe, bool, int, int)`, when not discovering and at an upgrader: the list's greyed-out state for each item, the craft button, and the check inside the upgrade.
  - `InventoryGui.UpdateRecipe`: the selected item's requirement list and craft button, every frame.
  - `InventoryGui.DoCrafting`: the upgrade itself. The game takes its odds from the first upgrader requirement's idol, then takes the idols with `ConsumeResources`.
- The prefixes run with `Priority.First` and the finalizers with `Priority.Last`, so the other mods' patches on these methods run inside the swap and see the right idol.
- Swaps nest: `DoCrafting` calls `HaveRequirements` and `UpdateCraftingPanel` (which rebuilds the list, checking other levels). `IdolSwap` always works from the recipe's own idol and restores in reverse order.

## Announcements

- A `DoCrafting` prefix notes the item, the target level, the station and the inventory's items. The postfix works out the outcome:
  - the item is still in the inventory: nothing was attempted (missing requirements, or another mod stopped it);
  - no new item of the same name: destroyed;
  - otherwise: upgraded if the new item reached the target level, lost a level if not.
- The result goes to `ZRoutedRpc.Everybody` as RPC "NhxIdolProgression_UpgradeResult" (registered in a `ZNet.Awake` postfix). The sender handles it locally and the server passes it on, so everyone sees it, the sender included. Dedicated servers skip it (no `Chat.instance`).
- The text is fixed English. Item and station names are sent as localization keys and translated by each receiver. The chat title is the station's name (Forge of Potential).
- "Announce Upgrades" is admin only, so Jotunn syncs the server's value to every client. Both the sender and the receivers check it.

## Other mods

- **ImpactfulSkills:** its forging skill improves the Forge's odds. It has code for Reforged Potential that finds it by its GUID `akuichi.ReforgedPotential` and its class names, and overwrote Reforged's Upgrade/Break Chance config values. Without Reforged it uses its normal code: a `DoCrafting` prefix changes the odds on the idol's shared data (the first upgrader requirement's item) and a finalizer restores them. Our swap runs first, so it changes the swapped idol's odds. It also clamps an item that fails at level 1 to level 1 instead of the game's level 0, and caps the materials refunded on a break with a transpiler on `DoCrafting`.
- **AzuCraftyBoxes:** its patches on `HaveRequirements`, `HaveRequirementItems`, `SetupRequirement` and `ConsumeResources` read the requirements when called, so they count and take the swapped idol from nearby containers.
- **NhxBetterChat** (our own, source in ..\NhxBetterChat\): when its world chat is on, it patches `Terminal.AddString(string, string, Talker.Type, bool)`, the overload the announcements call. Each announcement gets its `[HH:mm:ss]` stamp and plays its chat sound. The sound plays for the sender's own upgrades too, because it only recognizes your own messages by a `PlatformUserID` first argument.
- **Jewelcrafting** (and the other mods embedding ItemDataManager: AzuCraftyBoxes, AzuExtendedPlayerInventory): keeps an upgraded item's custom data, sockets and gems included, through a transpiler on `DoCrafting` that remembers the item passed to `Inventory.RemoveItem` and copies its data onto the next item created. That's why the game's own `DoCrafting` must run: Reforged Potential's prefix replaced it and lost sockets and gems on every upgrade.

# Decisions made with the user

- Keep from Reforged Potential only: idols craftable at the artisan table, idol progression and the upgrade announcements.
- The crafting station is always the artisan table, and the idol recipes are hardcoded. They started from Reforged Potential's defaults, then the user changed them on 2026-10-09:
  - The metals must be bars, not ores. They already were.
  - Iron idols also take 2 Surtling Cores.
  - Flametal idols: 10 FlametalNew, 2 CharcoalResin, 5 ProustitePowder, instead of 15 Charred Bone. The user wrote "ProustitePowderarcoal", which isn't a prefab; ProustitePowder was used.
  - Bloodgold idols no longer take 40 Coins, and take 1 Frostcore instead (the user called it "frozen core").
- Idols can only be crafted once players have the artisan table (it takes Dragon Tears, so after Moder). That's intentional: the user wants the early game to stay challenging despite the mods' power creep, and the progression from wood to silver is fine without idols. Later, players can craft lower-tier idols to improve their favorite lower-tier gear, or carry on with regular progression.
- No level cap on upgrades (confirmed again 2026-10-09). ImpactfulSkills' decay already acts as a soft cap; see "Item levels and glow" below.
- One recipe per idol tier, shared by its battle and protection idols: simpler, and players can pick either kind with the same materials. Confirmed 2026-10-09, when the user started tweaking the recipes. Their aim: not too cheap, but every player should be able to craft at least 10 of each.
- Idol progression: one tier every 4 levels counted from level 1, Bloodgold idols being the final tier. Each upgrade takes the amount the item's recipe asks for (no cost scaling).
- The success, degrade and break chances are the game's, improved by ImpactfulSkills. Reforged Potential's own chances are gone, and with them its replacement of the upgrade code.
- Announcements: on by default, switched on the server for everyone, with three messages (upgraded, lost a level, destroyed).
- Dropped: boss progression, level cap, cost scaling, upgrade time (back to the game's 8 s + 1 s per level), idol refunds on break, keeping the level on failure, EpicLoot support (EpicLoot isn't installed).
- The Forge of Potential's list, requirements and craft button must use the idol each item needs, counting containers through AzuCraftyBoxes.
- Write it properly and fix the problems found in Reforged Potential rather than copying its code.

## Ideas discussed but not done

- **Boss progression from the world's boss-defeated keys** instead of each character's kill stats, if boss progression ever comes back (see "Boss kills" below).
- **Re-adding Reforged's "keep the level on failure" and "refund idols on break"** as small patches on top of the game's code. The first would be a prefix on the 10-argument `Inventory.AddItem` during an upgrade, raising the quality. The second would be a `DoCrafting` postfix, using the outcome detection the announcements already do.

# Research notes

What the first session found, so it doesn't have to be looked up again. That session's decompiled code was in a temporary folder that's gone. To re-decompile, use `ilspycmd`:
- the game's `valheim_Data\Managed\assembly_valheim.dll`;
- the mods' DLLs in HeavyMods\BepInEx\plugins\.

## The game's upgrade code

- **The Forge of Potential's list** (`InventoryGui.UpdateRecipeList` at an upgrader station):
  - It holds every inventory item whose recipe has `m_maxQuality > 1` and an upgrader requirement, at any level. Items above the regular station's max level are included too.
  - An item can be crafted if `HaveRequirements(recipe, false, item.m_quality + 1)` is true, or the NoCraftCost key is set. Otherwise it's greyed out: a transparent icon and a grey name.
- **The requirement filter** in `HaveRequirementItems`, `SetupRequirementList`, `ConsumeResources` and `GetFirstRequiredItem`:
  - At a station, a requirement counts only if `station.m_upgrader == requirement.m_upgraderResource`.
  - Without a station, upgrader requirements never count.
  - `SetupRequirementList` also skips requirements whose amount is 0. It doesn't require the player to know the material, except for one-ingredient recipes.
- **`Piece.Requirement.GetAmount(q)`:**
  - At `q <= 1`: `m_amount`.
  - Otherwise: `floor(f * m_amountPerLevel + (m_upgraderResource ? m_amount : 0))`, where `f = q - 1` below 4 and `4 + (q - 4) / 2` from 4.
- **`InventoryGui.OnCraftPressed` at an upgrader** needs (number of `m_recover` requirements + 1) empty slots.
- **Upgrade time:** `UpdateRecipe` uses `m_upgraderDuration` (8) + target level × `m_upgraderDurationPerLevel` (1). It's shortened by the station's crafting skill, if it has one. When the timer ends it calls `DoCrafting`.
- **`InventoryGui.DoCrafting(Player)` at an upgrader:**
  1. Returns early if:
     - `HaveRequirements(recipe, false, target)` fails, unless a no-cost cheat or the NoCraftCost key applies;
     - the item is no longer in the inventory;
     - a one-ingredient recipe finds no ingredient.
  2. Unequips and removes the item (`Inventory.RemoveItem(ItemData)`) before rolling.
  3. Takes the odds from the first requirement with `m_upgraderResource`: its item's `m_shared` (`m_upgradeChance`, `m_breakChance`, `m_breakReturnIngreientsAmount`).
  4. Rolls once, `r = Random.Range(0, 1)`:
     - **Success** if `m_upgradeChance >= r`: `AddItem` at the target level.
     - **Break** if `m_breakChance >= 1 - r`: for each `m_recover` requirement, refunds `ceil((GetAmount(1) + GetAmount(current level)) × refund ratio)` of it.
     - **Otherwise failure:** `AddItem` at target − 2, which is current − 1, so level 0 for a level 1 item.
     - Center messages: `$msg_upgrader_success`, `_broke`, `_failed`.
  5. `ConsumeResources(recipe.m_resources, target, -1, multiplier)`, unless no cost applies.
  6. `UpdateCraftingPanel()` (rebuilds the list), raises the recipe station's crafting skill, plays `m_craftItemDoneEffects` or `m_craftItemDoneFailEffects`.
- **`Inventory.AddItem(string name, stack, quality, variant, crafterID, crafterName, Vector2i position, cheated, pickedUp = false, dropIfFullInv = true)`** is the 10-argument overload `DoCrafting` uses. It instantiates the prefab (so `ItemDrop.Awake` runs), then sets quality, variant, durability and crafter. Anything `Awake` sets is overwritten afterwards.
- **`ZRoutedRpc.InvokeRoutedRPC(ZRoutedRpc.Everybody, ...)`** handles the call on the sender. It also sends it to the server, which handles it and forwards it to the other peers. `ZNet.Awake` creates the `ZRoutedRpc` instance.
- **`Terminal.AddString(title, text, Talker.Type)`** writes `<color=orange>title</color>: <color=...>text</color>`. Shout makes the text yellow and uppercase; Whisper makes it lowercase. Setting `Chat.m_hideTimer = 0` shows the chat window for a while.

## Deep North materials

From community guides (web, 2026-10-09), not from the game's files:
- **Frostcore** (`FrostCore`): from Frostcore hangers (`Pickable_FrostCoreHanger`, said to be in the Mountains' frost caves) and from the Winding Tunnels dungeon, neither of which refills. Fryslings (`Frysling`) are said to drop 1 each, which makes it renewable. The game has text for a "Frysling Core" but no prefab of that name, which fits with Fryslings dropping Frostcores.
- **Memorial Coal** (`MemorialCoal`): from the Mörkhalla dungeon (`DG_MorkHalla`): coal piles (`Morkhalla_coal_pile_memorial`), firepits and braziers, which don't refill, and Krigen, said to drop one 20% of the time. 3 of them light an Ancient Altar (`offeraltar_memorialsite`) to fight an Ancestor, which drops the Frostfire and Thunderblood essences that Frostfire and Thunderblood weapons need.

## Item levels and glow

- **No maximum level at the Forge:** at an upgrader, `UpdateRecipeList` and `DoCrafting` skip the max quality check. The level is saved as a 16-bit number.
- **The glow** is `ParticleIntensityScaler`, on the items' particle effects. Up to its `maxQuality` (4 in the code) the glow is off. Above that, intensity = 1 + (level − maxQuality) × `itemDropLevelMultiplier` (0.1 in the code). Emission, size and the other particle values scale with intensity without a cap. The color follows a gradient until intensity reaches `intensityColorMax` (10 in the code), and lights can be capped by `lightIntensityMax`. Each item's prefab can set its own values, which aren't readable (compressed bundles). So nothing in the code stops at level 20.
- **ImpactfulSkills' decay is a soft cap:** with the server's settings (decay from level 10, 5% per level), success = (0.65 + 0.2 × skill factor) × 0.95^(target − 10), counting the 0.65 from its config text. A failure loses a level, so ignoring breaks an item stops progressing on average once success falls below 50%: around level 15 at no forging skill, and 20 at full skill.

## Boss kills

Kept in case boss progression comes back.
- **What Reforged read:** `Game.instance.GetPlayerProfile().m_playerStats[0].m_enemyStats[0]`, keyed by:
  - `$enemy_eikthyr`, `$enemy_gdking`, `$enemy_bonemass`, `$enemy_dragon`, `$enemy_goblinking`, `$enemy_seekerqueen`, `$enemy_fader`;
  - `$enemy_frozenking_p3`, Kall Fimbulbringer, which is in the game.
- **These stats are per character, not per world.** `Character.OnDeath` credits a kill (`Game.RPC_RegisterKill`) to every player flagged as an attacker in the boss's ZDO. So a player who didn't hit the boss gets no credit, and a character keeps kills made in other worlds.
- The world's boss-defeated global keys would be the shared alternative. Their exact names weren't checked.

## Other mods in HeavyMods that patch DoCrafting

Found by grepping the HeavyMods DLLs for "DoCrafting", then decompiling.

- **ImpactfulSkills** (0.21.1):
  - `RefinementChancePatch`: a prefix with `HarmonyBefore("akuichi.ReforgedPotential")`, plus a finalizer. It sets the forging odds on the idol's shared data and restores them afterwards.
  - `BreakRefundCapPatch`: a transpiler that caps the break refund at the item's max quality.
  - `ForgedCraftPatch`: prefix, postfix and finalizer, together with a prefix and postfix on the 10-argument `Inventory.AddItem`.
    - Gives forging XP. A refinement counts as attempted when the item left the inventory, and as succeeded when an `AddItem` of the recipe's prefab reached the target level.
    - Adds the masterwork and lightweight marks.
    - Clamps an upgrader item's quality below 1 up to 1.
  - An `UpdateRecipe` postfix shows the odds (`$forging_refine_chances`).
  - Its Reforged Potential code (`compatibility/ReforgedPotential.cs`, `Modcheck` checking for plugin `akuichi.ReforgedPotential`) is inactive without Reforged.
- **Jewelcrafting:**
  - ItemDataManager's transpiler and finalizer move custom data to the upgraded item.
  - `KeepSockets.CopySockets`: prefix and finalizer, for crafting only, not upgrades.
  - `StopCorruptedItemUpgrade`: a prefix that returns false for items with "Corrupted Item" data.
  - `GemStones.TryUpgradingGems` and `AddSocketToItem`: prefixes for its own gem recipes and socket tab.
  - `JewelrySetup`: a transpiler on the 10-argument `AddItem`, for the orange necklace's quality.
- **AzuCraftyBoxes:**
  - On `DoCrafting`: a crafting-flag prefix and finalizer, plus its own copy of ItemDataManager.
  - Elsewhere:
    - a postfix on `HaveRequirements` (counts containers);
    - patches on `HaveRequirementItems` and `GetFirstRequiredItem`;
    - a `SetupRequirementList` prefix and a `SetupRequirement` postfix (shows container totals);
    - a `ConsumeResources` prefix that returns false and takes from containers.
- **AzuExtendedPlayerInventory:** `DoCrafting_TrackUpgradeEquipped` re-equips the upgraded item, through an `Inventory.AddItem` postfix. It also has its own copy of ItemDataManager.
- **CreatureLevelAndLootControl:** a transpiler that replaces the 8-argument `Inventory.AddItem`. Upgrades use the 10-argument one, so they aren't affected.
- **Therzie Armory:** `KeepSocketsWhileUpgrading`, a prefix and finalizer that copy gems from `resources[0]` when crafting its items.
- **HipLantern:** crafting only.
- **Gear mods' idols:** Therzie Armory, Warfare and Monstrum, Wild Weapons and ValheimArmory all reference `m_upgraderResource`. Therzie's items use `RequiredUpgradeItems.Add("Upgrader<N><Armor|Weapon>", 1, 0, upgraderResource: true)`, so they ask for the game's idols, 1 per upgrade.

## ImpactfulSkills settings on the server

MidnightsFX.ImpactfulSkills.cfg:
- EnableForging = true, EnableRefinementBonus = true, RefinementBonusLevel = 0.
- RefineSuccessChanceBonus = 0.2. Its description says the game's idols have a 0.65 chance.
- RefineBreakChanceReduction = 1.
- RefineBreakRefundBonus = 0.35. Its description says the game's idols refund 0.35.
- EnableRefinementDecay = true, RefineDecayStartLevel = 10, RefineDecayRate = 0.05.
- ForgingRefineAttemptXP = 2, ForgingRefineSuccessXPMultiplier = 2.

Its odds, with the skill factor (0 to 1) and decay (1 up to RefineDecayStartLevel):
- Success = clamp01(upgradeChance + skill × bonus) × decay.
- Refund = clamp01(refund + skill × bonus) × decay.
- Break = min(1 − Success, breakChance) × clamp01(1 − skill × reduction × decay).

## Reforged Potential, reviewed

What the user decided against, and what not to repeat:
- **Its `DoCrafting` prefix replaced the game's upgrade code** and returned false. It used its own Upgrade/Break Chance, with two independent rolls. In doing so it:
  - skipped Jewelcrafting's transpiler, losing sockets and gems on every upgrade;
  - skipped ImpactfulSkills' refund cap;
  - never checked that the item and the idols were still there when the timer ended. Its own code says it computes that but never uses it. Reading the code, dropping the item during the timer should duplicate it, and dropping the idols should make the upgrade free. Not tested.
  - refunded the next level's amount on a break;
  - showed a hardcoded "Upgrade failed!".
- **Its `SetRecipe` postfix changed the recipe's idol requirement for good,** both the idol and the amount, to suit the selected item. That's the cause of its README's known bug: icons greyed out. It also set the selected weapon's own `m_breakReturnIngreientsAmount` to 1, which does nothing.
- **Smaller problems:**
  - The level cap only applied with boss progression on.
  - The "Defeat X to upgrade this weapon further" message also said "weapon" for armor.
  - Two Info logs were written on every upgrade.
  - At very high levels, idol progression could ask for a nonexistent `Upgrader8...`.
  - Its server-side chat handler ran even on a dedicated server, where `Chat.instance` is null.
- **Its config on the server when it was replaced:**
  - Upgrade Chance 1, Break Chance 0.
  - Idol cost 1, +1 per level from level 1.
  - Upgrade time 2 s + 1 s per level.
  - Boss progression on: base 5, +4 per boss, +100 for boss 8.
  - Idol progression on, recipes at the artisan table, announcements off.
  - The file lists every key twice. The numbered "01. ..." copies are leftovers from an older version.

## Looking things up in the game's files

- **Prefab names:** valheim_Data\StreamingAssets\SoftRef\manifest_extended contains `/<PrefabName>.prefab`.
- **English text:** valheim_Data\resources.assets contains `"key","text"` pairs, e.g. `LC_ALL=C grep -a -o -E '"item_gold","[^"]*"' resources.assets`.
  - piece_upgradestation = Forge of Potential.
  - item_upgrader_name = Idol.
  - item_upgrader_tier0..7 = Wooden, Bronze, Iron, Silver, Black Metal, Black Marble, Flametal, Bloodgold.
  - item_upgrader_weapon = Battle, item_upgrader_armor = Protection.
  - item_gold = Bloodgold, item_elderbark = Ancient Bark, item_bug_meat = Seeker Meat, item_ancientcoin = Ancient Coin.
  - item_frostcore = Frostcore ("filled with frozen energy"), prefab `FrostCore`. item_fryslingcore (Frysling Core) has text but no item prefab.
- **Metal prefabs, bar / ore:** Copper / CopperOre, Tin / TinOre, Bronze, Iron / IronOre (+ IronScrap), Silver / SilverOre, BlackMetal / BlackMetalScrap, FlametalNew (Flametal) / FlametalOreNew, Gold (Bloodgold) / GoldOre (Petrified Tissue). The old `Flametal` and `FlametalOre` are Ancient Metal and Glowing Metal Ore (item_flametal_old, item_flametalore_old); ValheimArmory's Flametal recipes use FlametalNew.
- **Not read:** recipe and idol values are inside compressed bundles. Two numbers come from other sources and haven't been checked in-game:
  - 1 idol per upgrade, from Therzie's recipes and Reforged's README ("Game Default is 1");
  - the 0.65 success and 0.35 refund odds, from ImpactfulSkills' config text.
