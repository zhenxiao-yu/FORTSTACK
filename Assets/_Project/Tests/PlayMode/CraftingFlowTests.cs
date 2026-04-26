using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Markyu.LastKernel.Tests
{
    /// <summary>
    /// Tests RecipeMatcher logic in PlayMode. Tests are self-contained and
    /// do not require a full game scene.
    /// </summary>
    public class CraftingFlowTests
    {
        private readonly List<Object> _created = new();

        [TearDown]
        public void TearDown()
        {
            foreach (Object obj in _created) Object.Destroy(obj);
            _created.Clear();
        }

        [UnityTest]
        public IEnumerator RecipeMatcher_ReturnsEmptyList_WhenRecipeListEmpty()
        {
            CardDefinition card = MakeCard("test", CardCategory.Material);
            var recipes = new List<RecipeDefinition>();

            yield return null;

            List<RecipeDefinition> matches = RecipeMatcher.FindMatchingRecipes(new[] { card }, recipes);
            Assert.IsEmpty(matches, "No matches should be returned when recipe list is empty.");
        }

        [UnityTest]
        public IEnumerator RecipeMatcher_FindsMatch_WhenStackMatches()
        {
            CardDefinition wood = MakeCard("wood", CardCategory.Material);
            CardDefinition plank = MakeCard("plank", CardCategory.Material);
            RecipeDefinition recipe = MakeRecipe("make_plank", plank, allowExcess: false, (wood, 2));

            yield return null;

            List<RecipeDefinition> matches = RecipeMatcher.FindMatchingRecipes(new[] { wood, wood }, new List<RecipeDefinition> { recipe });
            Assert.IsNotEmpty(matches, "A matching recipe should be found.");
            Assert.AreSame(recipe, matches[0]);
        }

        [UnityTest]
        public IEnumerator CombatStats_CreatedFromDefinition_HasPositiveHealth()
        {
            CardDefinition def = MakeCard("fighter", CardCategory.Character);
            SetField(def, "maxHealth", 20);

            yield return null;

            CombatStats stats = def.CreateCombatStats();
            // MaxHealth is a Stat wrapper — use .Value for the raw int.
            Assert.IsTrue(stats.MaxHealth.Value > 0, "CombatStats should have positive MaxHealth.");
        }

        // ─── Helpers ─────────────────────────────────────────────────────────

        private CardDefinition MakeCard(string id, CardCategory category)
        {
            var card = ScriptableObject.CreateInstance<CardDefinition>();
            _created.Add(card);
            card.SetId(id);
            card.SetDisplayName(id);
            SetField(card, "category", category);
            return card;
        }

        private RecipeDefinition MakeRecipe(
            string id, CardDefinition output, bool allowExcess,
            params (CardDefinition card, int count)[] ingredients)
        {
            var recipe = ScriptableObject.CreateInstance<RecipeDefinition>();
            _created.Add(recipe);
            SetField(recipe, "id", id);
            SetField(recipe, "displayName", id);
            SetField(recipe, "resultingCard", output);
            SetField(recipe, "craftingDuration", 5f);
            SetField(recipe, "randomWeight", 1f);
            SetField(recipe, "allowExcessIngredients", allowExcess);

            var list = new List<RecipeDefinition.Ingredient>();
            foreach (var (card, count) in ingredients)
                list.Add(new RecipeDefinition.Ingredient { card = card, count = count, consumptionMode = IngredientConsumption.Consume });
            SetField(recipe, "requiredIngredients", list);
            return recipe;
        }

        private static void SetField(object target, string name, object value)
        {
            FieldInfo f = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            Assert.IsNotNull(f, $"Field '{name}' not found on {target.GetType().Name}.");
            f.SetValue(target, value);
        }
    }
}
