using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace NhxIdolProgression
{
    // Tells every player in chat how an upgrade at the Forge of Potential went. The game decides the outcome (with
    // ImpactfulSkills' forging bonus), this only looks at what came out of it.
    internal static class UpgradeAnnouncements
    {
        private const string RpcName = "NhxIdolProgression_UpgradeResult";

        private enum Outcome
        {
            Upgraded = 0,
            LostLevel = 1,
            Destroyed = 2
        }

        private sealed class PendingUpgrade
        {
            internal ItemDrop.ItemData Item;
            internal int TargetLevel;
            internal string StationName;
            internal HashSet<ItemDrop.ItemData> ItemsBefore;
        }

        // ZNet.Awake creates the game's routed RPCs, once per session
        [HarmonyPatch(typeof(ZNet), "Awake")]
        private static class RegisterRpc
        {
            private static void Postfix()
            {
                ZRoutedRpc.instance.Register<ZPackage>(RpcName, OnUpgradeResult);
            }
        }

        [HarmonyPatch(typeof(InventoryGui), "DoCrafting")]
        private static class WatchUpgrade
        {
            private static void Prefix(InventoryGui __instance, Player player, out PendingUpgrade __state)
            {
                __state = null;

                ItemDrop.ItemData item = __instance.m_craftUpgradeItem;
                CraftingStation station = player != null ? player.GetCurrentCraftingStation() : null;

                if (item == null || station == null || !station.m_upgrader || !Plugin.AnnounceUpgrades)
                {
                    return;
                }

                __state = new PendingUpgrade
                {
                    Item = item,
                    TargetLevel = item.m_quality + 1,
                    StationName = station.m_name,
                    ItemsBefore = new HashSet<ItemDrop.ItemData>(player.GetInventory().GetAllItems())
                };
            }

            private static void Postfix(Player player, PendingUpgrade __state)
            {
                if (__state == null)
                {
                    return;
                }

                Inventory inventory = player.GetInventory();

                // The game takes the item before rolling, so if it's still there nothing was attempted (missing
                // requirements, or another mod stopped the upgrade)
                if (inventory.ContainsItem(__state.Item))
                {
                    return;
                }

                // The upgraded or downgraded copy is a new item; when the item broke there's none
                string itemName = __state.Item.m_shared.m_name;
                ItemDrop.ItemData result = inventory.GetAllItems().FirstOrDefault(item => !__state.ItemsBefore.Contains(item) && item.m_shared.m_name == itemName);

                Outcome outcome;

                if (result == null)
                {
                    outcome = Outcome.Destroyed;
                }
                else if (result.m_quality >= __state.TargetLevel)
                {
                    outcome = Outcome.Upgraded;
                }
                else
                {
                    outcome = Outcome.LostLevel;
                }

                ZPackage package = new ZPackage();
                package.Write(player.GetPlayerName());
                package.Write(itemName);
                package.Write(__state.StationName);
                package.Write((int)outcome);
                package.Write(__state.TargetLevel);
                package.Write(result != null ? result.m_quality : 0);

                Plugin.Log.LogInfo($"Announcing upgrade of {itemName} to level {__state.TargetLevel}: {outcome}.");

                // Everybody includes this client: the game handles it here too, then the server passes it on
                ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, RpcName, package);
            }
        }

        private static void OnUpgradeResult(long sender, ZPackage package)
        {
            // A dedicated server has no chat to show it in
            if (Chat.instance == null || Localization.instance == null || !Plugin.AnnounceUpgrades)
            {
                return;
            }

            string playerName = package.ReadString();
            string itemName = Localization.instance.Localize(package.ReadString());
            string stationName = Localization.instance.Localize(package.ReadString());
            Outcome outcome = (Outcome)package.ReadInt();
            int targetLevel = package.ReadInt();
            int resultLevel = package.ReadInt();

            string text;

            switch (outcome)
            {
                case Outcome.Upgraded:
                    text = $"{playerName} upgraded {itemName} to level {targetLevel}.";
                    break;

                case Outcome.LostLevel:
                    // An item at level 1 can't lose a level
                    text = resultLevel < targetLevel - 1
                        ? $"{playerName} failed to upgrade {itemName} to level {targetLevel}, it dropped to level {resultLevel}."
                        : $"{playerName} failed to upgrade {itemName} to level {targetLevel}.";
                    break;

                case Outcome.Destroyed:
                    text = $"{playerName} failed to upgrade {itemName} to level {targetLevel}, it was destroyed.";
                    break;

                default:
                    return;
            }

            Chat.instance.AddString(stationName, text, Talker.Type.Normal);

            // Shows the chat window for a moment, like a new chat message does
            Chat.instance.m_hideTimer = 0f;
        }
    }
}
