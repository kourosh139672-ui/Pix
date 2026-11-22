# Tom & Jerry 3D — Godot 4

A mobile-first 3D prototype maintained through GitHub. No editor needs to be installed on the phone.

## Current prototype
- Procedural starter house with kitchen, dining area, sofa, cupboard, window and furniture.
- First-person Jerry controller with touch movement and swipe camera.
- Cartoon placeholder Tom with pursuit AI that speeds up as cheese is collected.
- Three cheese collectibles, two snack pickups, baitable mousetrap, win/lose states and restart.
- Mobile HUD and keyboard/mouse fallback.
- Original synthesized sound effects (cheese, snacks, footsteps, fridge, trap, Tom alert, win/lose) and a looping original melody, generated locally by the game so no online audio files are needed.

## Android APK build
The workflow `.github/workflows/tom-jerry-godot-android.yml` exports a debug APK on GitHub Actions. Open **Actions → Tom & Jerry Godot Android APK**, then download the `TomJerry3D-Android-APK` artifact after a successful run. The APK is intended for offline play and does not request Internet permission.

## Web build
The separate workflow `.github/workflows/tom-jerry-godot-web.yml` exports a browser build. The target release for this project is the Android APK, not the browser.

## Honest status
- Project source, Web and Android export presets, and cloud workflows are committed.
- Android APK must still pass a successful GitHub Actions export before it can be called ready.
- The game is a procedural prototype; characters and props are not final authored models or professional animations.
- Audio is synthesized original game audio, not official Tom & Jerry recordings.
- The APK has not yet been installed or tested on a physical phone.
