# Public Transport Unstucker
Unstucks public transport by fixing known causes of stucking.

Alternatively:

Buses are stuck? metros are stuck? Public transport is stuck? Buses and metros arrive at their stations but never depart? Unbunching is disabled but they are still stuck? Look no further! Public Transport Unstucker (PTU) is here to help.

**Update (31 May 2025): as of Patch 1.17.1-f2 (released 12 June 2023), the bug that motivated the creation of this mod is fixed by Colossal Order themselves. As such, this mod is no longer needed.**

**With this, the repo shall be archived.**

# Mod Status
- No known incompatibility problems

Because this mod is very small and simple, I am open-sourcing this to everyone out there.

## Types of Transportation Covered

These transportation types are currently covered by this mod:

- Buses; as a result of game mechanics, the following transportation types are also covered:
  - Evacuation buses
  - Tourism buses
  - Intercity buses
- Trolleybuses
- Metros
- Trains
- Planes
- Blimps
- Helicopters
- Ships
- Ferries

Other types are not yet covered, but perhaps they are also affected by this bug. I am not sure.

# Motivation and Technical Information
It actually started when I was developing my other mod Express Bus Services. When testing that mod, I noticed that sometimes buses will fail to depart, yet the logic was correct on my side. Using ModTools, I discovered this phenomenon/bug which I dubbed the "Citizen Runaway Problem" ("CRP").

The main idea of CRP is very simple. It is known that buses etc cannot depart before everyone has entered the vehicle and despawned, this makes sense. However, randomly, there will be passengers who declare that they have entered the vehicle but actually did not (fun fact: they will begin to slide through the terrain to their destination so they usually cannot be seen). This results in the vehicles waiting basically forever for those rogue passengers to return, making them stuck.

I do not understand why this happens, nor do I have the time to find out, but I do know how to "fix" this problem: whenever this problem appears, disappear the problem. Simple.

When supported public transport vehicles are checking whether everyone is on board, I add an extra check to detect rogue passengers, and if there are such passengers, I force-despawn them. This sends them back to the "background" and the "all aboard" check passes.

Special Note: if for some reason the passenger in question is too far away from the transport vehicle, then the passenger is also considered as a rogue passenger. The rogue distance is different for each type of public transport covered, but the range is set so that they do not block vanilla behavior using vanilla numbers.

For example, currently trains have a rogue range of 160 units (16 tiles), which is the max length of a vanilla train platform. If the passenger is somehow beyond 160-units far, then it is marked as rogue and forced to despawn into the vehicle.
