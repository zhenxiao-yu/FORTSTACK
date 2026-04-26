using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Markyu.LastKernel
{
    /// <summary>
    /// Safe, non-destructive editor fixes. Never deletes, renames, or moves assets.
    /// Never changes gameplay data or recipe balance.
    /// </summary>
    public static class QuickFixTools
    {
        [MenuItem("Tools/LAST KERNEL/Fix Safe Issues")]
        public static void FixSafeIssues()
        {
            Debug.Log("=== QuickFixTools: Running safe fixes ===");
            int fixes = 0;

            fixes += CreateMissingFolders();
            fixes += AssignDefaultFeelProfileToSelectedCards();
            fixes += AddFeelPresenterToSelectedCards();
            fixes += AddCardControllerToSelectedCards();

            Debug.Log($"=== QuickFixTools: Done — {fixes} fix(es) applied ===");
        }

        // ─── Folder Creation ─────────────────────────────────────────────────

        private static int CreateMissingFolders()
        {
            string[] required =
            {
                "Assets/_Project/Scenes/Test",
                "Assets/_Project/Tests/EditMode",
                "Assets/_Project/Tests/PlayMode",
                "Assets/_Project/Docs",
                "Assets/_Project/Data/Resources/Cards",
                "Assets/_Project/Data/Resources/Recipes",
                "Assets/_Project/Data/Resources/Packs",
                "Assets/_Project/Data/Resources/Quests",
            };

            int created = 0;
            foreach (string folder in required)
            {
                if (!AssetDatabase.IsValidFolder(folder))
                {
                    EnsureFolder(folder);
                    Debug.Log($"[QuickFix] Created folder: {folder}");
                    created++;
                }
            }
            return created;
        }

        private static void EnsureFolder(string path)
        {
            string[] parts = path.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }

        // ─── CardFeelProfile Assignment ──────────────────────────────────────

        private static int AssignDefaultFeelProfileToSelectedCards()
        {
            // Only safe when exactly one default CardFeelProfile exists in the project
            string[] profileGuids = AssetDatabase.FindAssets("t:CardFeelProfile", new[] { "Assets/_Project" });
            if (profileGuids.Length != 1)
            {
                if (profileGuids.Length == 0)
                    Debug.LogWarning("[QuickFix] No CardFeelProfile found — skipping feel profile assignment.");
                else
                    Debug.LogWarning($"[QuickFix] {profileGuids.Length} CardFeelProfiles found — cannot auto-assign (ambiguous). Skipping.");
                return 0;
            }

            string profilePath = AssetDatabase.GUIDToAssetPath(profileGuids[0]);
            ScriptableObject profile = AssetDatabase.LoadAssetAtPath<ScriptableObject>(profilePath);

            int fixed_ = 0;
            foreach (CardSettings settings in Selection.GetFiltered<CardSettings>(SelectionMode.Assets))
            {
                var so = new SerializedObject(settings);
                SerializedProperty prop = so.FindProperty("feelProfile");
                if (prop != null && prop.objectReferenceValue == null)
                {
                    prop.objectReferenceValue = profile;
                    so.ApplyModifiedProperties();
                    Debug.Log($"[QuickFix] Assigned default CardFeelProfile to '{AssetDatabase.GetAssetPath(settings)}'.");
                    fixed_++;
                }
            }
            return fixed_;
        }

        // ─── CardFeelPresenter ───────────────────────────────────────────────

        private static int AddFeelPresenterToSelectedCards()
        {
            int fixed_ = 0;
            foreach (GameObject go in Selection.gameObjects)
            {
                if (go.GetComponent<CardInstance>() == null) continue;
                if (go.GetComponent<CardFeelPresenter>() != null) continue;

                go.AddComponent<CardFeelPresenter>();
                EditorUtility.SetDirty(go);
                Debug.Log($"[QuickFix] Added CardFeelPresenter to '{go.name}'.");
                fixed_++;
            }
            return fixed_;
        }

        // ─── CardController ──────────────────────────────────────────────────

        private static int AddCardControllerToSelectedCards()
        {
            int fixed_ = 0;
            foreach (GameObject go in Selection.gameObjects)
            {
                if (go.GetComponent<CardInstance>() == null) continue;
                if (go.GetComponent<CardController>() != null) continue;

                go.AddComponent<CardController>();
                EditorUtility.SetDirty(go);
                Debug.Log($"[QuickFix] Added CardController to '{go.name}'.");
                fixed_++;
            }
            return fixed_;
        }
    }
}
