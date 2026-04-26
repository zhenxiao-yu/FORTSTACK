# Sandbox Scenes

Development-only scenes for isolated feature testing. **Do not add any of these to Build Settings.**

---

## Sandbox_Cards.unity

Test card-level systems in isolation.

- Card spawning via DevSpawner window
- Drag, drop, and release behavior
- Stack formation and merging
- Card feel: hover, tilt, pickup animations
- Damage feedback (flash, shake)
- `CardFeelPresenter` shader effects

**Setup needed:** A Camera, an EventSystem, a CardSettings asset wired to a spawnable card prefab, and an `AudioManager` prefab in the scene.

---

## Sandbox_Crafting.unity

Test recipe and crafting systems in isolation.

- Valid recipe detection when cards are stacked
- Crafting timer progress
- Pause and resume mid-craft
- Invalid stacks (no recipe match)
- Continuous recipes (auto-repeat)

**Setup needed:** Camera, EventSystem, `CraftingManager` instance, a selection of card prefabs with definitions that form at least one known recipe.

---

## Sandbox_Combat.unity

Test combat flow between player cards and mob cards.

- Player card initiating combat with a mob
- Mob aggro and auto-join behavior
- Flee (drag card away mid-combat)
- Damage, death, and loot drop
- RPS (Rock-Paper-Scissors) combat type advantage

**Setup needed:** Camera, EventSystem, `CombatManager` instance, at least one player card and one aggressive mob card.

---

## Sandbox_Localization.unity

Test runtime language switching and localization coverage.

- Toggle between English and Simplified Chinese
- Verify card names update on language switch
- Verify quest titles and descriptions update
- Verify pack names update
- Verify recipe names update
- Check missing key fallback behavior

**Setup needed:** Camera, EventSystem, `GameLocalization` initialized. Cards, quests, packs on screen with text components bound to localization keys.

---

## Sandbox_UI.unity

Test UI layout, scaling, and pixel-perfect rendering.

- HUD elements at correct screen positions
- Info panels and tooltips
- UI scaling across resolutions (1080p, 720p, mobile 16:9, 4:3)
- Pixel-perfect camera alignment
- Mobile aspect ratio simulation (use Game View dropdown)

**Setup needed:** Camera with PixelPerfectCamera, UI Canvas with CanvasScaler, a sampling of HUD and tooltip prefabs.

---

## Rules

- These scenes are **never** added to Build Settings.
- They are listed in `.gitignore` only if you choose not to track them — otherwise commit them so the team can share setups.
- Use **DevSpawner** (`Tools/LAST KERNEL/Dev Spawner`) in Play Mode to populate these scenes quickly.
- Use **DebugOverlay** (press `F1` or `` ` ``) to monitor live state while testing.
