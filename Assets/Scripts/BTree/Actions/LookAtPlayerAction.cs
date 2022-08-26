
using TheKiwiCoder;

public class LookAtPlayerAction : ActionNode {
    protected override void OnStart() {
    }

    protected override void OnStop() {
        context.enemySight.LookAtPlayer();
    }

    protected override State OnUpdate() {
        if (!context.enemySight.CanSeePlayer()) {
            return State.Failure;
        }

        context.enemySight.LookAtPlayer();

        return State.Running;
    }
}
