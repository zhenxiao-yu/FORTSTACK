# Development Tools

This document describes all developer tooling for LAST KERNEL. These tools are for development use only and do not affect shipped game behavior.

---

## Project Validator

**Menu:** `Tools/LAST KERNEL/Validate Project`

**File:** `Assets/_Project/Scripts/Editor/ProjectValidator.cs`

Runs a full structural audit of the project and logs all issues to the Unity Console. The validator never modifies any data.

### Validation categories

| Category | What it checks |
|---|---|
| Cards | IDs unique, displayName, description, art texture, category, sell price, combat stats for combat cards |
| Recipes | IDs unique, has inputs, has output, craftingDuration > 0 |
| Packs | Has slots, slot cards not null, buyPrice > 0 |
| Quests | IDs unique, title and description not empty, targetAmount > 0 |
| Prefabs | Missing scripts, missing object references, CardInstance prefab has CardController / Collider / CardFeelPresenter / VisualRoot |
| Scenes | Boot/MainMenu/Game are in Build Settings; sandbox scenes are not in Build Settings; Boot scene has no unexpected root objects |
| Localization | English and Chinese locales exist; no duplicate keys; no empty English translations; all cards/recipes/quests/packs have localization key entries |
| Audio | AudioManager prefab exists and has component; mixer groups assigned; no null SFX clips; no duplicate AudioId entries; all AudioId enum values registered |
| Code Ownership | All project scripts use the `Markyu.LastKernel` namespace |

### Output

- Errors (red): broken gameplay — must fix before shipping
- Warnings (yellow): non-critical — review before shipping
- Summary line at the end: `Errors: N, Warnings: N`

---

## Safe Fix Tools

**Menu:** `Tools/LAST KERNEL/Fix Safe Issues`

**File:** `Assets/_Project/Scripts/Editor/QuickFixTools.cs`

Applies a conservative set of non-destructive fixes. It never deletes, renames, or moves assets, and never changes gameplay data.

### What it does

- Creates any missing required folders (`Scenes/Test`, `Tests/EditMode`, `Tests/PlayMode`, `Docs`, `Data/Resources/*`)
- If exactly one `CardFeelProfile` exists in the project, assigns it to selected `CardSettings` assets that are missing a feel profile
- Adds `CardFeelPresenter` to any selected GameObject that has `CardInstance` but no presenter
- Adds `CardController` to any selected GameObject that has `CardInstance` but no controller

### What it does NOT do

It does not auto-fix every validator warning. Items requiring judgement (art selection, recipe balance, localization content) must be fixed manually.

---

## Dev Spawner

**Menu:** `Tools/LAST KERNEL/Dev Spawner`

**File:** `Assets/_Project/Scripts/Editor/DevSpawnerWindow.cs`

An EditorWindow for spawning and manipulating cards during Play Mode. Guarded by `#if UNITY_EDITOR || DEVELOPMENT_BUILD` — never active in release builds.

### Setup

1. Open the window via `Tools/LAST KERNEL/Dev Spawner`.
2. Assign a **Card Prefab** (a prefab with `CardInstance`) and a **Card Settings** asset.
3. Enter Play Mode.

### Features

| Feature | How |
|---|---|
| Spawn a specific card | Select from the card list, set count, click Spawn |
| Spawn by category | Use preset group buttons (Resources / Materials / Mobs) |
| Damage selected card | Select a card GameObject → click "Damage Selected Card" |
| Heal selected card | Select a card GameObject → click "Heal Selected Card" |
| Switch language | Click "Switch to English" or "Switch to Simplified Chinese" |
| Clear spawned cards | Click "Clear Spawned Cards" — only removes cards this window spawned |

Day/Night transition is a stub — wire it to your `DayNightCycle` or `GameDirector` in the method `TriggerDayNightTransition()`.

---

## Debug Overlay

**File:** `Assets/_Project/Scripts/Runtime/UI/DebugOverlay.cs`

An in-game IMGUI overlay visible only in `UNITY_EDITOR || DEVELOPMENT_BUILD`. Add it to any persistent GameObject (e.g., a `DebugTools` object in your Game scene or Boot scene).

### Toggle

Press `F1` or `` ` `` (backtick) to show/hide.

### What it shows

- FPS (sampled every 0.5 s)
- Card count and stack count (live `FindObjectsByType`)
- Active crafting and combat task counts
- Current game language
- Hovered and dragged card name

### Adding to a scene

1. Create an empty GameObject, name it `DebugOverlay`.
2. Add the `DebugOverlay` component.
3. The component is compiled out in non-development builds automatically.

---

## Sandbox Scenes

**Location:** `Assets/_Project/Scenes/Test/`

See [Scenes/Test/README.md](../Scenes/Test/README.md) for per-scene setup details.

| Scene | Purpose |
|---|---|
| `Sandbox_Cards.unity` | Card spawning, drag/drop, feel, damage feedback |
| `Sandbox_Crafting.unity` | Recipe detection, crafting timers, pause/resume |
| `Sandbox_Combat.unity` | Player vs mob, damage, death, loot |
| `Sandbox_Localization.unity` | Language switching, UI text update verification |
| `Sandbox_UI.unity` | HUD, tooltips, scaling, pixel-perfect, mobile aspect ratios |

**Rule:** Never add sandbox scenes to Build Settings. The `ProjectValidator` checks and errors if any are found there.

---

## Running Tests

### EditMode Tests

**Location:** `Assets/_Project/Tests/EditMode/`

**Assembly:** `_Project.Tests.EditMode`

These tests run without entering Play Mode. They validate data assets using `AssetDatabase` and test pure logic classes.

To run:
1. Open `Window → General → Test Runner`
2. Select the **EditMode** tab
3. Click **Run All** or expand to run individual suites

| Test Class | What it validates |
|---|---|
| `CardDefinitionValidationTests` | Card ID uniqueness, displayName, art, combat stats |
| `RecipeValidationTests` | Recipe IDs, inputs, outputs, duration; RecipeMatcher logic |
| `PackValidationTests` | Pack slots, null cards, buyPrice |
| `LocalizationValidationTests` | Locale presence, duplicate keys, empty English translations |
| `AudioValidationTests` | Prefab exists, mixer groups, null clips, duplicate AudioId entries |

### PlayMode Tests

**Location:** `Assets/_Project/Tests/PlayMode/`

**Assembly:** `_Project.Tests.PlayMode`

These tests require the Unity runtime. They test component lifecycle and runtime APIs.

To run:
1. Open `Window → General → Test Runner`
2. Select the **PlayMode** tab
3. Click **Run All**

| Test Class | What it validates |
|---|---|
| `CardSpawnTests` | CardInstance can be added; defaults to not dragging; Definition is null before Initialize |
| `CardInteractionTests` | IsBeingDragged get/set; Stack is null when unattached |
| `CraftingFlowTests` | RecipeMatcher empty list; RecipeMatcher match; CombatStats creation |
| `CombatFlowTests` | CombatStats not null; MaxHealth positive; CurrentHealth zero before Initialize |
| `LocalizationRuntimeTests` | SetLanguage changes CurrentLanguage; GetOptional returns fallback for unknown keys |

---

## Pre-Commit Checklist

Before committing, run these checks manually or via CI:

- [ ] `Tools/LAST KERNEL/Validate Project` — zero errors (warnings are OK if reviewed)
- [ ] EditMode tests pass in Test Runner
- [ ] PlayMode tests pass in Test Runner
- [ ] No sandbox scenes in Build Settings (Validator checks this)
- [ ] New cards have art, category, localization keys
- [ ] New recipes have inputs, output, duration > 0, localization key
- [ ] No missing script references in prefabs
- [ ] Audio mixer groups assigned if AudioManager was touched
