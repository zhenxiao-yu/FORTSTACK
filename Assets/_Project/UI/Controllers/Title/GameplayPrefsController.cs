using UnityEngine;
using UnityEngine.UIElements;

namespace Markyu.LastKernel
{
    /// <summary>
    /// Controls the Gameplay Preferences sub-panel (#panel-gameplay-prefs).
    /// On confirm, raises UIEventBus.OnStartNewGame with the selected settings.
    /// </summary>
    public sealed class GameplayPrefsController : UIToolkitComponentController
    {
        private readonly ModalController _modal;

        private Label   _titleLabel;
        private Label   _durationLabel;
        private Slider  _durationSlider;
        private Toggle  _friendlyToggle;
        private Label   _friendlyLabel;
        private Button  _cancelButton;
        private Button  _confirmButton;

        public GameplayPrefsController(ModalController modal)
        {
            _modal = modal;
        }

        // ── Binding ────────────────────────────────────────────────────────────

        protected override void OnBind()
        {
            _titleLabel     = Root.Q<Label>  ("lbl-prefs-title");
            _durationLabel  = Root.Q<Label>  ("lbl-duration");
            _durationSlider = Root.Q<Slider> ("slider-duration");
            _friendlyToggle = Root.Q<Toggle> ("toggle-friendly");
            _friendlyLabel  = Root.Q<Label>  ("lbl-friendly");
            _cancelButton   = Root.Q<Button> ("btn-prefs-cancel");
            _confirmButton  = Root.Q<Button> ("btn-prefs-confirm");

            _durationSlider.RegisterValueChangedCallback(evt =>
                UpdateDurationLabel(Mathf.RoundToInt(evt.newValue)));

            _friendlyToggle.RegisterValueChangedCallback(evt =>
                UpdateFriendlyLabel(evt.newValue));

            _cancelButton.clicked  += Hide;
            _confirmButton.clicked += StartNewGame;
        }

        // ── API ────────────────────────────────────────────────────────────────

        public void Show()
        {
            Root.RemoveFromClassList("lk-hidden");
            OnLocalizationRefresh();
        }

        public void Hide() => Root.AddToClassList("lk-hidden");

        // ── Localization ───────────────────────────────────────────────────────

        public override void OnLocalizationRefresh()
        {
            if (_titleLabel    != null) _titleLabel.text    = GameLocalization.Get("title.gameplayHeader");
            if (_cancelButton  != null) _cancelButton.text  = GameLocalization.Get("common.cancelButton");
            if (_confirmButton != null) _confirmButton.text = GameLocalization.Get("common.confirmButton");

            if (_friendlyToggle != null)
            {
                _friendlyToggle.label = GameLocalization.Get("gameplay.friendlyModeLabel");
                UpdateFriendlyLabel(_friendlyToggle.value);
            }
            if (_durationSlider != null) UpdateDurationLabel(Mathf.RoundToInt(_durationSlider.value));
        }

        // ── Private ────────────────────────────────────────────────────────────

        private void UpdateDurationLabel(int duration)
        {
            if (_durationLabel != null)
                _durationLabel.text = GameLocalization.Format("gameplay.dayDuration", duration);
        }

        private void UpdateFriendlyLabel(bool isFriendly)
        {
            if (_friendlyLabel != null)
                _friendlyLabel.text = isFriendly
                    ? GameLocalization.Get("gameplay.friendlyOn")
                    : GameLocalization.Get("gameplay.friendlyOff");
        }

        private void StartNewGame()
        {
            int  dayDuration = Mathf.RoundToInt(_durationSlider.value);
            bool isFriendly  = _friendlyToggle.value;
            UIEventBus.RaiseStartNewGame(new GameplayPrefs(dayDuration, isFriendly));
            Hide();
        }
    }
}
