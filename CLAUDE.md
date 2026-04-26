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

## Editing rules

- Search first, then edit only relevant files.
- Prefer small targeted changes.
- Do not duplicate existing systems.
- Do not refactor unrelated gameplay during localization or UI work.
- Keep mouse controls working when adding touch support.
- After changes, summarize: files changed and any manual Unity setup required.
- Never add sandbox or test scenes to Build Settings.
- Run `Tools/LAST KERNEL/Validate Project` after structural changes.
