using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PublicTransportUnstucker
{
    [HarmonyPatch(typeof(TrolleybusAI))]
    [HarmonyPatch("CanLeave", MethodType.Normal)]
    public class Patch_TrolleybusAI_AntiRogue
    {
        [HarmonyPrefix]
        public static void PreFix(ushort vehicleID, ref Vehicle vehicleData)
        {
            if (vehicleData.m_waitCounter > 0 && vehicleData.m_waitCounter % 4 == 0)
            {
                // do not check too often, go easy on the CPU
                RoguePassengerTable.FixInvalidPublicTransitPassengers(vehicleID, ref vehicleData);
            }
        }
    }
}
