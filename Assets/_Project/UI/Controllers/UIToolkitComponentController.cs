using UnityEngine.UIElements;

namespace Markyu.LastKernel
{
    /// <summary>
    /// Base class for reusable UI Toolkit component controllers (buttons, panels, counters, etc.).
    ///
    /// Not a MonoBehaviour — instantiated and owned by a UIToolkitScreenController.
    /// Call Bind(element) once after querying the VisualElement from the screen's UXML.
    ///
    /// Subclasses expose a SetData / Bind API and raise UIEventBus events for user intent.
    /// No game state lives here.
    /// </summary>
    public abstract class UIToolkitComponentController
    {
        protected VisualElement Root { get; private set; }
        public bool IsBound => Root != null;

        // ── Binding ────────────────────────────────────────────────────────────

        /// <summary>Binds this controller to a VisualElement queried from a parent UXML.</summary>
        public void Bind(VisualElement element)
        {
            Root = element;
            OnBind();
        }

        /// <summary>Query child elements and register event callbacks here.</summary>
        protected abstract void OnBind();

        // ── Localization ───────────────────────────────────────────────────────

        /// <summary>
        /// Update any localized labels. Called by the owning screen controller
        /// from its OnLocalizationRefresh().
        /// </summary>
        public virtual void OnLocalizationRefresh() { }
    }
}
