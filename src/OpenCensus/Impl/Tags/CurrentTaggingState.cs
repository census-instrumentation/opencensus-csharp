namespace OpenCensus.Tags
{
    using System;

    public sealed class CurrentTaggingState
    {

        private TaggingState currentState = TaggingState.ENABLED;
        private readonly object lck = new object();
        private bool isRead;

        public TaggingState Value
        {
            get
            {
                lock (lck)
                {
                    isRead = true;
                    return Internal;
                }
            }
        }

        public TaggingState Internal
        {
            get
            {
                lock (lck)
                {
                    return currentState;
                }
            }
        }

        // Sets current state to the given state.
        internal void Set(TaggingState state)
        {
            lock (lck)
            {
                if (isRead)
                {
                    throw new InvalidOperationException("State was already read, cannot set state.");
                }

                currentState = state;
            }
        }
    }
}
