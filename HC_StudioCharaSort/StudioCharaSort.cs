using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using DigitalCraft;

namespace StudioCharaSort
{
    [BepInPlugin(GUID, DESC, VERSION)]
    [BepInProcess("DigitalCraft")]
    public class StudioCharaSort : BasePlugin
    {
        public const string GUID = "kky.HC.StudioCharaSort";
        public const string VERSION = "1.1.0";
        public const string NAME = "StudioCharaSort";
        public const string DESC = "HC Studio Character Sort";

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

        public static ConfigEntry<sortTypes> CharaSType;
        public static ConfigEntry<sortOrders> CharaSOrder;
        public static ConfigEntry<sortTypes> CoordSType;
        public static ConfigEntry<sortOrders> CoordSOrder;
        private static bool charaSortAscend;
        private static bool coordSortAscend;

        public override void Load()
        {
            CharaSType = Config.Bind("Character cards default sort values. Changes take effect at next startup.", "Sort By", sortTypes.Name, "Set character cards default sort type. Game default is Name");
            CharaSOrder = Config.Bind("Character cards default sort values. Changes take effect at next startup.", "Sort Order", sortOrders.Descending, "Set character cards default sort order. Game default is Descending");
            CoordSType = Config.Bind("Coordinate cards default sort values. Changes take effect at next startup.", "Sort By", sortTypes.Name, "Set coordinate cards default sort type. Game default is Name");
            CoordSOrder = Config.Bind("Coordinate cards default sort values. Changes take effect at next startup.", "Sort Order", sortOrders.Descending, "Set coordinate cards default sort order. Game default is Descending");

            charaSortAscend = ((int)CharaSOrder.Value == 0) ? false : true;
            coordSortAscend = ((int)CoordSOrder.Value == 0) ? false : true;

            var harmony = new Harmony(nameof(StudioCharaSort));
            harmony.PatchAll(typeof(StudioCharaSort));
        }

        [HarmonyPostfix, HarmonyPatch(typeof(CharaList), nameof(CharaList.InitCharaList))]
        private static void PatchCharaList(CharaList __instance)
        {
            CharaFileSort charaSort = __instance.charaFileSort;
            Sorting(charaSort, (int)CharaSType.Value, charaSortAscend);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MPCharCtrl.CostumeInfo), nameof(MPCharCtrl.CostumeInfo.InitFileList))]
        private static void PatchCoordList(MPCharCtrl.CostumeInfo __instance)
        {
            System.Collections.IEnumerator DelayedCo()
            {
                // Needs a delay or the sorting will not work
                yield return null;

                CharaFileSort coordSort = __instance.fileSort;

                Sorting(coordSort, (int)CoordSType.Value, coordSortAscend);
            }

            DigitalCraft.DigitalCraft.Instance.StartCoroutine(DelayedCo().WrapToIl2Cpp());
        }
        
        private static void Sorting(CharaFileSort sortInstance, int sortType, bool sortAccend)
        {
            if (!(sortType == 0 & !sortAccend))
            {
                sortInstance.Sort(sortType, sortAccend);
            }
        }
    }
}
