﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PublicTransportUnstucker
{
    internal class PatchController
    {
        public static string HarmonyModID
        {
            get
            {
                return "com.vectorial1024.cities.ptu";
            }
        }

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
