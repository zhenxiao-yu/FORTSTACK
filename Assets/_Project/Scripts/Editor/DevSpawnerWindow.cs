#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Markyu.LastKernel
{
    /// <summary>
    /// Editor window for spawning cards, triggering game events, and switching
    /// language during development. Never active in release builds.
    /// </summary>
    public class DevSpawnerWindow : EditorWindow
    {
        // ─── State ───────────────────────────────────────────────────────────

        private GameObject _cardPrefab;
        private CardSettings _cardSettings;
        private Vector2 _cardListScroll;
        private string _searchFilter = string.Empty;

        private CardDefinition[] _allDefinitions;
        private CardDefinition _selectedDefinition;

        private int _spawnCount = 1;
        private Vector3 _spawnPosition = Vector3.zero;

        private readonly List<GameObject> _spawnedCards = new();

        private bool _showPresets;
        private bool _showCombat;
        private bool _showWorld;
        private bool _showLanguage;

        // ─── Menu Item ───────────────────────────────────────────────────────

        [MenuItem("Tools/LAST KERNEL/Dev Spawner")]
        public static void Open()
        {
            var window = GetWindow<DevSpawnerWindow>("Dev Spawner");
            window.minSize = new Vector2(320, 480);
            window.Show();
        }

        // ─── Lifecycle ───────────────────────────────────────────────────────

        private void OnEnable() => RefreshDefinitions();

        private void OnFocus() => RefreshDefinitions();

        private void RefreshDefinitions()
        {
            string[] guids = AssetDatabase.FindAssets("t:CardDefinition", new[] { "Assets/_Project" });
            _allDefinitions = guids
                .Select(g => AssetDatabase.LoadAssetAtPath<CardDefinition>(AssetDatabase.GUIDToAssetPath(g)))
                .Where(d => d != null)
                .OrderBy(d => d.name)
                .ToArray();
        }

        // ─── GUI ─────────────────────────────────────────────────────────────

        private void OnGUI()
        {
            DrawPlayModeWarning();
            DrawPrefabSetup();
            EditorGUILayout.Space(4);
            DrawCardPicker();
            EditorGUILayout.Space(4);
            DrawSpawnControls();
            EditorGUILayout.Space(4);
            DrawPresets();
            EditorGUILayout.Space(4);
            DrawCombatTools();
            EditorGUILayout.Space(4);
            DrawWorldTools();
            EditorGUILayout.Space(4);
            DrawLanguageTools();
            EditorGUILayout.Space(4);
            DrawClearButton();
        }

        private static void DrawPlayModeWarning()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox(
                    "Enter Play Mode to use spawn actions. Card list and prefab setup are available in Edit Mode.",
                    MessageType.Info);
            }
        }

        private void DrawPrefabSetup()
        {
            EditorGUILayout.LabelField("Setup", EditorStyles.boldLabel);
            _cardPrefab = (GameObject)EditorGUILayout.ObjectField("Card Prefab", _cardPrefab, typeof(GameObject), false);
            _cardSettings = (CardSettings)EditorGUILayout.ObjectField("Card Settings", _cardSettings, typeof(CardSettings), false);

            if (_cardPrefab == null)
            {
                EditorGUILayout.HelpBox("Assign a card prefab with CardInstance to enable spawning.", MessageType.Warning);
            }
        }

        private void DrawCardPicker()
        {
            EditorGUILayout.LabelField("Card Definitions", EditorStyles.boldLabel);

            _searchFilter = EditorGUILayout.TextField("Filter", _searchFilter);

            IEnumerable<CardDefinition> filtered = string.IsNullOrWhiteSpace(_searchFilter)
                ? _allDefinitions
                : _allDefinitions.Where(d => d.name.IndexOf(_searchFilter, System.StringComparison.OrdinalIgnoreCase) >= 0);

            _cardListScroll = EditorGUILayout.BeginScrollView(_cardListScroll, GUILayout.Height(140));
            foreach (CardDefinition def in filtered)
            {
                bool isSelected = _selectedDefinition == def;
                if (GUILayout.Toggle(isSelected, def.name, "Button") && !isSelected)
                    _selectedDefinition = def;
            }
            EditorGUILayout.EndScrollView();

            if (_selectedDefinition != null)
                EditorGUILayout.LabelField("Selected", _selectedDefinition.name, EditorStyles.miniLabel);
        }

        private void DrawSpawnControls()
        {
            EditorGUILayout.LabelField("Spawn", EditorStyles.boldLabel);
            _spawnCount = EditorGUILayout.IntSlider("Count", _spawnCount, 1, 10);
            _spawnPosition = EditorGUILayout.Vector3Field("Position", _spawnPosition);

            GUI.enabled = Application.isPlaying && _cardPrefab != null && _selectedDefinition != null;
            if (GUILayout.Button($"Spawn '{(_selectedDefinition != null ? _selectedDefinition.name : "—")}' ×{_spawnCount}"))
                SpawnCards(_selectedDefinition, _spawnCount);
            GUI.enabled = true;
        }

        private void DrawPresets()
        {
            _showPresets = EditorGUILayout.Foldout(_showPresets, "Preset Groups", true);
            if (!_showPresets) return;

            GUI.enabled = Application.isPlaying && _cardPrefab != null;

            if (GUILayout.Button("Spawn: All Resources (first 5)"))
                SpawnByCategory(CardCategory.Resource, 5);
            if (GUILayout.Button("Spawn: All Materials (first 5)"))
                SpawnByCategory(CardCategory.Material, 5);
            if (GUILayout.Button("Spawn: All Mobs (first 3)"))
                SpawnByCategory(CardCategory.Mob, 3);

            GUI.enabled = true;
        }

        private void DrawCombatTools()
        {
            _showCombat = EditorGUILayout.Foldout(_showCombat, "Combat Tools", true);
            if (!_showCombat) return;

            GUI.enabled = Application.isPlaying;

            if (GUILayout.Button("Damage Selected Card (-5 HP)"))
                ApplyToSelectedCard(card => card.TakeDamage(5));
            if (GUILayout.Button("Heal Selected Card (+10 HP)"))
                ApplyToSelectedCard(card => card.Heal(10));

            GUI.enabled = true;
        }

        private void DrawWorldTools()
        {
            _showWorld = EditorGUILayout.Foldout(_showWorld, "World Tools", true);
            if (!_showWorld) return;

            GUI.enabled = Application.isPlaying;

            if (GUILayout.Button("Trigger Day → Night"))
                TriggerDayNightTransition();

            GUI.enabled = true;
        }

        private void DrawLanguageTools()
        {
            _showLanguage = EditorGUILayout.Foldout(_showLanguage, "Language", true);
            if (!_showLanguage) return;

            GUI.enabled = Application.isPlaying;

            if (GUILayout.Button("Switch to English"))
                SwitchLanguage(GameLanguage.English);
            if (GUILayout.Button("Switch to Simplified Chinese"))
                SwitchLanguage(GameLanguage.SimplifiedChinese);

            GUI.enabled = true;
        }

        private void DrawClearButton()
        {
            GUI.enabled = Application.isPlaying && _spawnedCards.Count > 0;
            if (GUILayout.Button($"Clear Spawned Cards ({_spawnedCards.Count})"))
                ClearSpawnedCards();
            GUI.enabled = true;
        }

        // ─── Actions ─────────────────────────────────────────────────────────

        private void SpawnCards(CardDefinition definition, int count)
        {
            if (_cardPrefab == null || definition == null) return;

            for (int i = 0; i < count; i++)
            {
                Vector3 pos = _spawnPosition + new Vector3(i * 1.2f, 0f, 0f);
                GameObject instance = Instantiate(_cardPrefab, pos, Quaternion.identity);
                instance.name = $"[DEV] {definition.name}";

                CardInstance card = instance.GetComponent<CardInstance>();
                if (card != null && _cardSettings != null)
                    card.Initialize(definition, _cardSettings, null);

                _spawnedCards.Add(instance);
            }

            Debug.Log($"[DevSpawner] Spawned {count}× {definition.name}.");
        }

        private void SpawnByCategory(CardCategory category, int max)
        {
            CardDefinition[] matching = _allDefinitions
                .Where(d => d.Category == category)
                .Take(max)
                .ToArray();

            for (int i = 0; i < matching.Length; i++)
                SpawnCards(matching[i], 1);
        }

        private void ApplyToSelectedCard(System.Action<CardInstance> action)
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                CardInstance card = go.GetComponent<CardInstance>();
                if (card != null) action(card);
            }
        }

        private void TriggerDayNightTransition()
        {
            // Searches for a component that exposes a day/night trigger.
            // Extend this when a DayNightCycle or GameDirector API is available.
            Debug.LogWarning("[DevSpawner] Day/Night transition: no manager found. Wire up DayNightCycle or GameDirector here.");
        }

        private void SwitchLanguage(GameLanguage language)
        {
            GameLocalization.SetLanguage(language);
            Debug.Log($"[DevSpawner] Language switched to {language}.");
        }

        private void ClearSpawnedCards()
        {
            foreach (GameObject go in _spawnedCards)
            {
                if (go != null) Destroy(go);
            }
            int count = _spawnedCards.Count;
            _spawnedCards.Clear();
            Debug.Log($"[DevSpawner] Cleared {count} spawned card(s).");
        }
    }
}
#endif
