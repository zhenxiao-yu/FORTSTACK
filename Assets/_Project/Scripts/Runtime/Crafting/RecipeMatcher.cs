using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Markyu.LastKernel
{
    /// <summary>
    /// Pure, stateless recipe-matching logic shared between runtime crafting and edit-mode tests.
    /// </summary>
    /// <remarks>
    /// Intentionally has no Unity or manager dependencies so it can be called from
    /// edit-mode tests without entering play mode. All state lives in the caller.
    ///
    /// Matching rules summary:
    ///   • AllowExcessIngredients recipes (workstations): stack must meet minimums; extra cards OK.
    ///   • Resource-category cards always allow excess count regardless of recipe flag.
    ///   • Strict recipes: card counts must exactly equal requirements; no extra card types allowed.
    /// </remarks>
    public static class RecipeMatcher
    {
        public static List<RecipeDefinition> FindMatchingRecipes(
            IEnumerable<CardDefinition> stackCards,
            IEnumerable<RecipeDefinition> recipes)
        {
            return recipes?
                .Where(recipe => DoesStackMatchRecipe(stackCards, recipe))
                .ToList() ?? new List<RecipeDefinition>();
        }

        public static bool DoesStackMatchRecipe(IEnumerable<CardDefinition> stackCards, RecipeDefinition recipe)
        {
            if (recipe == null)
            {
                return false;
            }

            var recipeIngredients = recipe.RequiredIngredients;
            if (recipeIngredients == null || recipeIngredients.Count == 0)
            {
                return false;
            }

            var stackComposition = BuildCounts(stackCards);
            if (stackComposition.Count == 0)
            {
                return false;
            }

            var requiredCounts = BuildRequiredCounts(recipeIngredients);
            if (requiredCounts.Count == 0)
            {
                return false;
            }

            foreach (var requirement in requiredCounts)
            {
                var card = requirement.Key;
                int requiredCount = requirement.Value;

                if (!stackComposition.TryGetValue(card, out int countInStack))
                {
                    return false;
                }

                // Resource-category cards (Wood, Stone, Food…) are generic accumulating inputs.
                // They are always allowed to exceed the required count so a player's stockpile
                // doesn't block a recipe that only needs a portion of what's in the stack.
                bool allowsExtraCount = recipe.AllowExcessIngredients || card.Category == CardCategory.Resource;
                if (allowsExtraCount)
                {
                    if (countInStack < requiredCount)
                    {
                        return false;
                    }
                }
                else if (countInStack != requiredCount)
                {
                    return false;
                }
            }

            if (recipe.AllowExcessIngredients)
            {
                return true;
            }

            // Strict mode: every card type in the stack must be a declared ingredient.
            // Prevents accidental recipe triggers when an unrelated card is dropped
            // onto a stack that would otherwise satisfy a recipe's ingredient list.
            return stackComposition.Keys.All(card => requiredCounts.ContainsKey(card));
        }

        // Deterministic overload — caller supplies the pre-rolled value.
        // Used in tests and replay scenarios where a controlled seed is required.
        public static RecipeDefinition PickWeightedRecipe(IReadOnlyList<RecipeDefinition> recipes, float roll)
        {
            if (recipes == null || recipes.Count == 0)
            {
                return null;
            }

            float totalWeight = GetTotalPositiveWeight(recipes);
            if (totalWeight <= 0f)
            {
                return recipes[0];
            }

            float remaining = Mathf.Clamp(roll, 0f, totalWeight);
            foreach (var recipe in recipes)
            {
                float weight = recipe != null ? recipe.RandomWeight : 0f;
                if (weight <= 0f)
                {
                    continue;
                }

                remaining -= weight;
                if (remaining <= 0f)
                {
                    return recipe;
                }
            }

            return recipes.LastOrDefault(recipe => recipe != null);
        }

        // Convenience overload for the typical runtime call-site in CraftingManager.
        // Rolls Unity's random internally; prefer the deterministic overload in tests.
        public static RecipeDefinition PickRandomWeightedRecipe(IReadOnlyList<RecipeDefinition> recipes)
        {
            if (recipes == null || recipes.Count == 0)
            {
                return null;
            }

            float totalWeight = GetTotalPositiveWeight(recipes);
            if (totalWeight <= 0f)
            {
                return recipes[Random.Range(0, recipes.Count)];
            }

            return PickWeightedRecipe(recipes, Random.Range(0f, totalWeight));
        }

        private static Dictionary<CardDefinition, int> BuildCounts(IEnumerable<CardDefinition> cards)
        {
            return cards?
                .Where(card => card != null)
                .GroupBy(card => card)
                .ToDictionary(group => group.Key, group => group.Count())
                ?? new Dictionary<CardDefinition, int>();
        }

        private static Dictionary<CardDefinition, int> BuildRequiredCounts(IEnumerable<RecipeDefinition.Ingredient> ingredients)
        {
            return ingredients
                .Where(ingredient => ingredient.card != null && ingredient.count > 0)
                .GroupBy(ingredient => ingredient.card)
                .ToDictionary(group => group.Key, group => group.Sum(ingredient => ingredient.count));
        }

        private static float GetTotalPositiveWeight(IEnumerable<RecipeDefinition> recipes)
        {
            return recipes.Sum(recipe => recipe != null ? Mathf.Max(0f, recipe.RandomWeight) : 0f);
        }
    }
}
