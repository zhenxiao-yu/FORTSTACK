// EnemyUnit — Runtime MonoBehaviour for a spatially-moving enemy in the defense lane.
//
// Lifecycle:
//   1. NightBattlefieldController.SpawnEnemy() calls Initialize(data, baseTransform).
//   2. Each frame, MoveTowardBase() advances the unit along the lane.
//   3. DefenderUnit.Attack() calls TakeDamage() to reduce HP.
//   4. When HP ≤ 0, Die() fires OnDied and destroys the GameObject.
//   5. If the unit reaches the base position, OnReachedBase fires → BaseCoreController.TakeDamage().

using UnityEngine;
using System;

namespace Markyu.LastKernel
{
    /// <summary>
    /// Represents a single enemy moving through the defense lane.
    /// Spawned and owned by <see cref="NightBattlefieldController"/>.
    /// </summary>
    [DisallowMultipleComponent]
    public class EnemyUnit : MonoBehaviour
    {
        // ── State ─────────────────────────────────────────────────────────────

        public EnemyDefinition Data       { get; private set; }
        public int             CurrentHP  { get; private set; }
        public bool            IsDead     { get; private set; }

        /// <summary>Base transform this enemy moves toward. Set by Initialize().</summary>
        private Transform _baseTarget;

        // ── Events ────────────────────────────────────────────────────────────

        /// <summary>Fired when HP drops to zero. Receiver should clean up the unit.</summary>
        public event Action<EnemyUnit> OnDied;

        /// <summary>Fired once when the enemy reaches the base (before self-destruct).</summary>
        public event Action<EnemyUnit> OnReachedBase;

        // ── Visuals (assigned by NightBattlefieldController on spawn) ─────────

        private SpriteRenderer _spriteRenderer;

        // ── Initialisation ────────────────────────────────────────────────────

        /// <summary>
        /// Must be called immediately after Instantiate().
        /// </summary>
        public void Initialize(EnemyDefinition data, Transform baseTarget)
        {
            Data        = data;
            CurrentHP   = data.MaxHP;
            _baseTarget = baseTarget;
            IsDead      = false;

            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer != null && data.Sprite != null)
                _spriteRenderer.sprite = data.Sprite;
        }

        // ── Movement ──────────────────────────────────────────────────────────

        private void Update()
        {
            if (IsDead || _baseTarget == null) return;
            MoveTowardBase();
        }

        private void MoveTowardBase()
        {
            Vector3 direction = (_baseTarget.position - transform.position).normalized;
            transform.position += direction * (Data.MoveSpeed * Time.deltaTime);

            // Reached base — deal contact damage and remove self
            float distToBase = Vector3.Distance(transform.position, _baseTarget.position);
            if (distToBase < 0.25f)
            {
                OnReachedBase?.Invoke(this);
                DestroyUnit();
            }
        }

        // ── Combat ────────────────────────────────────────────────────────────

        /// <summary>Called by DefenderUnit when it fires an attack at this enemy.</summary>
        public void TakeDamage(int damage)
        {
            if (IsDead) return;

            CurrentHP -= damage;
            UpdateHealthVisual();

            if (CurrentHP <= 0)
                Die();
        }

        private void Die()
        {
            IsDead = true;
            OnDied?.Invoke(this);
            // TODO: play death VFX before destroying
            DestroyUnit();
        }

        private void DestroyUnit()
        {
            IsDead = true; // guard against double-destroy
            Destroy(gameObject);
        }

        // ── Visual feedback ───────────────────────────────────────────────────

        private void UpdateHealthVisual()
        {
            if (_spriteRenderer == null) return;

            // Tint red as health depletes — simple but readable in playtesting
            float healthFraction = Mathf.Clamp01((float)CurrentHP / Data.MaxHP);
            _spriteRenderer.color = Color.Lerp(Color.red, Color.white, healthFraction);
        }
    }
}
