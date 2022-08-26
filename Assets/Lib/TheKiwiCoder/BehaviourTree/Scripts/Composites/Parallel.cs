
using System.Collections.Generic;
using System.Linq;

namespace TheKiwiCoder {
    public class Parallel : CompositeNode {
        List<State> childrenLeftToExecute = new List<State>();

        State currentStatus;
        bool stillRunning;

        protected override void OnStart() {
            childrenLeftToExecute.Clear();
            children.ForEach(a => {
                childrenLeftToExecute.Add(State.Running);
            });
        }

        protected override void OnStop() {
        }

        protected override State OnUpdate() {
            stillRunning = false;
            for (int i = 0; i < childrenLeftToExecute.Count(); ++i) {
                if (childrenLeftToExecute[i] == State.Running) {
                    currentStatus = children[i].Update();
                    if (currentStatus == State.Failure) {
                        AbortRunningChildren();
                        return State.Failure;
                    }

                    if (currentStatus == State.Running) {
                        stillRunning = true;
                    }

                    childrenLeftToExecute[i] = currentStatus;
                }
            }

            return stillRunning ? State.Running : State.Success;
        }

        void AbortRunningChildren() {
            for (int i = 0; i < childrenLeftToExecute.Count(); ++i) {
                if (childrenLeftToExecute[i] == State.Running) {
                    children[i].Abort();
                }
            }
        }
    }
}
