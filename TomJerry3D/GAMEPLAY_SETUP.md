# Tom & Jerry 3D — gameplay setup

This repository is a Unity C# prototype for an original cartoon mouse-and-cat chase game. The player controls the mouse in first person; the cat patrols, notices the player, chases, and searches the last known position.

## First-person player
1. Create a Player object and add a CharacterController and MousePlayerController.
2. Add a child object named CameraPivot at the mouse's eye height; put the Main Camera under it.
3. Assign CameraPivot to the controller's Camera Pivot field. The player object rotates left/right; the camera pivot rotates up/down.
4. Add CharacterHandPickup to the player. Assign the Main Camera to View and a child hand socket to Hand Socket.

## Android touch controls
1. Add MobileFirstPersonHUD to an active object in the scene.
2. Assign the player and its CharacterHandPickup, or leave them empty to auto-find the components.
3. Press Play: the HUD creates a left movement joystick, a right swipe-look area and a USE button.
4. The USE button picks up/drops carryable objects and interacts with the fridge or trap. The same player controller also supports keyboard/mouse testing in the Editor.

The touch look input is consumed once per frame, so the camera does not keep rotating after a swipe ends.

## Tom AI and house
- Add a NavMeshAgent and CatAI to Tom. Assign the player transform and patrol points.
- Bake a NavMesh for the walkable house floor.
- Add HouseBlockoutBuilder to an empty scene object to create a simple connected living room, kitchen, hallway, bedroom and storage-room blockout.
- Add CartoonJerryFactory and CartoonTomFactory to empty objects to generate lightweight placeholder character models.

## Existing gameplay systems
- CharacterHandPickup lets either character carry and drop CarryableItem objects.
- KitchenFridgeBuilder creates an interactive fridge and food dispenser.
- MouseTrapBaitSocket only catches Tom when food is placed in the trap.
- TomHouseChaos lets Tom knock over lightweight cartoon props; after enough mess, the owner scolds him and sends him outside temporarily.
- BreakableProp can restore harmless props after a short delay.

## Current limits
The repository contains scripts and setup documentation, not a finished Unity scene, rigged animation, final art, audio, baked NavMesh or Android APK. Those need to be assembled and tested in Unity Editor on a supported computer or cloud build machine. Use original or properly licensed assets; the placeholder characters are procedural primitives.
