using UnityEngine;
using UnityEngine.AI;

namespace TheKiwiCoder {

    // The context is a shared object every node has access to.
    // Commonly used components and subsytems should be stored here
    // It will be somewhat specfic to your game exactly what to add here.
    // Feel free to extend this class 
    public class Context {
        // UNUSED:
        // public Animator animator;
        // public Rigidbody2D rb;
        // public NavMeshAgent agent;

        public GameObject gameObject;
        public Transform transform;
        public Movement.ActorMovement movement;
        public Enemy.EnemyPatrol enemyPatrol;
        public Enemy.EnemyAttack enemyAttack;
        public Enemy.EnemySight enemySight;
        public Player.PlayerMain player;

        public static Context CreateFromGameObject(GameObject gameObject) {
            // UNUSED:
            // context.animator = gameObject.GetComponent<Animator>();
            // context.rb = gameObject.GetComponent<Rigidbody2D>();
            // context.agent = gameObject.GetComponent<NavMeshAgent>();

            Context context = new Context();
            context.gameObject = gameObject;
            context.transform = gameObject.transform;
            context.movement = gameObject.GetComponent<Movement.ActorMovement>();
            context.enemyPatrol = gameObject.GetComponent<Enemy.EnemyPatrol>();
            context.enemyAttack = gameObject.GetComponent<Enemy.EnemyAttack>();
            context.enemySight = gameObject.GetComponent<Enemy.EnemySight>();

            return context;
        }
    }
}
