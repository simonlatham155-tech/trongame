# L4th4m Wembley opening setup

This scene is intended to preserve the supplied **jumping onto lightcycle** character animation as the opening, then hand control to the existing lightcycle gameplay.

## 1. Wembley shell

1. Create an empty GameObject named `WembleyArena` centred on the existing 40 x 40 gameplay grid.
2. Add `WembleyArenaBuilder`.
3. Assign stadium/stand/emissive/tunnel materials.
4. Leave `gridSize` at 40 unless the gameplay grid itself changes.
5. Use **Rebuild Wembley** from the component context menu.

The generated stadium sits outside the gameplay footprint. It deliberately uses split seating sections and openings rather than four continuous walls.

## 2. Opening character

Import the character FBX and the supplied **jumping onto lightcycle** animation from the character pack into the Unity project. Use the same Humanoid avatar for the character and mount animation when possible.

Create an Animator state for the mount clip and transition into it using a Trigger parameter named:

`MountLightcycle`

The trigger name can be changed in `OpeningSequenceDirector` if the Animator already uses another parameter.

## 3. Animation events

For reliable synchronisation, add these Animation Events to the mount clip:

- `CycleMountPoint` at the frame where the character first reaches/contacts the lightcycle.
- `MountAnimationFinished` on the final seated frame.

Both functions are public methods on `OpeningSequenceDirector`.

If the animation file cannot accept events directly because it is an imported read-only FBX clip, duplicate/extract the clip inside Unity first and add the events to the extracted `.anim` asset.

## 4. Scene director

Create an empty GameObject named `OpeningDirector` and add `OpeningSequenceDirector`.

Assign:

- Character Animator
- Character root
- Lightcycle root
- Cinematic camera
- Gameplay camera
- Existing movement/trail scripts in `gameplayScripts`
- Countdown UI Text
- Lightcycle glow/trail effects in `cycleEffects`

Enable `useAnimationEvents`.

## 5. Intended sequence

1. Wembley is visible around the arena, with the tunnel framing the entrance.
2. Character appears/runs into the arena.
3. Mount trigger starts the supplied jump-on-lightcycle animation.
4. `CycleMountPoint` energises/reveals the bike at the exact contact frame.
5. `MountAnimationFinished` completes the cinematic.
6. Camera switches behind the lightcycle.
7. 3 - 2 - 1 - GO.
8. Existing gameplay scripts are enabled.

Do not replace the mount animation with a generic spawn. It is part of the intended L4th4m presentation.
