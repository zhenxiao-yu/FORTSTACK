using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Markyu.LastKernel.Tests
{
    /// <summary>
    /// Tests basic CardInstance component lifecycle.
    /// These tests create minimal GameObjects without requiring a full game scene.
    /// </summary>
    public class CardSpawnTests
    {
        private GameObject _go;

        [TearDown]
        public void TearDown()
        {
            if (_go != null) Object.Destroy(_go);
        }

        [UnityTest]
        public IEnumerator CardInstance_CanBeAddedToGameObject()
        {
            _go = new GameObject("TestCard");
            _go.AddComponent<MeshRenderer>();
            _go.AddComponent<BoxCollider>();
            CardInstance card = _go.AddComponent<CardInstance>();

            yield return null;

            Assert.IsNotNull(card, "CardInstance should be added without errors.");
        }

        [UnityTest]
        public IEnumerator CardInstance_IsBeingDragged_DefaultsToFalse()
        {
            _go = new GameObject("TestCard");
            _go.AddComponent<MeshRenderer>();
            _go.AddComponent<BoxCollider>();
            CardInstance card = _go.AddComponent<CardInstance>();

            yield return null;

            Assert.IsFalse(card.IsBeingDragged, "New CardInstance should not be dragging.");
        }

        [UnityTest]
        public IEnumerator CardInstance_DefinitionIsNull_BeforeInitialize()
        {
            _go = new GameObject("TestCard");
            _go.AddComponent<MeshRenderer>();
            _go.AddComponent<BoxCollider>();
            CardInstance card = _go.AddComponent<CardInstance>();

            yield return null;

            Assert.IsNull(card.Definition, "Definition should be null before Initialize is called.");
        }
    }
}
