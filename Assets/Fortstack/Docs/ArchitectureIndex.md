# Last Kernel — Architecture Index

**Unity:** 6000.4.3f1 · **Pipeline:** URP 17.4.0 · **Namespace:** `Markyu.FortStack`  
**Last updated:** 2026-04

---

## Table of Contents
1. [Folder Structure](#1-folder-structure)
2. [Major Systems](#2-major-systems)
3. [Data Flow Overview](#3-data-flow-overview)
4. [Card System](#4-card-system)
5. [UI Architecture](#5-ui-architecture)
6. [Camera Setup](#6-camera-setup)
7. [Localization](#7-localization)
8. [Save / Load System](#8-save--load-system)
9. [Input Handling](#9-input-handling)
10. [Where to Add New Content](#10-where-to-add-new-content)
11. [Coding Conventions](#11-coding-conventions)
12. [Known Risks & Refactoring Notes](#12-known-risks--refactoring-notes)

---

## 1. Folder Structure

```
Assets/Fortstack/
├── Docs/                    ← Architecture docs (you are here)
├── Fonts/                   ← TMP font assets (NotoSans, SmileySans)
├── Localization/
│   ├── Docs/                ← CSV source file for localization review
│   └── StringTables/        ← Unity Localization .asset string tables
├── Materials/               ← Shared materials (card shader, outline, etc.)
├── Prefabs/
│   ├── Cards/               ← Card prefabs by category
│   ├── Core/                ← Manager prefabs (GameDirector, AudioManager, etc.)
│   ├── Trading/             ← Trade zone prefabs
│   └── UI/                  ← ProgressUI, ScreenFader, etc.
├── Resources/
│   ├── Cards/               ← CardDefinition assets by category (loaded by catalog)
│   └── Recipes/             ← RecipeDefinition assets (loaded by CraftingManager)
├── Scenes/
│   ├── Title.unity          ← Main menu / save screen
│   ├── Main.unity           ← Primary gameplay (colony board)
│   └── Island.unity         ← Night phase / combat scene
├── Scripts/
│   ├── Card/                ← Card system (instances, stacks, definitions, VFX)
│   ├── Combat/              ← Combat tasks, rules, UI
│   ├── Core/                ← Singleton managers and shared infrastructure
│   ├── Crafting/            ← Crafting tasks, recipe matching, definitions
│   ├── Encounter/           ← Encounter events
│   ├── Editor/              ← Custom inspectors and editor utilities
│   ├── Extensions/          ← Extension methods (VectorExtensions, etc.)
│   ├── Night/               ← Night phase / lane combat
│   ├── Pack/                ← Pack / booster slot system
│   ├── PostProcess/         ← Custom URP post-process features
│   ├── Quest/               ← Quest instances and manager
│   ├── SaveSystem/          ← JSON save / load utilities
│   ├── Stats/               ← Stat types, modifiers, interfaces
│   ├── Trading/             ← Trade zones, card buyer, board expansion
│   └── UI/                  ← All UI MonoBehaviours
└── Settings/
    ├── Default_Card_Settings.asset   ← CardSettings ScriptableObject
    ├── Default_Camera_Settings.asset ← CameraSettings ScriptableObject (create via right-click)
    └── URP/                          ← URP pipeline and renderer assets
```

---

## 2. Major Systems

### GameDirector (`Scripts/Core/GameDirector.cs`)
**Role:** Root state machine for game flow. Persistent across scenes (`DontDestroyOnLoad`).  
**Owns:** `GameData` (active session), `SavedGames` dictionary, incoming travelers list.  
**Key events:** `OnSceneDataReady(SceneData, bool wasLoaded)`, `OnBeforeSave(GameData)`.  
**Entry points:** `NewGame(prefs)`, `LoadGame(data)`, `BackToTitle()`, `GameOver()`, `InitiateTravel(scenes, travelers)`.

### CardManager (`Scripts/Card/CardManager.cs`) ⚠️ God class
**Role:** Central registry and factory for all cards and stacks.  
**Owns:** All `CardStack` instances, card prefab lookup by category, discovery tracking.  
**Key events:** `OnStatsChanged(StatsSnapshot)`, `OnCardKilled(CardInstance)`.  
**Known issue:** 986 lines with multiple responsibilities. See §12 for the planned split.

### Board (`Scripts/Core/Board.cs`)
**Role:** Board grid state, world bounds, placement validation, blend-shape expansion.  
**Key event:** `OnBoundsUpdated(Bounds)` — consumed by `CameraController` to update pan limits.

### CraftingManager (`Scripts/Core/CraftingManager.cs`)
**Role:** Recipe catalog, active crafting tasks, progress UI lifecycle, recipe discovery.  
**Key events:** `OnRecipeDiscovered(string id)`, `OnCraftingFinished(RecipeDefinition)`.

### CombatManager (`Scripts/Combat/CombatManager.cs`)
**Role:** Active combat task registry, combat rect management, rule enforcement.

### QuestManager (`Scripts/Quest/QuestManager.cs`)
**Role:** Quest discovery, progress tracking, completion handling.  
**Key events:** `OnQuestActivated(QuestInstance)`, `OnQuestCompleted(QuestInstance)`.

### TradeManager (`Scripts/Trading/TradeManager.cs`)
**Role:** Vendor tracking, pack pricing, collection unlock, board expansion costs.

### AudioManager (`Scripts/Core/AudioManager.cs`)
**Role:** SFX pooling (8 sources, round-robin), BGM playback, volume persistence.  
**API:** `PlaySFX(AudioId, Vector3?, bool)`, `SetSFXVolume(float)`, `SetBGMVolume(float)`.

### InputManager (`Scripts/Core/InputManager.cs`)
**Role:** New Input System wrapper with lock-based disable. Multiple systems can independently disable input via `HashSet<object>` requesters.

### TimeManager (`Scripts/Core/TimeManager.cs`)
**Role:** `Time.timeScale` management with external-pause support.

### GameLocalization (`Scripts/Core/GameLocalization.cs`)
**Role:** Localization bridge. Wraps Unity Localization with a fast in-memory dictionary of 370+ pre-defined keys.  
**API:** `GameLocalization.Get(key)`, `GameLocalization.Format(key, args)`, `GameLocalization.GetOptional(key, fallback)`.  
**Event:** `GameLocalization.LanguageChanged` (static) — subscribe in `OnEnable`, unsubscribe in `OnDisable`.

### SaveSystem (`Scripts/SaveSystem/SaveSystem.cs`)
**Role:** Newtonsoft JSON serializer. Static utility.  
**Location:** `Application.persistentDataPath/SaveSlot{NNN}.json`.

---

## 3. Data Flow Overview

```
Title Screen
  └─ TitleScreen.cs
       ├─ NewGame  → GameDirector.NewGame(prefs)  → TravelSequence → Main.unity
       └─ LoadGame → GameDirector.LoadGame(data)  → TravelSequence → saved scene

Scene Load (Main / Island)
  └─ GameDirector.HandleSceneLoaded
       └─ OnSceneDataReady(SceneData, wasLoaded)
            ├─ CardManager.OnSceneDataReady  → restores stacks from SceneData
            ├─ CraftingManager.OnSceneDataReady
            ├─ QuestManager.OnSceneDataReady
            └─ (other managers subscribe to this event)

Save Game
  └─ GameDirector.SaveGame
       ├─ OnBeforeSave fired  → each manager writes its state into GameData
       ├─ RunStateManager.SyncToGameData
       └─ SaveSystem.SaveData<GameData>(GameData, fileName)
```

---

## 4. Card System

### Key Types
| Type | Kind | Role |
|------|------|------|
| `CardDefinition` | ScriptableObject | Static card data (stats, art, category, loot) |
| `CardStack` | Plain C# | Logical grouping of 1–N cards in world space |
| `CardInstance` | MonoBehaviour | Runtime card GameObject (state, visuals, movement) |
| `CardSettings` | ScriptableObject | Tunable physics, animation, and economy constants |
| `CardFeelProfile` | ScriptableObject | Per-category hover/spawn/damage feel tunables |
| `CardCombatant` | MonoBehaviour component | Combat participation for Character / Mob cards |
| `CardEquipper` | MonoBehaviour component | Equipment slot management for Character cards |
| `CardPhysicsSolver` | Static utility | Overlap resolution — iterative push-apart |
| `CardDefinitionCatalog` | Static lazy cache | `Resources.LoadAll<CardDefinition>` by category |

### Card Lifecycle
```
CardManager.CreateCardInstance(definition, position)
  → Instantiate prefab from category lookup
  → CardInstance.Initialize(definition, settings, stackToIgnore)
       → Cache components (Combatant, Equipper, FeelPresenter)
       → Create CardStack, register with CardManager
       → TryAttachToNearbyStack (Physics.OverlapSphere)
       → ResolveOverlaps
       → FeelPresenter.Initialize(feelProfile)  ← after art is ready
```

### Adding a New Card
1. Create `CardDefinition` asset: **Right-click > Last Kernel > Card Definition** (or subtype).
2. Place it in `Assets/Fortstack/Resources/Cards/{Category}/`.
3. Set a unique `id` (auto-generated on first save via `OnValidate`).
4. Assign an art texture and configure stats.
5. Ensure a card prefab for the category exists in `Prefabs/Cards/`.
6. If the card needs custom stacking rules, update `StackingRulesMatrix`.

---

## 5. UI Architecture

### Base Classes
- **`LocalizedUIBehaviour`** — Subscribe to `GameLocalization.LanguageChanged` and call `RefreshLocalizedText()`.
- **`MenuView`** — Abstract base for collapsible list views (QuestsView, RecipesView). Provides `CreateItemButton`, `ToggleInfoPanel`, `ToggleView`.

### Screen Hierarchy (Title scene)
```
TitleScreen (LocalizedUIBehaviour)
  ├─ TextButton: New Game → GameplayPrefsUI.Open()
  ├─ TextButton: Load     → SavedGamesUI.Open()
  ├─ TextButton: Options  → GameOptionsUI.Open()
  └─ TextButton: Quit     → ModalWindow.Show(confirm → Application.Quit)
```

### Gameplay UI
- `InfoPanel` — hover tooltip, priority-based display (cards, menu items).
- `ProgressUI` — floating progress bar above crafting stacks.
- `EquipmentPanel` — character equipment slots.
- `DayTimeUI` — day counter and cycle phase indicator.
- `PauseMenu` — pause overlay.
- `MenuView` subclasses — `QuestsView`, `RecipesView` with collapsible categories.

### Recommended Canvas Scaler (set manually in each scene)
Every root Canvas should have a `CanvasScaler` configured as:
- **UI Scale Mode:** Scale With Screen Size
- **Reference Resolution:** 1920 × 1080
- **Screen Match Mode:** Match Width Or Height — **Match: 0.5** (balanced)

See `Docs/PixelPerfectSetup.md` for full setup guidance.

### Adding a New UI Screen
1. Create a script inheriting `LocalizedUIBehaviour` (or plain `MonoBehaviour` if not localized).
2. Add a `CanvasGroup` for show/hide via alpha + `blocksRaycasts`.
3. Reference it from the appropriate parent controller (e.g., `TitleScreen`).
4. Hook into `GameLocalization.LanguageChanged` if the screen contains localized text.

---

## 6. Camera Setup

**Type:** 3D perspective/orthographic — NOT a 2D Pixel Perfect Camera.  
**Controller:** `CameraController.cs` — pan (drag), zoom (scroll wheel), smooth SmoothDamp.  
**Bounds source:** `Board.OnBoundsUpdated` drives the pan clamps at runtime.  
**Config asset:** `CameraSettings.cs` ScriptableObject — create via **Right-click > Last Kernel > Camera Settings** and save to `Settings/Default_Camera_Settings.asset`. See `Docs/PixelPerfectSetup.md`.

---

## 7. Localization

**System:** Hybrid — Unity Localization 1.5.11 + `GameLocalization.cs` bridge.  
**Supported languages:** Simplified Chinese, Traditional Chinese, English, Japanese, Korean, French, German, Spanish.

### Adding a Localized String
1. Add a row to `Assets/Fortstack/Localization/Docs/GameText_Localization_Source.csv`.
2. Run **Localization > Rebuild GameText Tables** in Unity to regenerate the `.asset` string tables.
3. Add the key to `GameLocalization.TextEntries` dictionary (for runtime fast-path lookup).
4. Use `GameLocalization.Get("your.key")` in code, or `GameLocalization.Format("your.key", args)` for parameterized strings.

### Language-Reactive UI
- MonoBehaviours: inherit `LocalizedUIBehaviour` and implement `RefreshLocalizedText()`.
- World-space objects (e.g. `PackVendor`): subscribe to `GameLocalization.LanguageChanged` in `Start`, unsubscribe in `OnDestroy`.

### String Table Files
| File | Purpose |
|------|---------|
| `GameText_en.asset` | English (primary authoring locale) |
| `GameText_zh-Hans.asset` | Simplified Chinese |
| `GameText_zh-Hant.asset` | Traditional Chinese |
| `GameText_ja.asset` | Japanese |
| `GameText_ko.asset` | Korean |
| `GameText_de.asset` | German |
| `GameText_fr.asset` | French |
| `GameText_es.asset` | Spanish |

**Important:** String table YAML must not have multi-line double-quoted values. Keep all `m_Localized:` values on a single line.

---

## 8. Save / Load System

**Serializer:** Newtonsoft JSON (`Newtonsoft.Json` 3.2.2).  
**Path:** `Application.persistentDataPath/SaveSlot{NNN}.json`.  
**Root object:** `GameData` — contains `SlotNumber`, `CurrentScene`, per-scene `SceneData` dictionary, discovered cards/recipes, `RunStateData`.

### Adding Persistent Data
1. Add a field to `GameData` or the relevant `SceneData`.
2. Subscribe to `GameDirector.OnBeforeSave` in your manager's `OnEnable`/`Start`.
3. Write your state into `GameData` when the event fires.
4. Read from `SceneData` when `GameDirector.OnSceneDataReady` fires.
5. Mark new fields `[JsonProperty]` if using non-default naming.

### Save Slot Naming
`SaveSlot001.json`, `SaveSlot002.json`, etc. Slots are never reused within a session.

---

## 9. Input Handling

**Package:** Unity Input System 1.19.0.  
**Wrapper:** `InputManager.cs` — singleton, exposes pointer position, buttons, scroll.  
**Locking:** `InputManager.RequestInputLock(requester)` / `ReleaseInputLock(requester)` using a `HashSet<object>`. Any locked requester disables all input.  
**Pause detection:** `InputManager.WasEscapePressedThisFrame()`.

---

## 10. Where to Add New Content

| Content Type | Location | Steps |
|---|---|---|
| New card | `Resources/Cards/{Category}/` | Create `CardDefinition` asset, assign art, set stats |
| New recipe | `Resources/Recipes/` | Create `RecipeDefinition` asset |
| New quest | `QuestManager` inspector | Add `QuestDefinition` to a `QuestGroup` |
| New pack / booster | `PackDefinition` asset | Define slots and entries, assign to `TradeManager` |
| New UI screen | `Scripts/UI/` | Inherit `LocalizedUIBehaviour`, hook into parent controller |
| New localization string | CSV + `GameLocalization.TextEntries` | See §7 |
| New SFX | `AudioManager` inspector → `_SFXDataList` | Add `AudioId` enum value, assign clip |
| New card category | `CardCategory` enum + `StackingRulesMatrix` | Add category rule row/column |

---

## 11. Coding Conventions

- **Namespace:** `Markyu.FortStack` on all production types.
- **Singletons:** `public static T Instance { get; private set; }` with `DontDestroyOnLoad`. Never use `FindObjectOfType` at runtime.
- **Events:** Standard `event System.Action<T>` — unsubscribe in `OnDisable` (MonoBehaviour) or `OnDestroy` (persistent objects).
- **ScriptableObjects:** Static data and configuration. No coroutines, no scene references.
- **Null safety:** Always null-check `Manager.Instance` before use. Pattern: `CardManager.Instance?.DoThing()`.
- **Inspector:** Use `[Header("...")]` for logical field groupings, `[Tooltip("...")]` on every designer-facing field.
- **Comments:** Add comments only where the *why* is non-obvious. Avoid restating the code.
- **Regions:** Use `#region` only for genuinely large classes where collapsing aids navigation (e.g. `CardInstance`).
- **Localization:** Never hardcode visible UI strings. Use `GameLocalization.Get(key)`.
- **Resources.Load:** Only use `Resources.LoadAll` in catalog/manager `Awake` — never in `Update`.

---

## 12. Known Risks & Refactoring Notes

### CardManager God Class (986 lines)
**Risk:** Medium. Changes to CardManager can have wide blast radius.  
**Planned split (when safe, in a dedicated branch):**
- `CardFactory` — `CreateCardInstance`, `CreatePackInstance`, prefab lookup
- `CardStackRegistry` — register/unregister stacks, stack queries
- `CardPhysicsResolver` — overlap resolution (currently `CardPhysicsSolver` is a static util called by CardManager)
- `CardDiscoveryTracker` — discovered cards/recipes tracking
- Keep `CardManager` as a facade that delegates to these services to preserve public API

### GameLocalization All-Hardcoded Dictionary (748 lines, 370+ entries)
**Risk:** Medium. Adding new strings requires recompile. No runtime hot-swap.  
**Plan:** Migrate to Unity Localization string tables entirely. `GameLocalization.Get` can switch to `LocalizationSettings.StringDatabase.GetLocalizedString`. Needs a full rollout with CSV roundtrip testing.

### Canvas Scaler Unverified
**Risk:** Medium. If scenes use default Canvas (no CanvasScaler), UI will not scale correctly at non-1080p resolutions.  
**Action (manual):** Open each scene in Unity and verify every root Canvas has a `CanvasScaler` component configured as described in §5 and `Docs/PixelPerfectSetup.md`.

### Multi-line YAML in String Tables
**Risk:** High for localization rebuild. Unity's YAML parser rejects multi-line double-quoted strings.  
**Rule:** All `m_Localized:` values in `.asset` string tables must be on a single line.

### `CardDefinition.OnValidate` GUID Generation
**Risk:** Low but worth watching. `OnValidate` regenerates a GUID if the `id` field is empty, which can cause unexpected asset re-serialization in version control.

### `CardPhysicsSolver.ResolveOverlaps` — `Physics.OverlapSphere` per frame
**Risk:** Medium performance at high card counts. Currently limited by `maxIterations` (default 8). Profiler-gate if card counts exceed ~50.
