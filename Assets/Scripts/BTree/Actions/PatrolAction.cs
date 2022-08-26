using UnityEngine;
using TheKiwiCoder;

using Core;

public class PatrolAction : ActionNode {

    Vector2 heading;
    Timer patrolling = new Timer();
    Timer waiting = new Timer();

    protected override void OnStart() {
        heading = context.enemyPatrol.InitialHeading * -1;
        StartPatrol();
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        patrolling.Tick();
        waiting.Tick();

        return State.Running;
    }

    void StartPatrol() {
        heading = heading * -1;
        context.movement.SetHeading(heading);
        patrolling.SetDuration(context.enemyPatrol.TimePatrol);
        patrolling.SetOnEnd((float t) => {
            StartWait();
        });
        patrolling.Start();
    }

    void StartWait() {
        context.movement.Halt();
        waiting.SetDuration(context.enemyPatrol.TimeWait);
        waiting.SetOnEnd((float t) => {
            StartPatrol();
        });
        waiting.Start();
    }
}
