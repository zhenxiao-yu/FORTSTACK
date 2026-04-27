// RewardData — ScriptableObject that describes what the player earns after a wave victory.

using UnityEngine;

namespace Markyu.LastKernel
{
    /// <summary>
    /// Defines the loot and narrative text shown on the victory reward panel.
    /// One asset per wave (or reuse a generic reward for early playtesting).
    /// </summary>
    [CreateAssetMenu(menuName = "Last Kernel/Defense/Reward Data", fileName = "Reward_")]
    public class RewardData : ScriptableObject
    {
        [Header("Currency")]
        [SerializeField, Min(0)] private int scrapAmount = 10;

        [Header("Card Pack (optional)")]
        [SerializeField, Tooltip("Leave null to skip the pack reward.")]
        private PackDefinition rewardPack;

        [Header("Presentation")]
        [SerializeField] private string rewardTitle = "Victory!";
        [SerializeField, TextArea(2, 4)] private string rewardDescription = "The attack has been repelled.";

        public int            ScrapAmount        => scrapAmount;
        public PackDefinition RewardPack         => rewardPack;
        public string         RewardTitle        => rewardTitle;
        public string         RewardDescription  => rewardDescription;
    }
}
