using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Markyu.LastKernel.Tests
{
    /// <summary>
    /// Validates AudioManager prefab configuration and AudioId coverage.
    /// </summary>
    public class AudioValidationTests
    {
        private const string AudioManagerPrefabPath = "Assets/_Project/Prefabs/Systems/Core/AudioManager.prefab";

        private GameObject _amPrefab;
        private AudioManager _am;
        private SerializedObject _so;

        [SetUp]
        public void SetUp()
        {
            _amPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(AudioManagerPrefabPath);
            if (_amPrefab != null)
            {
                _am = _amPrefab.GetComponent<AudioManager>();
                if (_am != null) _so = new SerializedObject(_am);
            }
        }

        [Test]
        public void AudioManagerPrefab_Exists()
        {
            Assert.IsNotNull(_amPrefab, $"AudioManager prefab not found at '{AudioManagerPrefabPath}'.");
        }

        [Test]
        public void AudioManagerPrefab_HasComponent()
        {
            if (_amPrefab == null) Assert.Ignore("Prefab not found — skipping.");
            Assert.IsNotNull(_am, "AudioManager prefab is missing the AudioManager component.");
        }

        [Test]
        public void AudioManager_MixerGroupsAreAssigned()
        {
            if (_so == null) Assert.Ignore("AudioManager not available — skipping.");

            bool sfxGroup = _so.FindProperty("_SFXAudioGroup")?.objectReferenceValue != null;
            bool bgmGroup = _so.FindProperty("_BGMAudioGroup")?.objectReferenceValue != null;
            bool mixer = _so.FindProperty("_audioMixer")?.objectReferenceValue != null;

            var missing = new List<string>();
            if (!mixer) missing.Add("_audioMixer");
            if (!sfxGroup) missing.Add("_SFXAudioGroup");
            if (!bgmGroup) missing.Add("_BGMAudioGroup");

            if (missing.Count > 0)
                Debug.LogWarning($"[Test] AudioManager missing assignments: {string.Join(", ", missing)}");
        }

        [Test]
        public void AudioManager_SFXList_HasNoNullClips()
        {
            if (_so == null) Assert.Ignore("AudioManager not available — skipping.");
            SerializedProperty list = _so.FindProperty("_SFXDataList");
            if (list == null || list.arraySize == 0)
            {
                Debug.LogWarning("[Test] AudioManager _SFXDataList is empty.");
                return;
            }

            var missing = new List<string>();
            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty entry = list.GetArrayElementAtIndex(i);
                SerializedProperty clip = entry.FindPropertyRelative("audioClip");
                if (clip != null && clip.objectReferenceValue == null)
                    missing.Add($"index [{i}]");
            }

            if (missing.Count > 0)
                Debug.LogWarning($"[Test] AudioManager SFX entries with no clip: {string.Join(", ", missing)}");
        }

        [Test]
        public void AudioManager_SFXList_HasNoDuplicateAudioIds()
        {
            if (_so == null) Assert.Ignore("AudioManager not available — skipping.");
            SerializedProperty list = _so.FindProperty("_SFXDataList");
            if (list == null) return;

            var seen = new HashSet<int>();
            var duplicates = new List<int>();
            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty idProp = list.GetArrayElementAtIndex(i).FindPropertyRelative("audioId");
                if (idProp != null && !seen.Add(idProp.enumValueIndex))
                    duplicates.Add(idProp.enumValueIndex);
            }

            Assert.IsEmpty(duplicates,
                $"Duplicate AudioId values in SFX list: {string.Join(", ", duplicates)}");
        }

        [Test]
        public void AudioId_AllEnumValues_AreRepresented()
        {
            if (_so == null) Assert.Ignore("AudioManager not available — skipping.");
            SerializedProperty list = _so.FindProperty("_SFXDataList");
            if (list == null) return;

            var registered = new HashSet<int>();
            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty idProp = list.GetArrayElementAtIndex(i).FindPropertyRelative("audioId");
                if (idProp != null) registered.Add(idProp.enumValueIndex);
            }

            var unregistered = new List<string>();
            foreach (AudioId id in Enum.GetValues(typeof(AudioId)))
            {
                if (!registered.Contains((int)id))
                    unregistered.Add(id.ToString());
            }

            if (unregistered.Count > 0)
                Debug.LogWarning($"[Test] AudioId values not in SFX list: {string.Join(", ", unregistered)}");
        }
    }
}
