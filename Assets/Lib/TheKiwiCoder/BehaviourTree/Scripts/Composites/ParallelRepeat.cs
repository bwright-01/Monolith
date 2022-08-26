
using System.Collections.Generic;
using System.Linq;

// This composite is exactly the same as Parallel, except that it will
// return the first completed action state, whether SUCCESS or FAILURE.

namespace TheKiwiCoder {
    public class ParallelRepeat : CompositeNode {
        State currentStatus;
        bool stillRunning;

        protected override void OnStart() {
        }

        protected override void OnStop() {
        }

        protected override State OnUpdate() {
            stillRunning = false;
            for (int i = 0; i < children.Count(); ++i) {
                currentStatus = children[i].Update();

                if (currentStatus == State.Failure) {
                    AbortRunningChildren();
                    return State.Failure;
                }

                if (currentStatus == State.Running) {
                    stillRunning = true;
                }

            }

            return stillRunning ? State.Running : State.Success;
        }

        void AbortRunningChildren() {
            for (int i = 0; i < children.Count(); ++i) {
                if (children[i].state == State.Running) {
                    children[i].Abort();
                }
            }
        }
    }
}
