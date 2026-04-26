# Localization Setup

## Scan Summary

- Unity version: 6000.4.3f1.
- Render pipeline: URP 17.4.0 is present in the manifest and active through quality settings.
- `Packages/manifest.json` is valid JSON and did not previously contain `com.unity.localization`.
- `Packages/packages-lock.json` is valid JSON and had no stale localization entry.
- TextMesh Pro assets exist under `Assets/TextMesh Pro`, and project UI code already uses `TextMeshProUGUI` and `TMP_Text`.
- Runtime code lives under `Assets/Fortstack` with namespace `Markyu.FortStack`.
- Existing localization system found:
  - `Assets/Fortstack/Scripts/Core/GameLocalization.cs`
  - `Assets/Fortstack/Scripts/UI/LocalizedUIBehaviour.cs`
  - `Assets/Fortstack/Scripts/Core/TMPThemeController.cs`
  - `Assets/Fortstack/Scripts/UI/GameOptionsUI.cs`
- Existing language selection is saved through `PlayerPrefs` using `GameIdentity.LanguagePlayerPrefsKey`.
- Scenes found: `Assets/Fortstack/Scenes/Main.unity`, `Title.unity`, and `Island.unity`.
- Menu/UI prefabs found under `Assets/Fortstack/Prefabs/UI`.
- `Library/PackageCache` had no `.tmp*` or `com.unity.localization@*` folders at scan time.
- `Library/PackageManager` was absent at scan time.
- The project is on drive `H:` labeled `X9 Pro`, formatted as exFAT. External/exFAT project drives are more prone to Unity Package Manager rename/lock problems than an internal NTFS workspace.

## Strategy

Chosen strategy: B + C.

- B because `com.unity.localization` was not installed, so the package dependency was added to the manifest.
- C because the project already has an active custom localization system. The existing system was extended instead of replaced.
- No scene or prefab references were edited. Unity package import and String Table asset creation should happen after Package Manager resolves successfully.

## Package Repair

`com.unity.localization` was added as:

```json
"com.unity.localization": "1.5.11"
```

Unity's package registry reports `1.5.11` as the latest `com.unity.localization` version, and its package metadata lists a minimum Unity version of `2019.4`. Unity 6 documentation uses the Localization 1.5 package line, so this is appropriate for Unity 6000.4.3f1.

`packages-lock.json` was left untouched because it had no localization entry. Unity should regenerate the lock entry when it resolves packages.

No cache folders were deleted because none of the allowed broken localization/temp cache folders existed at scan time.

## Existing Runtime Integration

The existing `GameLocalization` flow remains the runtime source of truth for current UI:

- `LocalizedUIBehaviour` subscribes to `GameLocalization.LanguageChanged`.
- `GameOptionsUI` uses the existing language button.
- `TMPThemeController` refreshes TextMesh Pro text and fallback fonts.
- Language preference still uses `PlayerPrefs` through `GameIdentity.LanguagePlayerPrefsKey`.

`GameLocalization` now supports these locale codes:

- `en`
- `zh-Hans`
- `zh-Hant`
- `ja`
- `ko`
- `fr`
- `de`
- `es`

It also exposes:

- `AvailableLanguages`
- `SetLanguageByCode(string localeCode)`
- `TryGetLanguageFromCode(string localeCode, out GameLanguage language)`
- `GetLocaleCode(GameLanguage language)`

Existing two-language entries still work. Entries without a native translation for a newly added language fall back to English, except the starter keys in `GameText_Localization_Source.csv`, which include placeholders for all target languages.

## CSV Source

Source file:

```text
Assets/Fortstack/Localization/Docs/GameText_Localization_Source.csv
```

Columns:

```text
key,en,zh-Hans,zh-Hant,ja,ko,fr,de,es,notes
```

Use this CSV as the reviewable source for future Unity String Table import or manual migration into `GameLocalization`.

All non-English translations are placeholders and need native review before the Steam demo.

## Key Naming

Use lowercase dot-separated keys:

```text
category.item
category.subcategory.item
```

Examples:

```text
ui.play
menu.start_run
combat.core_under_attack
tooltip.defense_grid
```

Keep terminology consistent:

- Core: Core / 核心 / 核心 / コア / 코어
- Scrap: Scrap / 废料 / 廢料 / スクラップ / 고철
- Morale: Morale / 士气 / 士氣 / 士気 / 사기
- Energy: Energy / 能量 / 能量 / エネルギー / 에너지

## Adding A Language

1. Add a value to `GameLanguage`.
2. Add locale-code aliases in `LanguageByCode`.
3. Add the language to `LanguageCycle`.
4. Add culture mapping in `CurrentCulture`.
5. Add a display-name key in `TextEntries`.
6. Add a CSV column and fill translations.
7. If using Unity String Tables, add a matching Locale in Project Settings > Localization.

## Adding String Keys

1. Add the key to the CSV.
2. Add English copy first and keep it concise.
3. Add placeholder translations for other target languages.
4. Mark placeholder rows in `notes`.
5. Add the key to `GameLocalization` if runtime code needs it before Unity String Tables are wired.
6. Use `GameLocalization.Get(key)` or `GameLocalization.Format(key, args)` from existing UI/gameplay code.

## Unity Editor Verification

1. Open Unity Hub as Administrator.
2. Open `H:\Game\FORTSTACK`.
3. Wait for Package Manager to resolve.
4. Check Window > Package Manager > Localization.
5. Open Project Settings > Localization.
6. Create or confirm Locales for `en`, `zh-Hans`, `zh-Hant`, `ja`, `ko`, `fr`, `de`, and `es`.
7. Create/import a String Table named `GameText` from the CSV if you want Unity String Tables now.
8. Keep the current `GameLocalization` runtime path until String Table coverage is complete.
9. Connect the existing options/settings language button to any future Unity Localization bridge only after the package imports cleanly.
10. Test language switching in Play Mode.

## Steam Demo Workflow

- Keep English as the editorial source.
- Run native review for all non-English languages.
- Migrate high-risk UI first: title menu, options menu, pause menu, combat warnings, and failure/victory text.
- Confirm fonts cover Simplified Chinese, Traditional Chinese, Japanese, and Korean.
- Test every supported language at 1280x720 and Steam Deck-like resolutions.
