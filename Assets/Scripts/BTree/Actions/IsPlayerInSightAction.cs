
using TheKiwiCoder;

public class IsPlayerInSightAction : ActionNode {
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (context.enemySight.CanSeePlayer()) {
            return State.Success;
        }

        return State.Failure;
    }
}
