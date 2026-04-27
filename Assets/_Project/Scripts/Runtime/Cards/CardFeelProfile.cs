using UnityEngine;
using DG.Tweening;

namespace Markyu.LastKernel
{
    /// <summary>
    /// Centralized tunable settings for card interaction feel.
    ///
    /// Safe pixel-art profile:
    /// - No hover mouse tilt by default to prevent mesh overlap/z-fighting.
    /// - No idle auto-tilt by default.
    /// - No UV parallax/drift by default.
    /// - Uses scale, lift, glow, flash, and small drag tilt for Balatro-like snap.
    /// </summary>
    [CreateAssetMenu(menuName = "Last Kernel/Card Feel Profile", fileName = "CardFeelProfile")]
    public class CardFeelProfile : ScriptableObject
    {
        [Header("Hover")]
        [SerializeField, Tooltip("Uniform scale when cursor is over the card.")]
        private float hoverScale = 1.035f;

        [SerializeField, Min(0.02f)]
        private float hoverScaleDuration = 0.075f;

        [SerializeField]
        private Ease hoverScaleEase = Ease.OutQuad;

        [Header("Hover Rotation Punch")]
        [SerializeField, Tooltip("Small yaw snap when cursor enters. Keep low to avoid mesh overlap.")]
        private float hoverPunchAngle = 2f;

        [SerializeField, Min(0.05f)]
        private float hoverPunchDuration = 0.14f;

        [Header("Pickup")]
        [SerializeField]
        private float pickupPunchAmount = 0.075f;

        [SerializeField, Min(0.04f)]
        private float pickupPunchDuration = 0.10f;

        [SerializeField, Range(1, 20)]
        private int pickupPunchVibrato = 4;

        [SerializeField]
        private float dragHoldScale = 1.035f;

        [Header("Drag Tilt")]
        [SerializeField, Tooltip("Keep small for pixel-art cards to avoid mesh clipping.")]
        private float dragTiltMax = 3f;

        [SerializeField]
        private float dragTiltStrength = 1.25f;

        [SerializeField]
        private float dragTiltSmoothing = 16f;

        [Header("Mouse Tilt")]
        [SerializeField, Tooltip("Disabled by default because hover tilt can cause mesh overlap/z-fighting.")]
        private bool mouseTiltEnabled = false;

        [SerializeField, Range(0f, 40f)]
        private float mouseTiltAmount = 0f;

        [SerializeField]
        private float mouseTiltSmoothing = 20f;

        [Header("Idle Auto-Tilt")]
        [SerializeField, Tooltip("Disabled by default to keep stacked cards visually stable.")]
        private bool autoTiltEnabled = false;

        [SerializeField, Range(0f, 5f)]
        private float autoTiltAmount = 0f;

        [SerializeField, Range(0.1f, 3f)]
        private float autoTiltFrequency = 0.8f;

        [Header("Drop / Settle")]
        [SerializeField]
        private float dropSquishScale = 0.965f;

        [SerializeField, Min(0.03f)]
        private float dropSquishDuration = 0.045f;

        [SerializeField, Min(0.05f)]
        private float dropSettleDuration = 0.09f;

        [SerializeField]
        private Ease dropSettleEase = Ease.OutQuad;

        [SerializeField, Range(0f, 2f)]
        private float dropSettleOvershoot = 0f;

        [Header("Spawn")]
        [SerializeField]
        private float spawnStartScale = 0.85f;

        [SerializeField, Min(0.05f)]
        private float spawnDuration = 0.12f;

        [SerializeField]
        private Ease spawnEase = Ease.OutBack;

        [SerializeField, Range(0f, 2.5f)]
        private float spawnOvershoot = 0.45f;

        [Header("Merge / Stack Accept")]
        [SerializeField]
        private float mergePunchAmount = 0.055f;

        [SerializeField, Min(0.05f)]
        private float mergePunchDuration = 0.12f;

        [SerializeField, Range(1, 20)]
        private int mergePunchVibrato = 4;

        [Header("Shader Feedback")]
        [SerializeField, Range(0f, 1f)]
        private float hoverFlashAmount = 0.05f;

        [SerializeField, Range(0f, 1f)]
        private float pickupFlashAmount = 0.10f;

        [SerializeField, Range(0f, 1f)]
        private float mergeFlashAmount = 0.13f;

        [SerializeField, Range(0f, 1f)]
        private float damageFlashAmount = 0.55f;

        [SerializeField, Min(0.03f)]
        private float flashReturnDuration = 0.13f;

        [SerializeField, Range(0f, 0.25f), Tooltip("Raises hovered/dragged cards so small tilt cannot clip nearby meshes.")]
        private float hoverLiftAmount = 0.12f;

        [SerializeField, Range(0f, 0.25f), Tooltip("Keep 0 for pixel-art cards. UV movement can create seams.")]
        private float overlayParallaxAmount = 0f;

        [SerializeField, Range(0f, 0.1f), Tooltip("Keep 0 for pixel-art cards. UV drift can create crawling seams.")]
        private float idleOverlayDriftAmount = 0f;

        [SerializeField, Range(0.1f, 3f)]
        private float idleOverlayDriftFrequency = 0.7f;

        [SerializeField, Range(0f, 0.4f)]
        private float hoverBrightnessBoost = 0.06f;

        [SerializeField, Range(0f, 0.4f)]
        private float dragBrightnessBoost = 0.10f;

        [SerializeField, Range(0f, 0.4f)]
        private float hoverSaturationBoost = 0.02f;

        [SerializeField, Range(0f, 0.15f), Tooltip("Keep 0 for stable pixel-art colors.")]
        private float idleHueShiftAmount = 0f;

        [SerializeField, Range(0.1f, 3f)]
        private float idleHueShiftFrequency = 0.45f;

        [SerializeField]
        private Color glowColor = new Color(0.52f, 0.85f, 1f, 1f);

        [SerializeField, Range(0f, 1f)]
        private float hoverGlowIntensity = 0.08f;

        [SerializeField, Range(0f, 1f)]
        private float dragGlowIntensity = 0.14f;

        [Header("Damage Feedback")]
        [SerializeField, Range(0f, 30f)]
        private float damagePunchAngle = 6f;

        [SerializeField, Min(0.05f)]
        private float damagePunchDuration = 0.13f;

        [Header("Movement Snap")]
        [SerializeField]
        private float snapDuration = 0.10f;

        [SerializeField]
        private Ease snapEase = Ease.OutQuad;

        [SerializeField, Range(0f, 2f)]
        private float snapOvershoot = 0f;

        public float HoverScale => hoverScale;
        public float HoverScaleDuration => hoverScaleDuration;
        public Ease HoverScaleEase => hoverScaleEase;

        public float HoverPunchAngle => hoverPunchAngle;
        public float HoverPunchDuration => hoverPunchDuration;

        public float PickupPunchAmount => pickupPunchAmount;
        public float PickupPunchDuration => pickupPunchDuration;
        public int PickupPunchVibrato => pickupPunchVibrato;
        public float DragHoldScale => dragHoldScale;

        public float DragTiltMax => dragTiltMax;
        public float DragTiltStrength => dragTiltStrength;
        public float DragTiltSmoothing => dragTiltSmoothing;

        public bool MouseTiltEnabled => mouseTiltEnabled;
        public float MouseTiltAmount => mouseTiltAmount;
        public float MouseTiltSmoothing => mouseTiltSmoothing;

        public bool AutoTiltEnabled => autoTiltEnabled;
        public float AutoTiltAmount => autoTiltAmount;
        public float AutoTiltFrequency => autoTiltFrequency;

        public float DropSquishScale => dropSquishScale;
        public float DropSquishDuration => dropSquishDuration;
        public float DropSettleDuration => dropSettleDuration;
        public Ease DropSettleEase => dropSettleEase;
        public float DropSettleOvershoot => dropSettleOvershoot;

        public float SpawnStartScale => spawnStartScale;
        public float SpawnDuration => spawnDuration;
        public Ease SpawnEase => spawnEase;
        public float SpawnOvershoot => spawnOvershoot;

        public float MergePunchAmount => mergePunchAmount;
        public float MergePunchDuration => mergePunchDuration;
        public int MergePunchVibrato => mergePunchVibrato;

        public float HoverLiftAmount => hoverLiftAmount;
        public float HoverFlashAmount => hoverFlashAmount;
        public float PickupFlashAmount => pickupFlashAmount;
        public float MergeFlashAmount => mergeFlashAmount;
        public float DamageFlashAmount => damageFlashAmount;
        public float FlashReturnDuration => flashReturnDuration;

        public float OverlayParallaxAmount => overlayParallaxAmount;
        public float IdleOverlayDriftAmount => idleOverlayDriftAmount;
        public float IdleOverlayDriftFrequency => idleOverlayDriftFrequency;

        public float HoverBrightnessBoost => hoverBrightnessBoost;
        public float DragBrightnessBoost => dragBrightnessBoost;
        public float HoverSaturationBoost => hoverSaturationBoost;
        public float IdleHueShiftAmount => idleHueShiftAmount;
        public float IdleHueShiftFrequency => idleHueShiftFrequency;

        public Color GlowColor => glowColor;
        public float HoverGlowIntensity => hoverGlowIntensity;
        public float DragGlowIntensity => dragGlowIntensity;

        public float DamagePunchAngle => damagePunchAngle;
        public float DamagePunchDuration => damagePunchDuration;

        public float SnapDuration => snapDuration;
        public Ease SnapEase => snapEase;
        public float SnapOvershoot => snapOvershoot;

        private void OnValidate()
        {
            hoverScale = Mathf.Max(0.01f, hoverScale);
            hoverScaleDuration = Mathf.Max(0.02f, hoverScaleDuration);
            hoverPunchDuration = Mathf.Max(0.05f, hoverPunchDuration);

            pickupPunchDuration = Mathf.Max(0.04f, pickupPunchDuration);
            dragHoldScale = Mathf.Max(0.01f, dragHoldScale);

            dragTiltMax = Mathf.Clamp(dragTiltMax, 0f, 6f);
            dragTiltStrength = Mathf.Max(0f, dragTiltStrength);
            dragTiltSmoothing = Mathf.Max(0.01f, dragTiltSmoothing);

            // Pixel-art safety: hover tilt and idle tilt are allowed,
            // but this profile keeps them conservative to prevent mesh overlap.
            mouseTiltAmount = Mathf.Clamp(mouseTiltAmount, 0f, 8f);
            mouseTiltSmoothing = Mathf.Max(0.01f, mouseTiltSmoothing);
            autoTiltAmount = Mathf.Clamp(autoTiltAmount, 0f, 1f);
            autoTiltFrequency = Mathf.Max(0.01f, autoTiltFrequency);

            dropSquishDuration = Mathf.Max(0.03f, dropSquishDuration);
            dropSettleDuration = Mathf.Max(0.05f, dropSettleDuration);
            dropSettleOvershoot = Mathf.Clamp(dropSettleOvershoot, 0f, 0.75f);

            spawnStartScale = Mathf.Clamp(spawnStartScale, 0.5f, 1f);
            spawnDuration = Mathf.Max(0.05f, spawnDuration);
            spawnOvershoot = Mathf.Clamp(spawnOvershoot, 0f, 0.75f);

            mergePunchDuration = Mathf.Max(0.05f, mergePunchDuration);
            flashReturnDuration = Mathf.Max(0.03f, flashReturnDuration);

            // Hard safety limits for the artifacts you saw.
            overlayParallaxAmount = 0f;
            idleOverlayDriftAmount = 0f;
            idleHueShiftAmount = Mathf.Clamp(idleHueShiftAmount, 0f, 0.02f);

            damagePunchDuration = Mathf.Max(0.05f, damagePunchDuration);
            snapDuration = Mathf.Max(0.01f, snapDuration);
            snapOvershoot = Mathf.Clamp(snapOvershoot, 0f, 0.5f);
        }
    }
}