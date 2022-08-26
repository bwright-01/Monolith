
using TheKiwiCoder;

public class IsPlayerInAttackRangeAction : ActionNode {
    protected override void OnStart() {
    }

    protected override void OnStop() {

    }

    protected override State OnUpdate() {

        if (context.enemyAttack.IsPlayerInAttackRange()) {
            return State.Success;
        }

        return State.Failure;
    }
}

