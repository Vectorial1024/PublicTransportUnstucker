using HarmonyLib;

namespace PublicTransportUnstucker
{
    [HarmonyPatch(typeof(PassengerShipAI))]
    [HarmonyPatch("CanLeave", MethodType.Normal)]
    public class Patch_PassengerShipAI_AntiRogue
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
