using System;

namespace NEW_MOVIES.Services
{
    public class OnBackgroundEnteringEventArgs : EventArgs
    {
        public SuspensionState SuspensionState {
            get; set;
        }

        public Type Target {
            get; set;
        }

        public OnBackgroundEnteringEventArgs(SuspensionState suspensionState, Type target)
        {
            SuspensionState = suspensionState;
            Target = target;
        }
    }
}
