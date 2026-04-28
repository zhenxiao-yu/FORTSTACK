using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace Markyu.LastKernel
{
    /// <summary>
    /// Scene-level entry point for the UI Toolkit layer.
    /// Owns the UIDocument, applies the shared theme, and routes show/hide
    /// calls to registered screen controllers.
    ///
    /// Does not own game logic. One instance per scene is expected.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class UIToolkitRootController : MonoBehaviour
    {
        [BoxGroup("Theme")]
        [SerializeField, Required]
        private UITheme theme;

        private UIDocument document;
        private readonly Dictionary<string, UIToolkitScreenController> screens = new();

        public VisualElement Root => document?.rootVisualElement;

        // ── Lifecycle ──────────────────────────────────────────────────────────

        private void Awake()
        {
            document = GetComponent<UIDocument>();

            if (theme != null && Root != null)
                theme.ApplyTo(Root);
        }

        // ── Screen registry ────────────────────────────────────────────────────

        /// <summary>
        /// Called by UIToolkitScreenControllers in their Awake to register themselves.
        /// </summary>
        public void RegisterScreen(string screenId, UIToolkitScreenController screen)
        {
            if (!string.IsNullOrEmpty(screenId))
                screens[screenId] = screen;
        }

        public void ShowScreen(string screenId)
        {
            if (screens.TryGetValue(screenId, out var screen))
                screen.Show();
        }

        public void HideScreen(string screenId)
        {
            if (screens.TryGetValue(screenId, out var screen))
                screen.Hide();
        }

        public void HideAllScreens()
        {
            foreach (var screen in screens.Values)
                screen.Hide();
        }

        public bool IsScreenVisible(string screenId)
        {
            return screens.TryGetValue(screenId, out var screen) && screen.IsVisible;
        }

        // ── Editor helpers ─────────────────────────────────────────────────────

        [Button("Apply Theme (Runtime Only)"), PropertyOrder(100)]
        private void EditorApplyTheme()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[UIToolkitRootController] Theme can only be applied at runtime.");
                return;
            }

            if (theme != null && Root != null)
                theme.ApplyTo(Root);
        }
    }
}
