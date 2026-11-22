# Tom & Jerry 3D — create and play the demo

This is a Unity C# prototype. The included Editor command creates a saved scene; the scene builds its house and gameplay objects when you press Play.

## Create the scene
1. Create a new Unity 3D project (Unity 6 recommended).
2. Copy the contents of this repository's `TomJerry3D/Scripts` folder into your Unity project's `Assets/TomJerry3D/Scripts` folder.
3. Copy `TomJerry3D/Editor/CreateTomJerryDemoScene.cs` into `Assets/Editor/CreateTomJerryDemoScene.cs`.
4. Wait for Unity to compile. In the Unity menu choose **Tools → Tom & Jerry 3D → Create Playable Demo Scene**.
5. Open `Assets/TomJerry3D/Scenes/TomJerryDemo.unity` if it did not open automatically, then press **Play**.

## What the demo builds
- Connected house layout with living room, kitchen, hallway, bedroom, storage and a front porch.
- First-person mouse camera and CharacterController.
- Android HUD created at runtime: left joystick, right swipe-to-look area, objective instructions, cheese counter, USE button, start screen and pause menu.
- The objective HUD updates when Jerry collects cheese or carries food toward the mousetrap.
- Start screen pauses the game until PLAY is tapped. PAUSE opens a pause overlay with RESUME and RESTART; Escape toggles pause in the Unity Editor.
- Procedural cartoon Tom with procedural walk bounce, body sway, head and tail animation, plus patrol/chase/search AI. Jerry's first-person paw also has a small idle animation.
- Mission progression: collect three cheese pieces, bait the mousetrap, and receive a completion message.
- Generated cartoon sound cues for collecting cheese and arming the trap.
- On-screen LOW / MED / HIGH graphics presets with frame-rate and shadow-distance adjustments.
- Runtime NavMesh built from the house colliders so Tom can navigate without a separate bake step.
- Kitchen fridge that opens and dispenses carryable cheese.
- Collectible cheese, a baitable mousetrap and four harmless breakable props.
- Tom's cartoon chaos routine: after enough props are knocked over, an owner-scolding event fires and Tom takes a short trip onto the porch before returning.

## Controls
- Android/touch: tap **PLAY** on the start screen; left joystick to move; drag on the right half to look; **USE** to pick up/drop food, open the fridge, or place carried food into the mousetrap; **PAUSE** opens the pause menu.
- Unity Editor: WASD/arrow keys and mouse movement to move/look; **E** to interact.

## Important setup notes
- Use a Unity project with the built-in UI (uGUI) and legacy Input Manager enabled. The runtime HUD uses `StandaloneInputModule`.
- The demo still uses procedural primitive character and environment art, not final high-detail imported models. Tom now has lightweight procedural motion; generated UI sound cues are included, but recorded voice acting, a music soundtrack and finished effects are not.
- The project scripts are C#, because Unity gameplay scripts run in C#. Kotlin can be used for separate native Android integrations, not as the Unity scene scripting language.
- Android APK export still requires Android Build Support in Unity and a build from Unity Editor. This repository does not contain a prebuilt APK.
