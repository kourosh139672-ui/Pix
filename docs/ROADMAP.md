# PixPhone roadmap

1. Choose a supported Android base (AOSP-derived ROM or existing device-compatible platform).
2. Define device target and boot/build environment.
3. Design launcher, status bar, notification shade, and settings.
4. Ensure package installer and APK runtime are provided by the Android base.
5. Test builds and recovery/rollback on supported hardware.

## Non-goals
- No integrated AI assistant.
- No bundled app store.

## Safety and feasibility
Building a bootable Android system requires device-specific sources, vendor components, kernel/device tree, signing, and a build environment. A generic repository alone cannot provide a universal APK-installing OS.