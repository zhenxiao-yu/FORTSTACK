using UnityEngine;

namespace Markyu.LastKernel
{
    /// <summary>
    /// Applies mobile-specific startup configuration: landscape orientation lock,
    /// target frame rate, screen-sleep prevention, and lightweight quality defaults.
    /// Has no effect on desktop platforms.
    ///
    /// Place on the GameDirector or a persistent manager GameObject so it runs
    /// before other systems initialise. Set Script Execution Order to -50 or lower
    /// if you need it to fire before other Awake() calls.
    /// </summary>
    public class MobileStartup : MonoBehaviour
    {
        [Header("Orientation")]
        [Tooltip("Lock to landscape on mobile. The card board is designed for landscape layout.")]
        [SerializeField] private bool lockLandscape = true;

        [Header("Performance")]
        [Tooltip("Target frame rate on mobile. 60 is a safe default; reduce to 30 for battery-focused devices.")]
        [SerializeField] private int mobileTargetFPS = 60;

        [Tooltip("Keep screen on while the app is active. Recommended for strategy games.")]
        [SerializeField] private bool preventScreenSleep = true;

        [Tooltip("Shadow distance cap on mobile. Lower values improve GPU performance on mid-range devices.")]
        [SerializeField] private float mobileShadowDistance = 15f;

        private void Awake()
        {
            if (!IsMobile()) return;

            ApplyOrientation();
            ApplyPerformance();
            ApplyQuality();
        }

        private void ApplyOrientation()
        {
            if (!lockLandscape) return;

            Screen.orientation = ScreenOrientation.LandscapeLeft;
            Screen.autorotateToLandscapeLeft       = true;
            Screen.autorotateToLandscapeRight      = true;
            Screen.autorotateToPortrait            = false;
            Screen.autorotateToPortraitUpsideDown  = false;
        }

        private void ApplyPerformance()
        {
            if (mobileTargetFPS > 0)
                Application.targetFrameRate = mobileTargetFPS;

            if (preventScreenSleep)
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        private void ApplyQuality()
        {
            // Reduce shadow cascade count and distance; the card game rarely needs
            // long-range shadows and mobile GPUs are shadow-bandwidth limited.
            QualitySettings.shadowDistance = mobileShadowDistance;
            QualitySettings.shadowCascades = 1;
        }

        private static bool IsMobile()
        {
#if UNITY_ANDROID || UNITY_IOS
            return true;
#else
            return Application.isMobilePlatform;
#endif
        }
    }
}
