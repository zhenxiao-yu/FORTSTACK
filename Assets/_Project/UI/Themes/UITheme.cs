using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Markyu.LastKernel
{
    /// <summary>
    /// Central registry for UI Toolkit assets that cannot live in USS:
    /// stylesheet references, font assets, and icon sprites.
    /// Create via Assets > Create > LAST KERNEL > UI > UI Theme.
    /// </summary>
    [CreateAssetMenu(menuName = "LAST KERNEL/UI/UI Theme", fileName = "UITheme")]
    public class UITheme : ScriptableObject
    {
        [BoxGroup("Stylesheets")]
        [Required, Tooltip("Master USS: all --lk-* custom properties.")]
        public StyleSheet themeSheet;

        [BoxGroup("Stylesheets")]
        [Tooltip("Layout utilities: flex direction, padding, alignment classes.")]
        public StyleSheet layoutSheet;

        [BoxGroup("Stylesheets")]
        [Tooltip("Component styles: panels, buttons, progress bars, sliders.")]
        public StyleSheet componentsSheet;

        [BoxGroup("Stylesheets")]
        [Tooltip("Animation / transition class definitions including .lk-hidden.")]
        public StyleSheet animationsSheet;

        [BoxGroup("Fonts")]
        [Tooltip("Primary English pixel font (TMP asset).")]
        public TMP_FontAsset primaryFont;

        [BoxGroup("Fonts")]
        [Tooltip("Simplified Chinese font (TMP asset). Must support full CJK range.")]
        public TMP_FontAsset chineseFont;

        // ── Runtime API ───────────────────────────────────────────────────────

        /// <summary>Adds all assigned stylesheets to <paramref name="element"/> in dependency order.</summary>
        public void ApplyTo(VisualElement element)
        {
            if (themeSheet      != null) element.styleSheets.Add(themeSheet);
            if (layoutSheet     != null) element.styleSheets.Add(layoutSheet);
            if (componentsSheet != null) element.styleSheets.Add(componentsSheet);
            if (animationsSheet != null) element.styleSheets.Add(animationsSheet);
        }

        // ── Editor validation ─────────────────────────────────────────────────

        [Button("Validate Theme"), PropertyOrder(100)]
        private void ValidateTheme()
        {
            int missing = 0;
            Check(themeSheet,      "themeSheet");
            Check(layoutSheet,     "layoutSheet");
            Check(componentsSheet, "componentsSheet");
            Check(animationsSheet, "animationsSheet");
            Check(primaryFont,     "primaryFont");
            Check(chineseFont,     "chineseFont");

            Debug.Log(missing == 0
                ? "[UITheme] All references assigned."
                : $"[UITheme] {missing} reference(s) missing — see warnings above.");

            void Check(Object asset, string fieldName)
            {
                if (asset == null)
                {
                    Debug.LogWarning($"[UITheme] '{fieldName}' is not assigned.", this);
                    missing++;
                }
            }
        }
    }
}
