
using TheKiwiCoder;

public class ChasePlayerAction : ActionNode {
    protected override void OnStart() {
        context.player = Player.PlayerUtils.FindPlayer();
        if (Player.PlayerUtils.CheckIsAlive(context.player)) {
            context.movement.SetTarget(context.player.transform);
        }
        if (context.wander != null) context.wander.enabled = false;
    }

    protected override void OnStop() {
        context.movement.Halt();
        if (context.wander != null) context.wander.enabled = true;
    }

    protected override State OnUpdate() {
        if (!context.enemySight.CanSeePlayer()) {
            return State.Failure;
        }

        return State.Running;
    }
}
