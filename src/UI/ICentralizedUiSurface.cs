using System;

namespace GridDungeon.UI
{
    /// <summary>
    /// Transition-agnostic lifecycle for centralized UITK overlays (#207).
    /// Intent and settled state only — no PopIn, slide, or USS details on this surface.
    /// </summary>
    public interface ICentralizedUiSurface
    {
        /// <summary>Authority wants the panel open.</summary>
        bool RequestedVisible { get; }

        /// <summary>Panel is on-screen for this authority (true during exit animation until hide completes).</summary>
        bool IsShown { get; }

        /// <summary>Animated dismiss in flight after <see cref="Hide"/> (<see cref="Show"/> clears).</summary>
        bool IsSettling { get; }

        event Action? PresentationChanged;

        void Show();

        /// <summary>Player / same-authority dismiss (may settle).</summary>
        void Hide();

        /// <summary>Phase exit, context swap, competing overlay — cancel deferred callbacks.</summary>
        void HideImmediate();
    }
}
