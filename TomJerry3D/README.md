# TomJerry3D — first-person Unity demo

A playable scene prototype: explore a connected house as a mouse in first person while a cartoon cat patrols, spots the player and chases. The scene is generated from Unity scripts and includes touch controls.

## Quick start
1. Create a Unity 3D project (Unity 6 recommended).
2. Copy `TomJerry3D/Scripts` into `Assets/TomJerry3D/Scripts`.
3. Copy `TomJerry3D/Editor/CreateTomJerryDemoScene.cs` into `Assets/Editor/`.
4. In Unity choose **Tools → Tom & Jerry 3D → Create Playable Demo Scene**.
5. Open `Assets/TomJerry3D/Scenes/TomJerryDemo.unity` and press Play.

Full steps and controls: [GAMEPLAY_SETUP.md](GAMEPLAY_SETUP.md).

## Included gameplay
- First-person mouse movement and camera look.
- Runtime Android HUD with movement joystick, swipe-look, objective/cheese counter and USE button.
- Start screen plus pause overlay with Resume and Restart controls.
- Procedurally generated connected house and runtime NavMesh.
- Procedurally animated cartoon cat and first-person Jerry paw, with patrol, sight detection, chase and search.
- Mission progression, generated sound cues and LOW / MED / HIGH graphics presets.
- Carry/drop items, kitchen fridge food dispenser and baitable mousetrap.
- Collectible cheese and cartoon prop chaos; Tom gets sent to the porch briefly after enough mess.

## Status
This is a playable prototype scene generator, not a finished commercial game. Final high-detail 3D assets, recorded audio/music, device testing and Android APK export remain to be done. Lightweight procedural animation and generated sound cues are included. The project uses C# for Unity gameplay scripts; Kotlin is only suitable for separate native Android modules.
