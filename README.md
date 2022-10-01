# Public Transport Unstucker
Unstucks public transport by fixing known causes of stucking.

Alternatively:

Buses are stuck? metros are stuck? Public transport is stuck? Buses and metros arrive at their stations but never depart? Unbunching is disabled but they are still stuck? Look no further! Public Transport Unstucker (PTU) is here to help.

# Mod Status
- No known incomaptibility problems (yet; TODO does EBS work with this?)

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
- Helicopers
- Ships
- Ferries

Other types are not yet covered, but perhaps they are also affected by this bug. I am not sure.

# Motivation and Technical Information
It actually started when I was developing my other mod Express Bus Services. When testing that mod, I noticed that sometimes buses will fail to depart, yet the logic was correct on my side. Using ModTools, I discovered this phenomenon/bug which I dubbed the "Citizen Runaway Problem" ("CRP").

The main idea of CRP is very simple. It is known that buses etc cannot depart before everyone has entered the vehicle and despawned, this makes sense. However, randomly, there will be passengers who declare that they have entered the vehicle but actually did not (fun fact: they will begin to slide through the terrain to their destination so they usually cannot be seen). This results in the vehicles waiting basically forever for those rogue passengers to return, making them stuck.

I do not understand why this happens, dor do I have the time to find out, but I do know how to "fix" this problem: whenever this problem appears, disappear the problem. Simple.

When supported public transport vehicles are checking whether everyone is on board, I add an extra check to detect rogue passengers, and if there are such passengers, I force-despawn them. This sends them back to the "background" and the "all aboard" check passes.
