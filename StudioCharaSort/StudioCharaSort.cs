using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace StudioCharaSort
{
    [BepInPlugin(GUID, "Studio Character Sort", Version)]
    [BepInProcess("StudioNEOV2")]
    [BepInProcess("CharaStudio")]
    public class StudioCharaSort : BaseUnityPlugin
    {
        public const string GUID = "kky.aihs2.studiocharasort";
        public const string Version = "1.0.1";

        public enum sortTypes
        {
            Name,
            Date
        }

        public enum sortOrders
        {
            Descending,
            Ascending
        }

        public static ConfigEntry<sortTypes> ConfigSType;
        public static ConfigEntry<sortOrders> ConfigSOrder;
        public static bool sortAscend;

        private void Awake()
        {
            ConfigSType = Config.Bind("Character cards default sort values. Changes take effect at next startup.", "Sort By", sortTypes.Name, "Set custom default sort type. Game default is Name");
            ConfigSOrder = Config.Bind("Character cards default sort values. Changes take effect at next startup.", "Sort Order", sortOrders.Descending, "Set custom default sort order. Game default is Descending");

            sortAscend = ((int)ConfigSOrder.Value == 0) ? false : true;

            var harmony = new Harmony(nameof(StudioCharaSort));
            harmony.PatchAll(typeof(StudioCharaSort));
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Studio.CharaList), nameof(Studio.CharaList.InitCharaList))]
        public static void InitCharaList(Studio.CharaFileSort ___charaFileSort)
        {
            if (!((int)ConfigSType.Value == 0 & !sortAscend))
            {
                ___charaFileSort.Sort((int)ConfigSType.Value, sortAscend);
            }
        }
    }
}
