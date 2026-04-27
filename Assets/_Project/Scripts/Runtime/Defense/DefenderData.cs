// DefenderData — ScriptableObject blueprint for a defender/tower unit.
//
// One asset per defender type (KernelGuard, FirewallNode, SignalPinger, …).
// Runtime DefenderUnit components reference this asset for their stats;
// the asset itself is never modified at runtime.

using UnityEngine;

namespace Markyu.LastKernel
{
    /// <summary>
    /// Immutable data asset that defines a defender's combat stats and presentation.
    /// Assign assets in the Inspector; create via
    /// Assets → Create → Last Kernel → Defense → Defender Data.
    /// </summary>
    [CreateAssetMenu(menuName = "Last Kernel/Defense/Defender Data", fileName = "Defender_")]
    public class DefenderData : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string displayName = "Defender";
        [SerializeField, TextArea(2, 4)] private string description;
        [SerializeField] private Sprite sprite;

        [Header("Combat")]
        [SerializeField, Min(1)]  private int maxHealth    = 30;
        [SerializeField, Min(1)]  private int attackDamage = 5;
        [SerializeField, Min(0.1f), Tooltip("Attacks per second.")]
        private float attackRate = 1f;
        [SerializeField, Min(0.5f), Tooltip("Attack radius in world units.")]
        private float range = 3f;

        [Header("Targeting")]
        [SerializeField, Tooltip("Prefer the enemy closest to the base (highest threat).")]
        private bool targetClosestToBase = true;

        public string DisplayName   => displayName;
        public string Description   => description;
        public Sprite Sprite        => sprite;
        public int    MaxHealth     => maxHealth;
        public int    AttackDamage  => attackDamage;
        public float  AttackRate    => attackRate;
        public float  Range         => range;
        public bool   TargetClosestToBase => targetClosestToBase;
    }
}
