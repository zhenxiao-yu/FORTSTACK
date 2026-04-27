// DefenseLoadoutController — Bridge between the card system and the defense loadout.
//
// Full card integration is a future milestone. This controller provides:
//   a) A clean seam that NightBattlefieldController calls (via ResolveLoadout)
//   b) A fall-through to defenderOverrides for playtesting without card setup
//
// TODO (card integration):
//   • Read CardInstance components that have been assigned to defense slots during day.
//   • Look up DefenderData via CardDefinition.DefenderDataOverride or a mapping table.
//   • Return the resolved array in ResolveLoadout().

using UnityEngine;

namespace Markyu.LastKernel
{
    /// <summary>
    /// Resolves which DefenderData fills each defender slot for the upcoming night.
    /// Attach to the same GameObject as <see cref="NightBattlefieldController"/>.
    /// </summary>
    public class DefenseLoadoutController : MonoBehaviour
    {
        [Header("Fallback Loadout (used when card integration is not yet active)")]
        [SerializeField, Tooltip("One entry per defender slot. Null = empty slot.")]
        private DefenderData[] defenderOverrides;

        /// <summary>
        /// Returns an array of DefenderData sized to <paramref name="slotCount"/>.
        /// Each index maps to the same-indexed defender slot.
        /// Returns null entries for empty slots.
        /// </summary>
        public DefenderData[] ResolveLoadout(int slotCount)
        {
            // TODO: query the day-phase card system for prepared defender cards.
            // For now, use the manually assigned override array.
            var result = new DefenderData[slotCount];

            if (defenderOverrides == null) return result;

            for (int i = 0; i < slotCount && i < defenderOverrides.Length; i++)
                result[i] = defenderOverrides[i];

            return result;
        }
    }
}
