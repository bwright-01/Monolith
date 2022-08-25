using UnityEngine;
using TheKiwiCoder;

using Core;

public class Patrol : ActionNode {

    [SerializeField] float timePatrol = 5f;
    [SerializeField] float timeWait = 1f;
    [SerializeField] Vector2 initialHeading = Vector2.right;

    Vector2 heading;
    Timer patrolling = new Timer();
    Timer waiting = new Timer();

    protected override void OnStart() {
        heading = initialHeading * -1;
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
        patrolling.SetDuration(timePatrol);
        patrolling.SetOnEnd((float t) => {
            StartWait();
        });
        patrolling.Start();
    }

    void StartWait() {
        context.movement.ClearTarget();
        waiting.SetDuration(timeWait);
        waiting.SetOnEnd((float t) => {
            StartPatrol();
        });
        waiting.Start();
    }
}
