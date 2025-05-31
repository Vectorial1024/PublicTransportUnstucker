using HarmonyLib;
using System.Reflection;

namespace PublicTransportUnstucker
{
    internal static class PatchController
    {
        public static string HarmonyModID => "com.vectorial1024.cities.ptu";

        /*
         * The "singleton" design is pretty straight-forward.
         */

        private static Harmony harmony;

        public static Harmony GetHarmonyInstance()
        {
            if (harmony == null)
            {
                harmony = new Harmony(HarmonyModID);
            }

            return harmony;
        }

        public static void Activate()
        {
            GetHarmonyInstance().PatchAll(Assembly.GetExecutingAssembly());
            RoguePassengerTable.EnsureTableExists();
        }

        public static void Deactivate()
        {
            GetHarmonyInstance().UnpatchAll(HarmonyModID);
            RoguePassengerTable.WipeTable();
        }
    }
}
