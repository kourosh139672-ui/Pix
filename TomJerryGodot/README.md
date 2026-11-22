# Tom & Jerry 3D — Godot mobile prototype

This is the new Godot 4 / GDScript port of the Tom & Jerry first-person mobile game. It is separate from the legacy Unity prototype in the TomJerry3D folder.

## Open on an Android phone

1. Install the official Godot Engine Android Editor while online.
2. Download/install the matching Godot export templates before going offline. Keep the editor and template versions identical.
3. Download this repository as a ZIP (or clone it with a Git client) and extract the TomJerryGodot folder into device storage.
4. Open Godot Android Editor → Import → select TomJerryGodot/project.godot.
5. Open the project and run the Main scene. Use the left half of the screen to move and the right half to look.
6. For APK export, create/select an Android export preset and use the matching prebuilt Android template. If using Gradle/custom builds, Godot's Android Build Environment (GABE) may also be required.

## Offline expectations

- After the editor, project files, and matching export templates are present on the phone, ordinary editing and non-Gradle export can work without a network connection.
- The first download/import of the editor and export templates requires internet.
- Android editor features vary by Godot version. The Android editor uses GDScript; it does not support C#/Mono.
- The Android editor has device/performance limitations. Use the Compatibility renderer and test on the actual phone.

## Current prototype features

- Procedurally generated small house, kitchen counter, fridge, dining table and rug.
- First-person player controller.
- Touch movement on the left half and swipe-look on the right half.
- Keyboard/mouse fallback for desktop testing.
- Tom pursuit behavior, collectible cheese, baitable trap and a simple win condition.
- Restart and mission HUD.

## Important status

This is an initial Godot port/scaffold, not a complete conversion of all Unity gameplay systems. It uses simple placeholder geometry and basic AI. It has not yet been opened, run, profiled or exported in Godot in this repository workflow, so no APK is included yet.
