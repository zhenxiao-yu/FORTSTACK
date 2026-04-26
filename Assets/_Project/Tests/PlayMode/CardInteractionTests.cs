using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Markyu.LastKernel.Tests
{
    /// <summary>
    /// Tests CardInstance interaction state (drag, hover flags).
    /// </summary>
    public class CardInteractionTests
    {
        private GameObject _go;

        [TearDown]
        public void TearDown()
        {
            if (_go != null) Object.Destroy(_go);
        }

        [UnityTest]
        public IEnumerator IsBeingDragged_CanBeSetTrue()
        {
            _go = CreateCard();
            CardInstance card = _go.GetComponent<CardInstance>();

            yield return null;

            card.IsBeingDragged = true;
            Assert.IsTrue(card.IsBeingDragged);
        }

        [UnityTest]
        public IEnumerator IsBeingDragged_CanBeCleared()
        {
            _go = CreateCard();
            CardInstance card = _go.GetComponent<CardInstance>();

            yield return null;

            card.IsBeingDragged = true;
            card.IsBeingDragged = false;
            Assert.IsFalse(card.IsBeingDragged);
        }

        [UnityTest]
        public IEnumerator CardInstance_StackIsNull_WhenNotAttached()
        {
            _go = CreateCard();
            CardInstance card = _go.GetComponent<CardInstance>();

            yield return null;

            Assert.IsNull(card.Stack, "Stack should be null on a freshly created card.");
        }

        private static GameObject CreateCard()
        {
            var go = new GameObject("TestCard");
            go.AddComponent<MeshRenderer>();
            go.AddComponent<BoxCollider>();
            go.AddComponent<CardInstance>();
            return go;
        }
    }
}
