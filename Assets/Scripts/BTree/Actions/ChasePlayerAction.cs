
using TheKiwiCoder;

public class ChasePlayerAction : ActionNode {
    protected override void OnStart() {
        context.player = Player.PlayerUtils.FindPlayer();
        if (Player.PlayerUtils.CheckIsAlive(context.player)) {
            context.movement.SetTarget(context.player.transform);
        }
    }

    protected override void OnStop() {
        context.movement.Halt();
    }

    protected override State OnUpdate() {
        if (!context.enemySight.CanSeePlayer()) {
            return State.Failure;
        }

        return State.Running;
    }
}
