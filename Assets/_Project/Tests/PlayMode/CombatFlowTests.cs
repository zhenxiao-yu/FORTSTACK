using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Markyu.LastKernel.Tests
{
    /// <summary>
    /// Tests CombatStats creation and CardDefinition combat data.
    /// Full combat flow (CombatManager, CombatTask) requires a game scene and is
    /// covered by manual sandbox testing in Sandbox_Combat.unity.
    /// </summary>
    public class CombatFlowTests
    {
        [UnityTest]
        public IEnumerator CombatStats_CreatedFromDefinition_IsValid()
        {
            CardDefinition def = ScriptableObject.CreateInstance<CardDefinition>();
            def.SetId("fighter_test");
            def.SetDisplayName("Test Fighter");

            yield return null;

            CombatStats stats = def.CreateCombatStats();
            Assert.IsNotNull(stats, "CreateCombatStats should not return null.");

            Object.Destroy(def);
        }

        [UnityTest]
        public IEnumerator CombatStats_DefaultMaxHealth_IsPositive()
        {
            CardDefinition def = ScriptableObject.CreateInstance<CardDefinition>();
            def.SetId("fighter_default");
            def.SetDisplayName("Default Fighter");

            yield return null;

            CombatStats stats = def.CreateCombatStats();
            // MaxHealth is a Stat wrapper — use .Value for the raw int.
            Assert.IsTrue(stats.MaxHealth.Value > 0,
                $"Default MaxHealth.Value should be > 0, got {stats.MaxHealth.Value}.");

            Object.Destroy(def);
        }

        [UnityTest]
        public IEnumerator CardInstance_CurrentHealth_IsZero_BeforeInitialize()
        {
            var go = new GameObject("TestCard");
            go.AddComponent<MeshRenderer>();
            go.AddComponent<BoxCollider>();
            CardInstance card = go.AddComponent<CardInstance>();

            yield return null;

            Assert.AreEqual(0, card.CurrentHealth,
                "CurrentHealth should be 0 before Initialize is called.");

            Object.Destroy(go);
        }
    }
}
