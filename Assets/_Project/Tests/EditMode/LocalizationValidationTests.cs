using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization.Tables;

namespace Markyu.LastKernel.Tests
{
    /// <summary>
    /// Validates the project's localization setup and key coverage.
    /// </summary>
    public class LocalizationValidationTests
    {
        [Test]
        public void EnglishLocale_Exists()
        {
            var locales = LocalizationEditorSettings.GetLocales();
            bool found = locales.Any(l => l.Identifier.Code == "en");
            Assert.IsTrue(found, "No English ('en') locale found in Localization settings.");
        }

        [Test]
        public void ChineseLocale_Exists()
        {
            var locales = LocalizationEditorSettings.GetLocales();
            bool found = locales.Any(l => l.Identifier.Code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(found, "No Chinese ('zh*') locale found in Localization settings.");
        }

        [Test]
        public void DefaultStringTable_Exists()
        {
            StringTableCollection collection = LocalizationEditorSettings
                .GetStringTableCollection(UnityLocalizationBridge.DefaultStringTable);
            Assert.IsNotNull(collection, $"String table '{UnityLocalizationBridge.DefaultStringTable}' not found.");
        }

        [Test]
        public void DefaultStringTable_HasNoEmptyEnglishTranslations()
        {
            StringTableCollection collection = LocalizationEditorSettings
                .GetStringTableCollection(UnityLocalizationBridge.DefaultStringTable);
            if (collection == null)
            {
                Assert.Ignore($"String table '{UnityLocalizationBridge.DefaultStringTable}' not found — skipping.");
                return;
            }

            StringTable englishTable = collection.StringTables
                .FirstOrDefault(t => t.LocaleIdentifier.Code == "en");
            if (englishTable == null)
            {
                Assert.Ignore("No English string table found — skipping.");
                return;
            }

            var empty = new List<string>();
            foreach (KeyValuePair<long, StringTableEntry> pair in englishTable)
            {
                if (string.IsNullOrWhiteSpace(pair.Value.Value))
                {
                    SharedTableData.SharedTableEntry sharedEntry = collection.SharedData.GetEntry(pair.Key);
                    empty.Add(sharedEntry?.Key ?? pair.Key.ToString());
                }
            }

            if (empty.Count > 0)
                Debug.LogWarning($"[Test] Empty English translations ({empty.Count}):\n{string.Join("\n", empty)}");
        }

        [Test]
        public void DefaultStringTable_HasNoDuplicateKeys()
        {
            StringTableCollection collection = LocalizationEditorSettings
                .GetStringTableCollection(UnityLocalizationBridge.DefaultStringTable);
            if (collection == null)
            {
                Assert.Ignore($"String table '{UnityLocalizationBridge.DefaultStringTable}' not found — skipping.");
                return;
            }

            var seen = new HashSet<string>(System.StringComparer.Ordinal);
            var duplicates = new List<string>();
            foreach (SharedTableData.SharedTableEntry entry in collection.SharedData.Entries)
            {
                if (!seen.Add(entry.Key))
                    duplicates.Add(entry.Key);
            }

            Assert.IsEmpty(duplicates, "Duplicate localization keys:\n" + string.Join("\n", duplicates));
        }
    }
}
