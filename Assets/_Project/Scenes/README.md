# Scene Roles

Production Build Settings use this order:

0. `Boot.unity` - Persistent managers, save/bootstrap setup, localization/input/audio initialization, and first-flow routing.
1. `MainMenu.unity` - Title screen, save slot selection, options, and menu flow.
2. `Game.unity` - Main gameplay scene.
3. `Island.unity` - Secondary gameplay scene used by the current travel recipe.

Development and prototype scenes stay out of production Build Settings:

- `Scenes/Test/` - Development-only scene work.
- `Scenes/Test/Main.unity` - Legacy duplicate of `Game.unity`; retained for reference and legacy scene-name mapping.
- `Scenes/Test/Title.unity` - Legacy duplicate of `MainMenu.unity`; retained for reference and legacy scene-name mapping.

Do not rename or delete scenes without first checking Build Settings, scene-loading code, saved-game scene names, Addressables, and prefab references.
