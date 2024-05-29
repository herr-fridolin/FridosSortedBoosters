using BepInEx;
using BepInEx.Unity.IL2CPP;
using CellMenu;
using HarmonyLib;
using LogUtils;

namespace FridosSortedBoosters
{
    [BepInPlugin("com.fridos.sortedboosters", "FridosSortedBoosters", "1.0.0")]
    public class Plugin : BasePlugin
    {
        public Harmony HarmonyInstance { get; private set; }

        public override void Load()
        {
            // Plugin startup logic
            Log.LogInfo("FridosSortedBoosters is loaded!");

            HarmonyInstance = new Harmony("com.fridos.sortedboosters");
            HarmonyInstance.PatchAll();
        }
    }

    [HarmonyPatch(typeof(CM_PlayerLobbyBar), nameof(CM_PlayerLobbyBar.UpdateBoosterSelectionPoup))]
    public static class CM_PlayerLobbyBar_interact
    {
        public static bool Prefix(CM_PlayerLobbyBar __instance, BoosterImplantCategory category)
        {
            Il2CppSystem.Collections.Generic.List<global::BoosterImplantInventoryItem> boosterImplantInventory = PersistentInventoryManager.GetBoosterImplantInventory(category);
            DebugLog.LogPrefix(__instance, "Sorting boosters now.");
            List<global::BoosterImplantInventoryItem> netBoosterImplantInventory = new List<global::BoosterImplantInventoryItem>();
            foreach (var item in boosterImplantInventory)
            {
                netBoosterImplantInventory.Add(item);
            }
            // Sort the list by boosterId
            var sortedBoosterImplantInventory = netBoosterImplantInventory.OrderBy(item => item.Implant.GetCompositPublicName(false)).ToList();

            boosterImplantInventory.Clear();

            // Add sorted items back into the Il2Cpp list
            foreach (var item in sortedBoosterImplantInventory)
            {
                boosterImplantInventory.Add(item);
            }
            return true;
        }
    }
}