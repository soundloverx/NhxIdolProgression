using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Jotunn.Utils;
using System.Reflection;

namespace NhxIdolProgression
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid, BepInDependency.DependencyFlags.HardDependency)]
    // Reforged Potential replaces the Forge of Potential's upgrade code and rewrites the same idol requirements, so
    // running both would mix its rules with ours
    [BepInIncompatibility("akuichi.ReforgedPotential")]
    // Each player's own client decides which idol their upgrades take, so everyone needs the same version
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    public class Plugin : BaseUnityPlugin
    {
        internal const string PluginGuid = "NhxIdolProgression";
        private const string PluginName = "NhxIdolProgression";
        private const string PluginVersion = "1.0.0";

        internal static Plugin Instance { get; private set; }
        internal static ManualLogSource Log { get; private set; }

        private ConfigEntry<bool> _announceUpgrades;
        private Harmony _harmonyInstance;

        internal static bool AnnounceUpgrades => Instance?._announceUpgrades == null || Instance._announceUpgrades.Value;

        private void Awake()
        {
            Instance = this;
            Log = Logger;

            // Admin only: Jotunn syncs the server's value to every player
            _announceUpgrades = Config.Bind(
                "General",
                "Announce Upgrades",
                true,
                new ConfigDescription(
                    "Announce in every player's chat when someone upgrades an item at the Forge of Potential, and whether it was upgraded, lost a level or was destroyed. The server's setting applies to every player.",
                    null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }
                )
            );

            IdolRecipes.Register();

            Assembly assembly = Assembly.GetExecutingAssembly();
            _harmonyInstance = new Harmony(PluginGuid);
            _harmonyInstance.PatchAll(assembly);

            Log.LogInfo($"{PluginName} v{PluginVersion} loaded.");
        }

        private void OnDestroy()
        {
            _harmonyInstance?.UnpatchSelf();

            Log?.LogInfo($"{PluginName} v{PluginVersion} unloaded.");

            Instance = null;
        }
    }
}
