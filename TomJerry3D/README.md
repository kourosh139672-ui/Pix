# TomJerry3D — Android game prototype

A Unity 3D first-person stealth/chase prototype. The player controls a small mouse exploring a house, collecting cheese and avoiding a roaming cat.

## Planned features
- First-person mouse controller
- Indoor house level: kitchen, living room, bedroom, hallway and storage room
- Cat AI: patrol, detect, chase and search
- Cheese collectibles and a simple counter
- Traps that briefly stun the cat
- Android-friendly controls and URP graphics

## Unity setup
1. Install a Unity 6 LTS editor on a computer or a supported cloud build machine.
2. Create a 3D URP project.
3. Copy the contents of this folder into the project's Assets/TomJerry3D folder (or move the Scripts folder into Assets).
4. Add a NavMesh Surface to the level, bake the NavMesh, and assign the player and cat references in the Inspector.
5. Add a CharacterController to the player and a NavMeshAgent to the cat.
6. Configure Android Build Support to build an APK.

## Current status
This repository contains starter C# scripts only. It does not yet contain a Unity scene, 3D models, animations, audio, baked NavMesh, or an APK. Those require Unity Editor and suitable assets/build tools.

## Controls
The movement script supports keyboard input in the Unity Editor and public methods that can be connected to on-screen Android joystick/buttons.

## Original assets
Use original or properly licensed character models, textures, sounds and animations. This prototype uses generic mouse/cat roles and does not include official Tom and Jerry assets.
