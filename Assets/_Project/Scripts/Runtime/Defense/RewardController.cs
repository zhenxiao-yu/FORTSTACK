// RewardController — Distributes victory rewards and shows the reward panel.
//
// Called by NightHUD (or any listener of DefensePhaseController.OnPhaseChanged == Victory).
// Keeps reward logic centralised so it's easy to expand (e.g. add XP, unlock cards, etc.).

using UnityEngine;

namespace Markyu.LastKernel
{
    /// <summary>
    /// Applies rewards from a <see cref="RewardData"/> asset to the run state
    /// and notifies the UI so the reward panel can be shown.
    /// </summary>
    public class RewardController : MonoBehaviour
    {
        [SerializeField] private RewardData defaultReward;

        public event System.Action<RewardData> OnRewardsReady;

        private void OnEnable()
        {
            if (DefensePhaseController.Instance != null)
                DefensePhaseController.Instance.OnPhaseChanged += HandlePhaseChanged;
        }

        private void OnDisable()
        {
            if (DefensePhaseController.Instance != null)
                DefensePhaseController.Instance.OnPhaseChanged -= HandlePhaseChanged;
        }

        private void HandlePhaseChanged(DefensePhase phase)
        {
            if (phase == DefensePhase.Victory)
                GrantRewards(defaultReward);
        }

        /// <summary>Apply rewards to run state and fire the UI event.</summary>
        public void GrantRewards(RewardData reward)
        {
            if (reward == null) reward = defaultReward;
            if (reward == null)
            {
                Debug.LogWarning("[RewardController] No RewardData assigned.", this);
                OnRewardsReady?.Invoke(null);
                return;
            }

            // Apply scrap to run state — wire to RunStateManager / CardManager currency when economy is implemented.
            // If reward.RewardPack != null, spawn a PackInstance on the board.

            OnRewardsReady?.Invoke(reward);
        }
    }
}
