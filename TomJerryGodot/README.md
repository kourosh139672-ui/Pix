# Tom & Jerry 3D — Godot 4 browser-first project

The project lives in this folder and is maintained through GitHub. No game editor needs to be installed on the phone for the GitHub-based workflow.

## Play without installing a game editor

A GitHub Actions workflow is configured at `.github/workflows/tom-jerry-godot-web.yml`. When files in this project change, GitHub's cloud runner attempts to export a browser build. If GitHub Pages is enabled for this repository with the GitHub Actions source, the workflow also publishes the web build.

The browser build is intended to run on a compatible modern browser. It is not an Android APK.

## Current prototype features

- Procedurally generated small house, kitchen counter, fridge, dining table and rug.
- First-person player controller.
- Touch movement on the left half and swipe-look on the right half.
- Keyboard/mouse fallback for desktop testing.
- Cartoon-style placeholder Tom model and a first-person Jerry hand.
- Tom pursuit behavior, collectible cheese, baitable trap and simple win/lose feedback.
- Restart and mission HUD.

## Honest status

- The Godot project files, web export preset, and cloud workflow are committed.
- The cloud workflow has to run successfully before a playable web link or downloadable build exists.
- The project has not yet been validated by a successful Godot engine run. The current visuals use procedural placeholder geometry, not final character models or professional animation.
- No APK is currently available. The current no-install target is a browser build.
