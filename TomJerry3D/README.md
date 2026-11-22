# TomJerry3D — Android Unity prototype

A first-person 3D cartoon chase prototype. Play as a small mouse exploring a house while a cat patrols, spots the player, chases and searches.

## Included
- First-person mouse movement and camera look
- Runtime Android HUD: left movement joystick, right swipe-to-look area and USE button
- Cat patrol/chase/search AI using Unity NavMesh
- Procedural house blockout: living room, kitchen, hallway, bedroom and storage
- Procedural placeholder mouse and cat characters
- Cheese pickups and a counter
- Carry/drop interactions, fridge food dispenser and baitable mousetrap
- Cartoon slapstick: Tom knocks over harmless props and gets sent outside after making a mess

## Unity setup
1. Open or create a Unity 3D URP project in Unity 6.
2. Copy the TomJerry3D folder into Assets/.
3. Follow GAMEPLAY_SETUP.md to wire the player camera, mobile HUD, NavMesh and gameplay components.
4. Add original or properly licensed 3D assets, animations, sounds and scene lighting before release.
5. Configure Android Build Support and build an APK from Unity Editor.

## Controls
- Android: left virtual joystick to move, swipe the right side to look, USE to interact/pick up/drop.
- Unity Editor: WASD/arrow keys and mouse look; E to interact.

## Status
This is a script-based prototype, not a ready-to-install APK. A Unity scene, final character models, animation rig, audio, baked NavMesh and device testing are still required.
