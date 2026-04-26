# Localization Setup

Unity Localization is now the main localization system for Fortstack/Last Kernel. The existing custom `GameLocalization` API remains as a compatibility bridge until all scenes, prefabs, and gameplay strings are fully verified against Unity String Tables.

## Package And Settings

- Package: `com.unity.localization` `1.5.11`.
- Resolved package cache: `Library/PackageCache/com.unity.localization@2baf2a27280b`.
- Active settings asset path: `Assets/_Project/Localization/Localization Settings.asset`.
- Default string table: `GameText`.
- Project locale and fallback locale: English (`en`).

Supported locales:

- `en` - English
- `zh-Hans` - Simplified Chinese
- `zh-Hant` - Traditional Chinese
- `ja` - Japanese
- `ko` - Korean
- `fr` - French
- `de` - German
- `es` - Spanish

## Asset Layout

Localization assets belong under:

```text
Assets/_Project/Localization/
Assets/_Project/Localization/Locales/
Assets/_Project/Localization/StringTables/
Assets/_Project/Localization/AssetTables/
Assets/_Project/Localization/Docs/
```

The `AssetTables` folder is reserved for future localized sprites, audio, and font assets. Current work uses the `GameText` String Table Collection.

## Rebuild Workflow

Use the menu item:

```text
Last Kernel > Localization > Rebuild GameText Tables
```

The rebuild utility:

- Creates missing folders.
- Creates or activates the Unity Localization settings asset.
- Creates the eight target Locale assets.
- Creates or updates the `GameText` String Table Collection.
- Imports `GameText_Localization_Source.csv`.
- Adds legacy `GameLocalization` keys for compatibility.
- Adds safe asset-backed keys for cards, packs, recipes, quests, encounters, enemies, and night waves.
- Exports the merged table back to the CSV.

Batch equivalent:

```powershell
E:\Unity\6000.4.3f1\Editor\Unity.exe -batchmode -quit -projectPath E:\FORTSTACK -executeMethod Markyu.FortStack.Localization.EditorTools.LocalizationAssetBuilder.RebuildGameTextTablesBatch
```

Only run one Unity instance for the project while rebuilding. Batch mode cannot open the project if the Unity Editor already has it open.

## CSV Source

Source file:

```text
Assets/_Project/Localization/Docs/GameText_Localization_Source.csv
```

Columns:

```text
key,en,zh-Hans,zh-Hant,ja,ko,fr,de,es,notes
```

Add a key by adding a row to the CSV, then run the rebuild menu item. Keep keys lowercase and dot-separated:

```text
ui.play
menu.start_run
combat.core_under_attack
tooltip.defense_grid
card.scrap.name
card.scrap.description
```

English is the source copy. Non-English strings currently include placeholder translations and source-copy fallbacks, so all player-facing copy needs native review before release.

## Runtime Binding

Preferred binding for new TextMeshPro UI:

- Add Unity's `LocalizeStringEvent` to the object with `TextMeshProUGUI`.
- Set the table to `GameText`.
- Pick the table entry key.

Existing scenes and prefabs can keep using `LocalizedUIBehaviour` for now. It still exposes the same serialized fields, but it initializes the Unity Localization bridge and refreshes on `GameLocalization.LanguageChanged`.

Code paths should use:

```csharp
new LocalizedString("GameText", "ui.play")
```

or the compatibility API:

```csharp
GameLocalization.Get("ui.play")
GameLocalization.Format("day.current", dayNumber)
```

`GameLocalization.Get` now checks Unity's `GameText` table first and falls back to the legacy dictionary if a key is still missing from Unity Localization.

## Language Selection

`UnityLocalizationBridge` is the runtime bridge to `LocalizationSettings`.

- Available languages come from `LocalizationSettings.AvailableLocales`.
- Locale switches use locale codes such as `en` or `zh-Hans`.
- The selected locale code is saved in `PlayerPrefs` as `LastKernel.LocaleCode`.
- Old integer language preferences remain supported through `LastKernel.Language` and `FortStack.Language`.
- Missing or invalid saved locales fall back to English.
- Changing locale clears cached strings, updates `LocalizationSettings.SelectedLocale`, and triggers visible UI refresh through the existing `GameLocalization.LanguageChanged` path.

`GameOptionsUI` still calls the legacy-facing language API, but that API now delegates to Unity Localization when settings are available.

## Legacy System

Do not delete these yet:

- `GameLocalization`
- `LocalizedUIBehaviour`
- `GameOptionsUI`
- `TMPThemeController`

`GameLocalization` is intentionally still present because many scripts call `GameLocalization.Get(...)`, `GameLocalization.Format(...)`, or subscribe to `GameLocalization.LanguageChanged`. It should be removed only after all direct callers are migrated to Unity `LocalizedString`/`LocalizeStringEvent` and play mode tests pass in every supported locale.

## Font Fallback

The project already includes `TMP_NotoSansSC_Fallback` in TextMesh Pro settings, backed by:

```text
Assets/_Project/Art/Fonts/NatoSans/Source/NotoSansSC-Regular.ttf
```

This is a useful Simplified Chinese fallback, but it should not be treated as final coverage for Traditional Chinese, Japanese, or Korean.

If approved CJK fonts are added later:

1. Place source font files under `Assets/_Project/Art/Fonts/<FontName>/Source/`.
2. Create TMP font assets under `Assets/_Project/Art/Fonts/<FontName>/TMP/`.
3. Add those TMP font assets to `Project Settings > TextMesh Pro > Settings > Fallback Font Assets`.
4. Also add them to the default UI font asset fallback list if that font is used directly by UI prefabs.
5. Test all CJK locales in title, options, HUD, card tooltip, quest, and combat UI.

Do not download or include external font files without approval.

## Validation Checklist

- `Packages/manifest.json` contains `com.unity.localization`.
- `Packages/packages-lock.json` resolves `com.unity.localization`.
- Project Settings > Localization has an active settings asset.
- All eight Locale assets exist and are listed in Available Locales.
- `GameText` exists under `StringTables`.
- `GameText_Localization_Source.csv` imports without duplicate keys.
- Existing `LocalizedUIBehaviour` labels update after switching locale.
- `GameOptionsUI` changes `LocalizationSettings.SelectedLocale` immediately.
- Direct `GameLocalization.Get(...)` callers still work through the bridge/fallback.
- TextMesh Pro has usable fallback fonts for CJK glyphs.
- C# compiles in Unity.
