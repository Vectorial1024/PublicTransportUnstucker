﻿using HarmonyLib;
using JetBrains.Annotations;

namespace PublicTransportUnstucker
{
    [HarmonyPatch(typeof(PassengerFerryAI))]
    [HarmonyPatch(nameof(PassengerFerryAI.CanLeave), MethodType.Normal)]
    [UsedImplicitly]
    public class Patch_PassengerFerryAI_AntiRogue
    {
        [HarmonyPrefix]
        [UsedImplicitly]
        public static void InterveneRogueCitizens(ushort vehicleID, ref Vehicle vehicleData)
        {
            if (vehicleData.m_waitCounter > 0 && vehicleData.m_waitCounter % 4 == 0)
            {
                // do not check too often, go easy on the CPU
                RoguePassengerTable.FixInvalidPublicTransitPassengers(vehicleID, ref vehicleData);
            }
        }
    }
}
