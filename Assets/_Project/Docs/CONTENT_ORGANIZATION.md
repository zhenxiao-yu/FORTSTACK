# Content Organization

## Scripts

Project gameplay scripts belong under `Assets/_Project/Scripts/Runtime/`.

- Core: bootstrap, game director, save/load, time, global state, shared utilities.
- Cards: card definitions, instances, stacks, settings, feel, view, and card components.
- Crafting: recipes, crafting manager, crafting tasks, and recipe matching.
- Colony: colony stats, morale, resources, population, and survival state.
- Combat: combat manager, combat tasks, stats, hit resolution, and targeting.
- Night: night phase, lanes, waves, enemy definitions, and deployment UI.
- Packs: pack definitions, opening, slots, and pack presentation.
- Quests: quest definitions, instances, progress, and quest views.
- Trading: buyers, vendors, trade zones, and trade manager code.
- UI: HUD, menus, buttons, tooltips, info panels, and card stat UI.
- Input: input managers and input action wrappers.
- Localization: runtime localization helpers and bridges.
- Audio: audio managers, audio data, music, and SFX services.

Editor-only tools belong under `Assets/_Project/Scripts/Editor/`. Existing EditMode tests in `Assets/_Project/Scripts/Tests/Editor/` are part of the `_Project.Editor` assembly. New structured tests belong under `Assets/_Project/Tests/` — see below.

## Prefabs

Project-owned prefabs belong under `Assets/_Project/Prefabs/`.

- Cards: card visual prefabs.
- UI: menus, HUD, modals, overlays, and reusable UI prefabs.
- Combat: combat-specific prefabs.
- Systems: persistent or manager-style prefabs.
- Effects/VFX: particles, projectiles, and presentation effects.
- Trading: buyers, vendors, and trade-zone prefabs.

## ScriptableObjects

Project-owned authored data belongs under `Assets/_Project/Data/`.

Current runtime content uses Unity `Resources.LoadAll`, so live card, recipe, pack, quest, and encounter data is organized under `Assets/_Project/Data/Resources/`. Keep the folder segment named `Resources` until a deliberate data-loading migration replaces those calls.

## Art And Audio

Project-owned art belongs under `Assets/_Project/Art/`.

- Card and pack art: `Assets/_Project/Art/Sprites/`
- UI art: `Assets/_Project/Art/UI/`
- VFX sprites/textures: `Assets/_Project/Art/VFX/`
- Backgrounds and tiles: `Assets/_Project/Art/Backgrounds/`
- Materials and shaders: `Assets/_Project/Materials/`

Project-owned audio belongs under `Assets/_Project/Audio/`.

- Music: `Assets/_Project/Audio/Music/`
- SFX: `Assets/_Project/Audio/SFX/`
- Mixers: `Assets/_Project/Audio/Mixers/`

## Third Party

Imported third-party assets belong under `Assets/ThirdParty/`. Do not place project gameplay scripts inside third-party folders, and do not modify package internals during ordinary project cleanup.

Local UPM packages remain under `Packages/`.

## Tests

Structured NUnit tests are organized in two assemblies:

- EditMode tests: `Assets/_Project/Tests/EditMode/` — assembly `_Project.Tests.EditMode`. Use `AssetDatabase` and editor-only APIs. Validates data assets and pure logic.
- PlayMode tests: `Assets/_Project/Tests/PlayMode/` — assembly `_Project.Tests.PlayMode`. Tests runtime component lifecycle. No editor APIs.

The older tests under `Assets/_Project/Scripts/Tests/Editor/` remain valid and compile under `_Project.Editor`.

## Sandbox Scenes

Development-only scenes for isolated feature testing belong under `Assets/_Project/Scenes/Test/`. Never add these to Build Settings. See `Assets/_Project/Scenes/Test/README.md` for per-scene setup.

## Dev-Only Editor Tools

Editor tools that are not part of a custom inspector belong under `Assets/_Project/Scripts/Editor/`:

- `ProjectValidator.cs` — full project audit, menu `Tools/LAST KERNEL/Validate Project`
- `QuickFixTools.cs` — safe non-destructive fixes, menu `Tools/LAST KERNEL/Fix Safe Issues`
- `DevSpawnerWindow.cs` — Play Mode card spawner, menu `Tools/LAST KERNEL/Dev Spawner`

Runtime-only debug tools (compiled out in release) belong under `Assets/_Project/Scripts/Runtime/UI/`:

- `DebugOverlay.cs` — in-game overlay, guarded by `#if UNITY_EDITOR || DEVELOPMENT_BUILD`

## Adding Content

Add new cards under `Assets/_Project/Data/Resources/Cards/` while card loading still uses `Resources.LoadAll("Cards")`.

Add new recipes under `Assets/_Project/Data/Resources/Recipes/`.

Add new packs under `Assets/_Project/Data/Resources/Packs/`.

Add new quests under `Assets/_Project/Data/Resources/Quests/`.

Add new UI scripts under `Assets/_Project/Scripts/Runtime/UI/` and UI prefabs under `Assets/_Project/Prefabs/UI/`.

Add new art under `Assets/_Project/Art/` and new audio under `Assets/_Project/Audio/`. Preserve `.meta` files when moving or renaming assets.
