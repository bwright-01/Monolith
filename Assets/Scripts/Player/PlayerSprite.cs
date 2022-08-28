using UnityEngine;

namespace Player {

    public class PlayerSprite : MonoBehaviour {
        PlayerMain player;
        PlayerMovement movement;
        PlayerShooter shooter;

        Animator anim;

        const string ANIM_PLAYER_DEATH = "Death";
        const string ANIM_PLAYER_IDLE_UP = "IdleUp";
        const string ANIM_PLAYER_IDLE_DOWN = "IdleDown";
        const string ANIM_PLAYER_IDLE_LEFT = "IdleLeft";
        const string ANIM_PLAYER_IDLE_RIGHT = "IdleRight";
        const string ANIM_PLAYER_MOVE_UP = "MoveUp";
        const string ANIM_PLAYER_MOVE_DOWN = "MoveDown";
        const string ANIM_PLAYER_MOVE_LEFT = "MoveLeft";
        const string ANIM_PLAYER_MOVE_RIGHT = "MoveRight";
        const string ANIM_PLAYER_ATTACK_UP = "AttackUp";
        const string ANIM_PLAYER_ATTACK_DOWN = "AttackDown";
        const string ANIM_PLAYER_ATTACK_LEFT = "AttackLeft";
        const string ANIM_PLAYER_ATTACK_RIGHT = "AttackRight";

        string currentAnimState;
        string nextAnimState;

        void Awake() {
            player = GetComponentInParent<PlayerMain>();
            movement = GetComponentInParent<PlayerMovement>();
            shooter = GetComponentInParent<PlayerShooter>();
            anim = GetComponentInParent<Animator>();
        }

        private void Update() {
            Animate();
        }

        void Animate() {
            nextAnimState = GetNextAnimationState();
            if (currentAnimState == nextAnimState) return;
            currentAnimState = nextAnimState;
            anim.Play(currentAnimState);
        }

        string GetNextAnimationState() {
            if (player == null || !player.IsAlive()) return ANIM_PLAYER_DEATH;

            if (shooter.IsMeleeing) {
                if (IsFacingUp()) return ANIM_PLAYER_ATTACK_UP;
                if (IsFacingDown()) return ANIM_PLAYER_ATTACK_DOWN;
                if (IsFacingLeft()) return ANIM_PLAYER_ATTACK_LEFT;
                if (IsFacingRight()) return ANIM_PLAYER_ATTACK_RIGHT;
            }

            if (shooter.IsShooting || movement.HasMoveInput()) {
                if (IsFacingUp()) return ANIM_PLAYER_MOVE_UP;
                if (IsFacingDown()) return ANIM_PLAYER_MOVE_DOWN;
                if (IsFacingLeft()) return ANIM_PLAYER_MOVE_LEFT;
                if (IsFacingRight()) return ANIM_PLAYER_MOVE_RIGHT;
            }

            if (IsFacingUp()) return ANIM_PLAYER_IDLE_UP;
            if (IsFacingDown()) return ANIM_PLAYER_IDLE_DOWN;
            if (IsFacingLeft()) return ANIM_PLAYER_IDLE_LEFT;
            if (IsFacingRight()) return ANIM_PLAYER_IDLE_RIGHT;

            return ANIM_PLAYER_IDLE_DOWN;
        }

        bool IsFacingDown() {
            return transform.parent.rotation.eulerAngles.z == 0f;
        }
        bool IsFacingRight() {
            return transform.parent.rotation.eulerAngles.z == 90f;
        }
        bool IsFacingUp() {
            return transform.parent.rotation.eulerAngles.z == 180f;
        }
        bool IsFacingLeft() {
            return transform.parent.rotation.eulerAngles.z == 270f;
        }
    }
}
