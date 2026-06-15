namespace GridDungeon.Core.Hub
{
    /// <summary>Debounced settle for hub root-menu focus before Cinemachine pan (ADR 033).</summary>
    public sealed class HubEnvironmentFocusDebouncer
    {
        HubRootMenuSlot? m_pending;
        float m_settleUntil = float.NegativeInfinity;

        public void NotifyFocus(
            HubRootMenuSlot slot,
            bool focusable,
            float nowSeconds,
            float settleSeconds
        )
        {
            if (!focusable)
            {
                Clear();
                return;
            }

            m_pending = slot;
            m_settleUntil = nowSeconds + settleSeconds;
        }

        public void Clear()
        {
            m_pending = null;
            m_settleUntil = float.NegativeInfinity;
        }

        public bool TryConsumeSettledFocus(float nowSeconds, out HubRootMenuSlot slot)
        {
            slot = default;
            if (m_pending == null || nowSeconds < m_settleUntil)
            {
                return false;
            }

            slot = m_pending.Value;
            m_pending = null;
            m_settleUntil = float.NegativeInfinity;
            return true;
        }
    }
}
