# Tom & Jerry 3D — Godot 4

A browser-first 3D prototype maintained through GitHub. No editor needs to be installed on the phone.

## Current prototype
- Procedural starter house with kitchen, dining area, sofa, cupboard, window and furniture.
- First-person Jerry controller with touch movement and swipe camera.
- Cartoon placeholder Tom with pursuit AI that speeds up as cheese is collected.
- Three cheese collectibles, two snack pickups, baitable mousetrap, win/lose states and restart.
- Mobile HUD and keyboard/mouse fallback.

## Build without installing on the phone
The workflow at `.github/workflows/tom-jerry-godot-web.yml` attempts a Web export after project changes. The Godot Web preset is stored in `export_presets.cfg`.

For public publishing, enable **Settings → Pages → Build and deployment → Source: GitHub Actions**. A successful Actions run is required before a playable Pages URL exists.

## Honest status
- Project source, Web export preset and cloud workflow are committed.
- This remains a procedural prototype; characters and props are not final authored models or professional animations.
- The workflow and game have not yet been confirmed by a successful run in this session.
- The target is a browser build, not an Android APK.
- See `GITHUB_ONLY_WORKFLOW_FA.md` for the Persian no-install instructions.
