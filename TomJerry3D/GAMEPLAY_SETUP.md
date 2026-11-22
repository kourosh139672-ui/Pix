# Tom & Jerry 3D — create, play and export the demo

This is a Unity C# prototype. The Editor command creates a saved scene; the scene builds its house and gameplay objects when you press Play.

## Create the scene
1. Create a new Unity 3D project (Unity 6 recommended).
2. Copy the contents of this repository's `TomJerry3D/Scripts` folder into your Unity project's `Assets/TomJerry3D/Scripts` folder.
3. Copy the contents of `TomJerry3D/Editor` into `Assets/Editor/`.
4. Wait for Unity to compile. In the Unity menu choose **Tools → Tom & Jerry 3D → Create Playable Demo Scene**.
5. Open `Assets/TomJerry3D/Scenes/TomJerryDemo.unity` if it did not open automatically, then press Play.

## What the demo builds
- Connected house layout with living room, kitchen, hallway, bedroom, storage and a front porch.
- Generated decorative details: framed windows, wall pictures, a dining table and chairs, indoor plants and ceiling beams.
- First-person mouse camera and CharacterController.
- Android HUD created at runtime: left joystick, right swipe-to-look area, objective instructions, cheese counter, USE button, start screen and pause menu.
- Objective HUD updates when Jerry collects cheese or carries food toward the mousetrap.
- Start screen pauses the game until PLAY is tapped. PAUSE opens a pause overlay with RESUME and RESTART; Escape toggles pause in the Unity Editor.
- Procedural cartoon Tom and Jerry models; Tom now has an alternating arm/leg walk cycle, body bounce, head and ear motion, a segmented tail wave and occasional blinking, alongside patrol/chase/search AI.
- Mission progression: collect three cheese pieces, bait the mousetrap, and receive a completion message.
- Generated sound cues for collecting cheese and arming the trap.
- On-screen LOW / MED / HIGH graphics presets with frame-rate and shadow-distance adjustments; the selected preset is saved between launches.
- GFX panel master-volume slider; the audio level is saved between launches and applies to generated game sound cues.
- Runtime NavMesh built from the house colliders.
- Kitchen fridge that opens and dispenses carryable cheese.
- Collectible cheese, a baitable mousetrap and harmless breakable props.
- Tom's cartoon chaos routine: after enough props are knocked over, an owner-scolding event fires and Tom takes a short trip onto the porch before returning.

## Controls
- Android/touch: tap **PLAY**; left joystick to move; drag on the right half to look; **USE** to pick up/drop food, open the fridge, or place carried food into the mousetrap; **PAUSE** opens the pause menu.
- Unity Editor: WASD/arrow keys and mouse movement to move/look; **E** to interact.

## Export an Android APK
1. Install **Android Build Support** in Unity Hub, including Android SDK & NDK Tools and OpenJDK.
2. In Unity, open **File → Build Settings**, select Android and add `Assets/TomJerry3D/Scenes/TomJerryDemo.unity` to **Scenes In Build**.
3. Copy `TomJerry3D/Editor/TomJerryAndroidBuild.cs` into `Assets/Editor/` and wait for compilation.
4. Run **Tools → Tom & Jerry 3D → Validate Project**. Resolve any missing scene or folder reported in the Console.
5. Run **Tools → Tom & Jerry 3D → Build Android APK**. If successful, the APK is written to `Builds/Android/TomJerry3D.apk`.

The build helper sets landscape orientation, IL2CPP and ARM64. Android export still requires a valid Unity installation, license and Android toolchain. A failed build must be diagnosed from the Unity Console.

## Important limitations
- The character and environment art are procedural primitive prototypes, not final high-detail imported meshes or skinned character rigs.
- Motion is lightweight procedural animation. The included audio cues are generated tones, not recorded voice acting, soundtrack or final sound effects.
- This repository workflow cannot launch Unity Editor, compile the project or test on a physical phone. Treat the APK as unbuilt until the Android build completes successfully in Unity.
- Unity gameplay scripts are C#. Kotlin can be used for separate native Android integrations, not as the Unity scene scripting language.
