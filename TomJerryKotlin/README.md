# Tom & Jerry 3D — Kotlin edition

This is the Kotlin/LibGDX Android implementation. The game code lives in `core/src/main/kotlin`; the Android launcher is also Kotlin.

## Build
The GitHub Actions workflow builds a debug APK without requiring Android Studio on the user's phone. It uses Java 17, Gradle 8.9, Android Gradle Plugin 8.7.3, Kotlin 2.0.21 and LibGDX 1.13.5.

## Current prototype
- Landscape, offline Android app
- First-person mouse movement and camera look
- Touch controls and keyboard controls for desktop testing
- Tom follows Jerry
- Cheese pickups and a simple trap interaction
- Basic 3D kitchen/living-room blockout

The project is an initial playable foundation, not a finished high-detail commercial game. Character models, polished animations, sound design and full scene progression remain to be added.
