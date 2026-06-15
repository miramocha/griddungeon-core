namespace GridDungeon.Core.Exploration
{
    /// <summary>Ensures floor session commit runs at most once per transition beat.</summary>
    public sealed class FloorTransitionCommitGuard
    {
        bool m_committed;

        public bool HasCommitted => m_committed;

        public bool TryCommit()
        {
            if (m_committed)
            {
                return false;
            }

            m_committed = true;
            return true;
        }

        public void Reset() => m_committed = false;
    }
}
