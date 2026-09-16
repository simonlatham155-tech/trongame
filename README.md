# L4th4m / TRON-style Arena Game

This repository is the clean rebuild baseline for L4th4m.

## Source of truth

The old GitHub repository was empty. The current game and character archives supplied in September 2026 are the asset baseline.

The project is being rebuilt around two principles:

1. Preserve the existing 40 x 40 competitive grid so gameplay balance remains stable.
2. Replace simple perimeter-wall arenas with recognisable venue architecture and a proper character-to-lightcycle opening sequence.

## Opening sequence

The intended opening is:

Character enters arena -> approaches lightcycle -> plays the supplied jump/mount animation -> lightcycle activates -> camera hands over to gameplay -> countdown -> player control enabled.

`OpeningSequenceDirector.cs` is deliberately asset-agnostic. Assign the supplied character Animator, jump/mount animation trigger, lightcycle, cinematic cameras and player controller in the Unity Inspector. This means the existing animation FBX can be used without baking asset-specific paths into code.

## Arena structure

`VenueShellBuilder.cs` builds architecture outside and around the 40 x 40 playfield while leaving the competitive grid clear.

Initial venue identities:

- Wembley: tiered seating bowl, tunnel opening, scoreboards and floodlight towers.
- Colosseum: repeated columns/arches and stepped stone seating.
- Madison Square Garden: steep enclosed seating, overhead truss and central display cube.

These are structural gameplay-ready shells, intended to be replaced or dressed with final art assets later without changing the grid logic.

## Binary assets

FBX models, textures, animation clips and other large Unity binary assets should be added under `Assets/Art/` and `Assets/Characters/`. The GitHub connector used for this rebuild can write source files but cannot upload the supplied binary FBX/texture archive directly, so those assets remain in the supplied project archives until copied into the Unity project.

## Next integration step

1. Open the current Unity project.
2. Add the scripts from `Assets/Scripts/L4th4m/`.
3. Add `OpeningSequenceDirector` to an empty scene object.
4. Wire the supplied jump-onto-lightcycle character Animator and animation trigger in the Inspector.
5. Add `VenueShellBuilder` to the arena root and choose a venue.
6. Keep the existing gameplay grid/collision logic as the inner arena.
