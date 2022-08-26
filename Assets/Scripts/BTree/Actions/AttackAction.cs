
using UnityEngine;
using TheKiwiCoder;

public class AttackAction : ActionNode {
    protected override void OnStart() {
        context.enemyAttack.enabled = true;
    }

    protected override void OnStop() {
        context.enemyAttack.enabled = false;
    }

    protected override State OnUpdate() {
        if (!context.enemyAttack.enabled) {
            return State.Failure;
        }

        if (!context.enemyAttack.IsPlayerInAttackRange()) {
            return State.Failure;
        }

        return State.Running;
    }
}
