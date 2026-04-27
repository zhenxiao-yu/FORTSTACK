// NightDefenseSetup — One-click scene scaffolding for the night defense vertical slice.
//
// Run via: Tools / LAST KERNEL / Setup Night Defense Scene
//
// What it creates:
//   1. DefensePhaseController  — phase singleton
//   2. NightBattlefield     — 5 defender slots, spawn + base markers
//   3. BaseCoreController
//   4. DefenseLoadoutController
//   5. RewardController
//   6. Night HUD Canvas (NightHUD, VictoryPanel, DefeatPanel)
//   7. Day HUD Canvas (DayHUD with Start Night button)
//   8. Test ScriptableObject assets for 3 enemies, 3 defenders, 3 waves
//
// This tool is ADDITIVE — safe to run on an existing scene; it skips objects
// that already exist (checked by GameObject name).

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Markyu.LastKernel
{
    public static class NightDefenseSetup
    {
        private const string EnemyAssetPath    = "Assets/_Project/Data/Balance/Enemies/";
        private const string DefenderAssetPath = "Assets/_Project/Data/Balance/Defenders/";
        private const string WaveAssetPath     = "Assets/_Project/Data/Balance/Waves/";
        private const string RewardAssetPath   = "Assets/_Project/Data/Balance/";

        [MenuItem("Tools/LAST KERNEL/Setup Night Defense Scene")]
        public static void SetupNightDefenseScene()
        {
            // ── 1. Create test data assets ─────────────────────────────────────
            var enemies   = CreateTestEnemies();
            var defenders = CreateTestDefenders();
            var waves     = CreateTestWaves(enemies);
            var reward    = CreateTestReward();

            // ── 2. Scene controllers ───────────────────────────────────────────
            var phaseCtrl = EnsureGameObject<DefensePhaseController>("DefensePhaseController");
            var baseCtrl  = EnsureGameObject<BaseCoreController>("BaseCoreController");
            var rewardCtrl = EnsureGameObject<RewardController>("RewardController");

            // ── 3. Battlefield ─────────────────────────────────────────────────
            SetupBattlefield(baseCtrl, defenders, waves[0]);

            // ── 4. UI ──────────────────────────────────────────────────────────
            SetupNightCanvas(baseCtrl, rewardCtrl, reward);
            SetupDayCanvas();

            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog(
                "Night Defense Setup",
                "Scene scaffolding created.\n\n" +
                "Next steps:\n" +
                "  1. Assign NightBattlefieldController references in Inspector.\n" +
                "  2. Wire NightHUD → battlefield + baseCoreController.\n" +
                "  3. Wire VictoryPanel → rewardController.\n" +
                "  4. Press Play and click 'Start Night'.",
                "OK");
        }

        // ── Enemy assets ─────────────────────────────────────────────────────

        private static EnemyDefinition[] CreateTestEnemies()
        {
            return new[]
            {
                CreateEnemy("Enemy_BasicCrawler",  "Basic Crawler",  hp:  8, atk: 2, def: 0, speed: 2.0f, dmgToBase: 1, reward: 1),
                CreateEnemy("Enemy_FastGlitch",    "Fast Glitch",    hp:  5, atk: 1, def: 0, speed: 3.5f, dmgToBase: 1, reward: 2),
                CreateEnemy("Enemy_HeavyBug",      "Heavy Bug",      hp: 25, atk: 4, def: 2, speed: 0.9f, dmgToBase: 3, reward: 4),
            };
        }

        private static EnemyDefinition CreateEnemy(
            string fileName, string displayName,
            int hp, int atk, int def, float speed, int dmgToBase, int reward)
        {
            string path = EnemyAssetPath + fileName + ".asset";
            var existing = AssetDatabase.LoadAssetAtPath<EnemyDefinition>(path);
            if (existing != null) return existing;

            var asset = ScriptableObject.CreateInstance<EnemyDefinition>();
            // Use SerializedObject to set private fields cleanly
            var so = new SerializedObject(asset);
            so.FindProperty("displayName").stringValue    = displayName;
            so.FindProperty("maxHP").intValue             = hp;
            so.FindProperty("attack").intValue            = atk;
            so.FindProperty("defense").intValue           = def;
            so.FindProperty("moveSpeed").floatValue       = speed;
            so.FindProperty("damageToBase").intValue      = dmgToBase;
            so.FindProperty("rewardAmount").intValue      = reward;
            so.ApplyModifiedPropertiesWithoutUndo();

            AssetDatabase.CreateAsset(asset, path);
            Debug.Log($"[NightSetup] Created enemy: {path}");
            return asset;
        }

        // ── Defender assets ───────────────────────────────────────────────────

        private static DefenderData[] CreateTestDefenders()
        {
            return new[]
            {
                CreateDefender("Defender_KernelGuard",  "Kernel Guard",  hp: 30, dmg: 5, rate: 1.0f, range: 3.0f),
                CreateDefender("Defender_FirewallNode", "Firewall Node", hp: 50, dmg: 9, rate: 0.5f, range: 2.5f),
                CreateDefender("Defender_SignalPinger", "Signal Pinger", hp: 15, dmg: 2, rate: 3.0f, range: 4.0f),
            };
        }

        private static DefenderData CreateDefender(
            string fileName, string displayName,
            int hp, int dmg, float rate, float range)
        {
            string path = DefenderAssetPath + fileName + ".asset";
            var existing = AssetDatabase.LoadAssetAtPath<DefenderData>(path);
            if (existing != null) return existing;

            var asset = ScriptableObject.CreateInstance<DefenderData>();
            var so = new SerializedObject(asset);
            so.FindProperty("displayName").stringValue    = displayName;
            so.FindProperty("maxHealth").intValue         = hp;
            so.FindProperty("attackDamage").intValue      = dmg;
            so.FindProperty("attackRate").floatValue      = rate;
            so.FindProperty("range").floatValue           = range;
            so.ApplyModifiedPropertiesWithoutUndo();

            AssetDatabase.CreateAsset(asset, path);
            Debug.Log($"[NightSetup] Created defender: {path}");
            return asset;
        }

        // ── Wave assets ───────────────────────────────────────────────────────

        private static NightWaveDefinition[] CreateTestWaves(EnemyDefinition[] enemies)
        {
            // Waves reference NightWaveDefinition's EnemyEntry list
            // We create them and log a reminder to wire enemy entries manually in Inspector
            // since EnemyEntry uses a nested serialized class.
            string[] waveNames = { "Wave_Night1", "Wave_Night2", "Wave_Night3" };
            var result = new NightWaveDefinition[3];

            for (int i = 0; i < waveNames.Length; i++)
            {
                string path = WaveAssetPath + waveNames[i] + ".asset";
                var existing = AssetDatabase.LoadAssetAtPath<NightWaveDefinition>(path);
                if (existing != null) { result[i] = existing; continue; }

                var asset = ScriptableObject.CreateInstance<NightWaveDefinition>();
                var so = new SerializedObject(asset);
                so.FindProperty("waveName").stringValue = $"Night {i + 1}";
                so.ApplyModifiedPropertiesWithoutUndo();
                AssetDatabase.CreateAsset(asset, path);
                result[i] = asset;
                Debug.Log($"[NightSetup] Created wave {path} — add EnemyEntries in Inspector.");
            }
            return result;
        }

        // ── Reward asset ──────────────────────────────────────────────────────

        private static RewardData CreateTestReward()
        {
            string path = RewardAssetPath + "Reward_Default.asset";
            var existing = AssetDatabase.LoadAssetAtPath<RewardData>(path);
            if (existing != null) return existing;

            var asset = ScriptableObject.CreateInstance<RewardData>();
            var so = new SerializedObject(asset);
            so.FindProperty("scrapAmount").intValue          = 15;
            so.FindProperty("rewardTitle").stringValue       = "Wave Cleared!";
            so.FindProperty("rewardDescription").stringValue = "The attackers have been repelled. Scrap collected.";
            so.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        // ── Scene objects ─────────────────────────────────────────────────────

        private static void SetupBattlefield(
            BaseCoreController baseCtrl, DefenderData[] defenders, NightWaveDefinition wave)
        {
            // Check if battlefield already exists
            var existing = GameObject.Find("NightBattlefield");
            if (existing != null) return;

            var root = new GameObject("NightBattlefield");
            Undo.RegisterCreatedObjectUndo(root, "Create NightBattlefield");

            var battlefield = root.AddComponent<NightBattlefieldController>();
            var loadout     = root.AddComponent<DefenseLoadoutController>();

            // Spawn + base markers
            var spawnMarker = new GameObject("SpawnMarker");
            spawnMarker.transform.SetParent(root.transform);
            spawnMarker.transform.localPosition = new Vector3(8f, 0f, 0f);

            var baseMarker = new GameObject("BaseMarker");
            baseMarker.transform.SetParent(root.transform);
            baseMarker.transform.localPosition = new Vector3(-8f, 0f, 0f);

            // 5 defender slots evenly spaced along the lane
            var slots = new Transform[5];
            for (int i = 0; i < 5; i++)
            {
                var slot = new GameObject($"DefenderSlot_{i}");
                slot.transform.SetParent(root.transform);
                // Space evenly from x=-6 to x=6 (base side to spawn side)
                slot.transform.localPosition = new Vector3(-6f + i * 3f, 0f, 0f);
                slots[i] = slot.transform;
            }

            // Wire serialized fields via SerializedObject
            var so = new SerializedObject(battlefield);
            so.FindProperty("baseCoreController").objectReferenceValue = baseCtrl;
            so.FindProperty("spawnMarker").objectReferenceValue        = spawnMarker.transform;
            so.FindProperty("baseMarker").objectReferenceValue         = baseMarker.transform;
            so.FindProperty("currentWave").objectReferenceValue        = wave;

            var slotsProp = so.FindProperty("defenderSlots");
            slotsProp.arraySize = slots.Length;
            for (int i = 0; i < slots.Length; i++)
                slotsProp.GetArrayElementAtIndex(i).objectReferenceValue = slots[i];

            so.ApplyModifiedPropertiesWithoutUndo();

            // Wire loadout
            var loadoutSo = new SerializedObject(loadout);
            var loadoutProp = loadoutSo.FindProperty("defenderOverrides");
            loadoutProp.arraySize = Mathf.Min(defenders.Length, slots.Length);
            for (int i = 0; i < loadoutProp.arraySize; i++)
                loadoutProp.GetArrayElementAtIndex(i).objectReferenceValue = defenders[i];
            loadoutSo.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetupNightCanvas(
            BaseCoreController baseCtrl, RewardController rewardCtrl, RewardData reward)
        {
            if (GameObject.Find("NightCanvas") != null) return;

            var canvas = CreateCanvas("NightCanvas");
            Undo.RegisterCreatedObjectUndo(canvas, "Create NightCanvas");

            // NightHUD
            var hudGO  = CreatePanel(canvas.transform, "NightHUD", new Color(0, 0, 0, 0.3f));
            var hud    = hudGO.AddComponent<NightHUD>();
            CreateLabel(hudGO.transform, "PhaseLabel",     "NIGHT",          new Vector2(0, 70), 28);
            CreateLabel(hudGO.transform, "WaveLabel",      "Night 1",        new Vector2(0, 40), 22);
            CreateLabel(hudGO.transform, "EnemyCount",     "Enemies: 0 / 0", new Vector2(0, 10), 18);

            CreateLabel(hudGO.transform, "BaseHPText", "HP: 20 / 20", new Vector2(0, -20), 18);

            // Wire HUD references
            var hudSo = new SerializedObject(hud);
            hudSo.FindProperty("baseCoreController").objectReferenceValue = baseCtrl;
            hudSo.ApplyModifiedPropertiesWithoutUndo();

            // VictoryPanel
            var victoryGO = CreatePanel(canvas.transform, "VictoryPanel", new Color(0.1f, 0.4f, 0.1f, 0.9f));
            victoryGO.SetActive(false);
            var victory = victoryGO.AddComponent<VictoryPanel>();
            CreateLabel(victoryGO.transform, "TitleLabel",  "Victory!",                 Vector2.up * 60, 32);
            CreateLabel(victoryGO.transform, "DescLabel",   "Wave cleared.",             Vector2.up * 20, 18);
            CreateLabel(victoryGO.transform, "ScrapLabel",  "+15 Scrap",                 Vector2.down * 20, 18);
            CreateButton(victoryGO.transform, "ContinueButton", "Continue to Day");

            var victorySo = new SerializedObject(victory);
            victorySo.FindProperty("rewardController").objectReferenceValue = rewardCtrl;
            victorySo.ApplyModifiedPropertiesWithoutUndo();

            // DefeatPanel
            var defeatGO = CreatePanel(canvas.transform, "DefeatPanel", new Color(0.4f, 0.05f, 0.05f, 0.9f));
            defeatGO.SetActive(false);
            defeatGO.AddComponent<DefeatPanel>();
            CreateLabel(defeatGO.transform, "MessageLabel", "The base has fallen.", Vector2.zero, 28);
            CreateButton(defeatGO.transform, "RetryButton",   "Retry");
            CreateButton(defeatGO.transform, "QuitButton",    "Quit to Menu");

            // Wire reward
            var rewardSo = new SerializedObject(rewardCtrl);
            rewardSo.FindProperty("defaultReward").objectReferenceValue = reward;
            rewardSo.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetupDayCanvas()
        {
            if (GameObject.Find("DayCanvas") != null) return;

            var canvas = CreateCanvas("DayCanvas");
            Undo.RegisterCreatedObjectUndo(canvas, "Create DayCanvas");

            var root = CreatePanel(canvas.transform, "DayHUDRoot", new Color(0, 0, 0, 0.2f));
            root.AddComponent<DayHUD>();
            CreateLabel(root.transform, "PhaseLabel", "DAY", new Vector2(-200, 70), 24);
            CreateButton(root.transform, "StartNightButton", "End Day / Start Night");
        }

        // ── UI helper builders ────────────────────────────────────────────────

        private static GameObject CreateCanvas(string name)
        {
            var go     = new GameObject(name);
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            go.AddComponent<CanvasScaler>();
            go.AddComponent<GraphicRaycaster>();
            return go;
        }

        private static GameObject CreatePanel(Transform parent, string name, Color color)
        {
            var go    = new GameObject(name);
            go.transform.SetParent(parent, false);
            var image = go.AddComponent<Image>();
            image.color = color;
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            return go;
        }

        private static GameObject CreateLabel(Transform parent, string name, string text, Vector2 anchoredPos, int fontSize)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text      = text;
            tmp.fontSize  = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color     = Color.white;
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta        = new Vector2(400, 40);
            return go;
        }

        private static void CreateButton(Transform parent, string name, string label)
        {
            var go     = new GameObject(name);
            go.transform.SetParent(parent, false);
            var image  = go.AddComponent<Image>();
            image.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            go.AddComponent<Button>();
            CreateLabel(go.transform, "Label", label, Vector2.zero, 18);
            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(220, 50);
        }

        // ── Generic helpers ───────────────────────────────────────────────────

        private static T EnsureGameObject<T>(string name) where T : Component
        {
            var existing = GameObject.Find(name);
            if (existing != null)
                return existing.GetComponent<T>() ?? existing.AddComponent<T>();

            var go = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(go, $"Create {name}");
            return go.AddComponent<T>();
        }
    }
}
