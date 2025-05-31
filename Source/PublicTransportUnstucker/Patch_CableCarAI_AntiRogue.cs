using HarmonyLib;
using JetBrains.Annotations;

namespace PublicTransportUnstucker
{
    [HarmonyPatch(typeof(CableCarAI))]
    [HarmonyPatch(nameof(CableCarAI.CanLeave), MethodType.Normal)]
    [UsedImplicitly]
    public class Patch_CableCarAI_AntiRogue
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
