// DefenderUnit — Runtime MonoBehaviour for a stationary auto-attacking defender.
//
// Placement:
//   Placed at fixed world positions by NightBattlefieldController.
//   Each slot maps to a DefenderData ScriptableObject (or a card for integration).
//
// Combat loop (runs in Update):
//   1. FindTarget() — find the nearest live EnemyUnit within range.
//   2. If found and cooldown expired, call Attack(target).
//   3. AttackCooldown ticks down from (1 / attackRate) seconds.
//
// For future full-card integration, see DefenseLoadoutController.

using System.Collections.Generic;
using UnityEngine;

namespace Markyu.LastKernel
{
    /// <summary>
    /// A placed, stationary defender that automatically attacks enemies within range.
    /// </summary>
    [DisallowMultipleComponent]
    public class DefenderUnit : MonoBehaviour
    {
        // ── State ─────────────────────────────────────────────────────────────

        public DefenderData Data       { get; private set; }
        public int          CurrentHP  { get; private set; }
        public bool         IsDead     { get; private set; }

        private float _attackCooldown;    // seconds remaining before next attack

        // The battlefield controller registers all active enemies here so we can
        // query them without a Physics.OverlapSphere call every frame.
        private List<EnemyUnit> _enemyRegistry;

        private SpriteRenderer _spriteRenderer;

        // ── Initialisation ────────────────────────────────────────────────────

        /// <param name="data">Stats and presentation data.</param>
        /// <param name="enemyRegistry">Shared list maintained by NightBattlefieldController.</param>
        public void Initialize(DefenderData data, List<EnemyUnit> enemyRegistry)
        {
            Data           = data;
            CurrentHP      = data.MaxHealth;
            _enemyRegistry = enemyRegistry;
            IsDead         = false;
            _attackCooldown = 0f;

            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer != null && data.Sprite != null)
                _spriteRenderer.sprite = data.Sprite;
        }

        // ── Combat loop ───────────────────────────────────────────────────────

        private void Update()
        {
            if (IsDead) return;

            _attackCooldown -= Time.deltaTime;

            if (_attackCooldown <= 0f)
            {
                EnemyUnit target = FindTarget();
                if (target != null)
                {
                    Attack(target);
                    _attackCooldown = 1f / Data.AttackRate;
                }
            }
        }

        /// <summary>
        /// Returns the best enemy target within range.
        /// Prioritises the enemy closest to the base (highest threat) when
        /// <see cref="DefenderData.TargetClosestToBase"/> is set; otherwise closest to self.
        /// </summary>
        private EnemyUnit FindTarget()
        {
            EnemyUnit best     = null;
            float     bestDist = float.MaxValue;

            foreach (EnemyUnit enemy in _enemyRegistry)
            {
                if (enemy == null || enemy.IsDead) continue;

                float dist = Vector3.Distance(transform.position, enemy.transform.position);
                if (dist > Data.Range) continue;

                if (best == null || dist < bestDist)
                {
                    best     = enemy;
                    bestDist = dist;
                }
            }

            return best;
        }

        private void Attack(EnemyUnit target)
        {
            target.TakeDamage(Data.AttackDamage);

            // Brief scale punch as cheap attack feedback (no DOTween dependency here)
            StartCoroutine(AttackPunchRoutine());
        }

        private System.Collections.IEnumerator AttackPunchRoutine()
        {
            Vector3 original = transform.localScale;
            transform.localScale = original * 1.2f;
            yield return new WaitForSeconds(0.07f);
            transform.localScale = original;
        }
    }
}
