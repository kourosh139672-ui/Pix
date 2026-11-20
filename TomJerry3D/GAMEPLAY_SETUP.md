# Gameplay additions: carrying, fridge, bait and Tom's cartoon chaos

These scripts are prototype gameplay systems. They are committed to the repository, but still need to be attached and configured in a Unity scene.

## 1. Let Jerry and Tom carry things
- Add a child empty GameObject named `HandSocket` to each character, positioned in front of the hand.
- Add `CharacterHandPickup` to Jerry and Tom.
- Assign each character's camera/eye transform to `View` and its `HandSocket` to `Hand Socket`.
- Food and objects that can be carried need a collider and `CarryableItem`.
- Aim at an item and press E. On mobile, connect a UI button to `CharacterHandPickup.Interact()`. Press E again to drop it.

## 2. Add the kitchen fridge
- Add `KitchenFridgeBuilder` to an empty object in the kitchen.
- Adjust its `Position` in the Inspector if needed.
- It builds a fridge with a door and a cheese item dispenser. Aim at the fridge and press E to open/close it and spawn food.

## 3. Put food in the mousetrap
- Add a trigger collider to the trap object.
- Add `MouseTrapBaitSocket` to the trap and assign Tom's `CatAI`.
- Add a child empty transform named `BaitSocket` and assign it to the trap's Bait Socket field.
- With food held, aim at the trap and call `MouseTrapBaitSocket.PlaceHeldBait(CharacterHandPickup)` from a mobile UI button. This places the held food in the trap.
- Tom only gets caught while bait is installed.

## 4. Tom makes a cartoon mess
- Add `TomHouseChaos` to Tom (which needs a `NavMeshAgent`).
- Create an empty transform outside the house and assign it to `Outside Point`.
- Put `BreakableProp` on harmless, lightweight furniture/decor props that Tom can bump into.
- When Tom runs into enough props, the owner scolds him through the `On Owner Scolds` UnityEvent and sends him outside for a short timeout. He then returns. This is slapstick game logic, not realistic destruction.

## Current limitations
- The scripts do not create a finished Unity scene, animation rig, mobile HUD, or Android APK.
- NavMesh must be baked in Unity for Tom's navigation.
- Connect the public methods to on-screen buttons for touch controls.
- Use original or properly licensed 3D assets for final character and furniture models.
