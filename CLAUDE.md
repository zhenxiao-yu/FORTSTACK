# LAST KERNEL — Claude Instructions

## Before editing

Do not scan the whole Unity project unless necessary.

Start with:
- `Assets/_Project/`
- `Assets/StackCraft/Scripts/`
- `Assets/StackCraft/ScriptableObjects/`
- `Packages/manifest.json`
- `ProjectSettings/ProjectVersion.txt`

Avoid reading:
- `Library/`
- `Temp/`
- `Logs/`
- `Obj/`
- `Build/`
- `Builds/`
- `UserSettings/`
- Generated `.csproj` / `.sln` files
- Imported asset demo folders unless directly relevant

## Main architecture

Unity 6000.4.3f1 pixel-art card survival / auto-battler.

**Priorities:**
1. Unity Localization for English + Simplified Chinese
2. Data-driven cards, packs, recipes, quests
3. Pixel-perfect responsive UI/camera
4. Mobile input support
5. Code cleanup without breaking gameplay

**Namespace:** `Markyu.LastKernel`

**Key assemblies:**
- Runtime: `Assets/_Project/Scripts/Runtime/` → `_Project.Runtime`
- Editor: `Assets/_Project/Scripts/Editor/` → `_Project.Editor`
- EditMode tests: `Assets/_Project/Tests/EditMode/` → `_Project.Tests.EditMode`
- PlayMode tests: `Assets/_Project/Tests/PlayMode/` → `_Project.Tests.PlayMode`

**Docs to read first for context:**
- `Assets/_Project/Docs/ARCHITECTURE.md`
- `Assets/_Project/Docs/CONTENT_ORGANIZATION.md`
- `Assets/_Project/Docs/DEVELOPMENT_TOOLS.md`
- `Assets/_Project/Docs/ART_DIRECTION.md` ← **source of truth for all UI/visual decisions**

## UI design rules

Before writing any UI code or prefab, read `Assets/_Project/Docs/ART_DIRECTION.md`.

- Style: dark cyberpunk pixel-art terminal — functional, not decorative.
- Palette: navy/charcoal background, cyan accent, muted magenta secondary, off-white text, amber/red-orange for warnings only.
- Resolution: 320×180 base composition, 1920×1080 display, integer scaling, point-sampled sprites only.
- Fonts: pixel-style, both English and Chinese legible, no hardcoded text, allow 30–40% text expansion in layouts.
- Cards: dark terminal frame, category border, name → art → description layout.
- Animations: short and snappy, no bouncing, no cartoon feel, respect pixel crispness.
- Mobile: 44 px minimum touch targets, safe-area insets, collapse secondary panels.

## Editing rules

- Search first, then edit only relevant files.
- Prefer small targeted changes.
- Do not duplicate existing systems.
- Do not refactor unrelated gameplay during localization or UI work.
- Keep mouse controls working when adding touch support.
- After changes, summarize: files changed and any manual Unity setup required.
- Never add sandbox or test scenes to Build Settings.
- Run `Tools/LAST KERNEL/Validate Project` after structural changes.
