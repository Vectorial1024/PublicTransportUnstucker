using HarmonyLib;
using JetBrains.Annotations;

namespace PublicTransportUnstucker
{
    [HarmonyPatch(typeof(TaxiAI))]
    [HarmonyPatch(nameof(TaxiAI.CanLeave), MethodType.Normal)]
    [UsedImplicitly]
    public class Patch_TaxiAI_AntiRogue
    {
        [HarmonyPrefix]
        [UsedImplicitly]
        public static void InterveneRogueCitizens(ushort vehicleID, ref Vehicle vehicleData)
        {
            if (vehicleData.m_waitCounter > 0 && vehicleData.m_waitCounter % 2 == 0)
            {
                // do not check too often, go easy on the CPU
                // however, taxis have a much lower boarding time of 4 ticks, so we gotta check frequently
                RoguePassengerTable.FixInvalidPublicTransitPassengers(vehicleID, ref vehicleData);
            }
        }
    }
}
