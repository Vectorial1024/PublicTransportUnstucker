using ColossalFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PublicTransportUnstucker
{
    public class RoguePassengerTable
    {
        // essentially the same as the CitizenRunawayTable from ExpressBusServices
        // but renamed toa void confusion and name clash

        // remembers the last location of the citizen
        private static Dictionary<ushort, float> citizenDistanceTable;

        public static void EnsureTableExists()
        {
            if (citizenDistanceTable == null)
            {
                citizenDistanceTable = new Dictionary<ushort, float>();
            }
        }

        public static void WipeTable() => citizenDistanceTable?.Clear();

        public static void ForgetCitizen(ushort citizenInstanceID) => citizenDistanceTable?.Remove(citizenInstanceID);

        public static bool CheckIfCitizenIsRunningAway(ushort citizenInstanceID, float distance)
        {
            float previousDistance;
            if (citizenDistanceTable.TryGetValue(citizenInstanceID, out previousDistance))
            {
                ForgetCitizen(citizenInstanceID);
                return distance > previousDistance;
            }
            else
            {
                citizenDistanceTable.Add(citizenInstanceID, previousDistance);
                return false;
            }
        }

        private static void FixRoguePassengersForThisTrailer(ushort trailerVehicleID, int checkRogueRange, out int faultyCitizenCount)
        {
            faultyCitizenCount = 0;
            CitizenManager instance = Singleton<CitizenManager>.instance;
            // this is supposed to be read only
            VehicleManager vehicleManager = Singleton<VehicleManager>.instance;
            Vehicle vehicleData = vehicleManager.m_vehicles.m_buffer[trailerVehicleID];
            uint num = vehicleData.m_citizenUnits;
            int num2 = 0;
            while (num != 0)
            {
                uint nextUnit = instance.m_units.m_buffer[num].m_nextUnit;
                for (int i = 0; i < 5; i++)
                {
                    uint citizen = instance.m_units.m_buffer[num].GetCitizen(i);
                    if (citizen != 0)
                    {
                        ushort instance2 = instance.m_citizens.m_buffer[citizen].m_instance;
                        if (instance2 != 0 && (instance.m_instances.m_buffer[instance2].m_flags & CitizenInstance.Flags.EnteringVehicle) != 0)
                        {
                            // Debug.Log(citizen);

                            CitizenInstance citizenInstanceReadonly = instance.m_instances.m_buffer[instance2];
                            Vector3 citizenPosition = citizenInstanceReadonly.GetLastFramePosition();
                            Vector3 vehiclePosition = vehicleData.GetLastFramePosition();
                            float distance = Vector3.Distance(citizenPosition, vehiclePosition);

                            if (distance > checkRogueRange || CheckIfCitizenIsRunningAway(instance2, distance))
                            {
                                // This citizen is determined to be faulty.
                                // CitizenInstance is a struct and is given to us as a clone of the actual data.
                                // To manipulate the actual CitizenInstance in the game, we need to do like this
                                instance.m_instances.m_buffer[instance2].Unspawn(instance2);
                                faultyCitizenCount++;
                            }
                        }
                    }
                }
                num = nextUnit;
                if (++num2 > 524288)
                {
                    // "invalid list detected yada yada"
                    break;
                }
            }
        }

        /// <summary>
        /// Fix citizens who bugged out and appear to run away from public transits,
        /// causing delays as public transit wait endlessly for those citizens' lengthy return.
        /// </summary>
        /// <param name="vehicleID"></param>
        /// <param name="vehicleData"></param>
        public static void FixInvalidPublicTransitPassengers(ushort vehicleID, ref Vehicle vehicleData)
        {
            /*
             * We correct CIMs with two criteria:
             * 1. CIMs who are walking further and further away from the vehicle will be unspawned
             * 2. CIMs who are too far away (be they approaching the vehicle or not) will be unspawned
             * 
             * The "runaway range" for case 2 is dependent on the type of transit involved.
             * Metro runaway range will be higher than bus runaway range because metro station platforms
             * are generally larger than bus station platforms.
             * What might be faulty state in buses may be a normal state for metros if the CIM happen
             * to get assigned the metro cab at the other end of the station.
             */
            // we correct
            // determine the "runaway range": any CIMs who went
            int checkRunawayRange;
            ItemClass itemClass = vehicleData.Info.m_class;
            if (itemClass.m_service != ItemClass.Service.PublicTransport)
            {
                // out of scope
                return;
            }
            switch (itemClass.m_subService)
            {
                case ItemClass.SubService.PublicTransportCableCar:
                    checkRunawayRange = 10;
                    break;
                case ItemClass.SubService.PublicTransportBus:
                case ItemClass.SubService.PublicTransportTrolleybus:
                case ItemClass.SubService.PublicTransportTours:
                    checkRunawayRange = 30;
                    break;
                case ItemClass.SubService.PublicTransportShip:
                case ItemClass.SubService.PublicTransportPlane:
                    checkRunawayRange = 60;
                    break;
                case ItemClass.SubService.PublicTransportMonorail:
                    checkRunawayRange = 90;
                    break;
                case ItemClass.SubService.PublicTransportMetro:
                case ItemClass.SubService.PublicTransportConcourse:
                    checkRunawayRange = 150;
                    break;
                case ItemClass.SubService.PublicTransportTrain:
                    checkRunawayRange = 210;
                    break;
                default:
                    // unsupported
                    return;
            }

            // Iterate through all trailers!!!
            int totalInvalidCitizens = 0;
            ushort currentVehicleID = vehicleID;
            VehicleManager vehicleManager = Singleton<VehicleManager>.instance;
            int iterationCount = 0;
            while (true)
            {
                FixRoguePassengersForThisTrailer(currentVehicleID, checkRunawayRange, out int faultyCountInThisTrailer);
                totalInvalidCitizens += faultyCountInThisTrailer;
                currentVehicleID = vehicleManager.m_vehicles.m_buffer[vehicleID].m_trailingVehicle;
                if (++iterationCount > 16384)
                {
                    // invalid list yada yada
                    break;
                }
            }

            if (totalInvalidCitizens > 0)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("Some CIMs ran away after reserving some public transport. Correcting...");
                builder.AppendLine("");
                builder.AppendLine($"Reporting vehicle ID: {vehicleID}");
                builder.AppendLine($"Number of corrected CIMs: {totalInvalidCitizens}");
                Debug.Log(builder.ToString());
            }
        }
    }
}
