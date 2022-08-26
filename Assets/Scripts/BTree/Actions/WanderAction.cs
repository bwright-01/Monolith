using TheKiwiCoder;

public class WanderAction : ActionNode {

    protected override void OnStart() {
        context.wander.enabled = true;
    }

    protected override void OnStop() {
        context.wander.enabled = false;
    }

    protected override State OnUpdate() {
        return State.Running;
    }
}
