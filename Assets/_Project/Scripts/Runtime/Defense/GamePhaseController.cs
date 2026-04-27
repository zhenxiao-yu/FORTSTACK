// DefensePhaseController — Single authoritative source for the current game phase.
//
// Day systems (card board, crafting, trading) are active during Day phase.
// Night systems (defense battlefield, wave spawning) are active during NightCombat.
//
// Other scripts subscribe to OnPhaseChanged rather than polling CurrentPhase
// every frame — this avoids tight coupling and keeps phase transitions clean.
//
// Usage:
//   DefensePhaseController.Instance.OnPhaseChanged += HandlePhaseChange;
//   DefensePhaseController.Instance.StartNight();

using UnityEngine;

namespace Markyu.LastKernel
{
    /// <summary>Which broad phase of the game loop is currently active.</summary>
    public enum DefensePhase  // distinct from RunStateData.GamePhase (Dawn/Day/Dusk/Night)
    {
        Day,            // Player manages cards, crafts, explores
        NightPrep,      // Brief preparation window before enemies spawn (optional)
        NightCombat,    // Defense auto-battler is running
        Victory,        // Wave cleared — showing reward panel
        Defeat          // Base destroyed — showing game-over panel
    }

    /// <summary>
    /// Singleton that owns the current <see cref="DefensePhase"/> and broadcasts
    /// transitions to all subscribed listeners. Does not own any gameplay logic —
    /// it is a pure state machine + event hub.
    /// </summary>
    [DisallowMultipleComponent]
    public class DefensePhaseController : MonoBehaviour
    {
        public static DefensePhaseController Instance { get; private set; }

        public DefensePhase CurrentPhase { get; private set; } = DefensePhase.Day;

        /// <summary>Fired whenever the phase changes. Receives the new phase.</summary>
        public event System.Action<DefensePhase> OnPhaseChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        // ── Public transitions ─────────────────────────────────────────────────

        /// <summary>Start the night combat phase. Called by the "End Day" button.</summary>
        public void StartNight()
        {
            if (CurrentPhase != DefensePhase.Day && CurrentPhase != DefensePhase.NightPrep) return;
            SetPhase(DefensePhase.NightCombat);
        }

        /// <summary>Called by NightBattlefieldController when the wave is fully cleared.</summary>
        public void DeclareVictory() => SetPhase(DefensePhase.Victory);

        /// <summary>Called by BaseCoreController when base HP reaches zero.</summary>
        public void DeclareDefeat() => SetPhase(DefensePhase.Defeat);

        /// <summary>Called by the reward panel's "Continue" button to return to day.</summary>
        public void ReturnToDay() => SetPhase(DefensePhase.Day);

        // ── Internal ──────────────────────────────────────────────────────────

        private void SetPhase(DefensePhase next)
        {
            if (CurrentPhase == next) return;
            CurrentPhase = next;
            OnPhaseChanged?.Invoke(next);
        }
    }
}
