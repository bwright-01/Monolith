
using UnityEngine;

namespace Core {
    // delegate types
    public delegate void VoidEvent();
    public delegate void IntEvent(int value);
    public delegate void FloatEvent(float value);
    public delegate void TwoFloatEvent(float value1, float value2);
    public delegate void BoolEvent(bool value);
    public delegate void StringEvent(string value);
    public delegate void GuidEvent(System.Guid value);
    public delegate void ActorEvent(Actor.iActor actor);
    public delegate void PlayerEvent(Player.PlayerMain player);
    public delegate void EnemyEvent(Enemy.EnemyMain enemy);
    public delegate void MonolithEvent(Environment.MonolithType monolithType);
    public delegate void UpgradeEvent(Game.UpgradeType upgradeType);
    public delegate void HealthEvent(float damage, float hp);
    public delegate void HazardEvent(Environment.HazardType hazardType);
    public delegate void FreezeTimeEvent(float duration = 0.1f, float timeScale = 0f);
    public delegate void ShakeGamepadEvent(float duration = 0.1f, float intensity = 0f);

    public class VoidEventHandler {
        event VoidEvent ev;
        public void Subscribe(VoidEvent action) { ev += action; }
        public void Unsubscribe(VoidEvent action) { ev -= action; }
        public void Invoke() { if (ev != null) ev.Invoke(); }
    }

    public class IntEventHandler {
        event IntEvent ev;
        public void Subscribe(IntEvent action) { ev += action; }
        public void Unsubscribe(IntEvent action) { ev -= action; }
        public void Invoke(int value) { if (ev != null) ev.Invoke(value); }
    }

    public class FloatEventHandler {
        event FloatEvent ev;
        public void Subscribe(FloatEvent action) { ev += action; }
        public void Unsubscribe(FloatEvent action) { ev -= action; }
        public void Invoke(float value) { if (ev != null) ev.Invoke(value); }
    }

    public class BoolEventHandler {
        event BoolEvent ev;
        public void Subscribe(BoolEvent action) { ev += action; }
        public void Unsubscribe(BoolEvent action) { ev -= action; }
        public void Invoke(bool value) { if (ev != null) ev.Invoke(value); }
    }

    public class StringEventHandler {
        event StringEvent ev;
        public void Subscribe(StringEvent action) { ev += action; }
        public void Unsubscribe(StringEvent action) { ev -= action; }
        public void Invoke(string value) { if (ev != null) ev.Invoke(value); }
    }

    public class GuidEventHandler {
        event GuidEvent ev;
        public void Subscribe(GuidEvent action) { ev += action; }
        public void Unsubscribe(GuidEvent action) { ev -= action; }
        public void Invoke(System.Guid value) { if (ev != null) ev.Invoke(value); }
    }

    public class ActorEventHandler {
        event ActorEvent ev;
        public void Subscribe(ActorEvent action) { ev += action; }
        public void Unsubscribe(ActorEvent action) { ev -= action; }
        public void Invoke(Actor.iActor value) { if (ev != null) ev.Invoke(value); }
    }

    public class PlayerEventHandler {
        event PlayerEvent ev;
        public void Subscribe(PlayerEvent action) { ev += action; }
        public void Unsubscribe(PlayerEvent action) { ev -= action; }
        public void Invoke(Player.PlayerMain value) { if (ev != null) ev.Invoke(value); }
    }

    public class EnemyEventHandler {
        event EnemyEvent ev;
        public void Subscribe(EnemyEvent action) { ev += action; }
        public void Unsubscribe(EnemyEvent action) { ev -= action; }
        public void Invoke(Enemy.EnemyMain value) { if (ev != null) ev.Invoke(value); }
    }

    public class MonolithEventHandler {
        event MonolithEvent ev;
        public void Subscribe(MonolithEvent action) { ev += action; }
        public void Unsubscribe(MonolithEvent action) { ev -= action; }
        public void Invoke(Environment.MonolithType value) { if (ev != null) ev.Invoke(value); }
    }

    public class UpgradeEventHandler {
        event UpgradeEvent ev;
        public void Subscribe(UpgradeEvent action) { ev += action; }
        public void Unsubscribe(UpgradeEvent action) { ev -= action; }
        public void Invoke(Game.UpgradeType value) { if (ev != null) ev.Invoke(value); }
    }

    public class HealthEventHandler {
        event HealthEvent ev;
        public void Subscribe(HealthEvent action) { ev += action; }
        public void Unsubscribe(HealthEvent action) { ev -= action; }
        public void Invoke(float damage, float hp) { if (ev != null) ev.Invoke(damage, hp); }
    }

    public class HazardEventHandler {
        event HazardEvent ev;
        public void Subscribe(HazardEvent action) { ev += action; }
        public void Unsubscribe(HazardEvent action) { ev -= action; }
        public void Invoke(Environment.HazardType hazardType) { if (ev != null) ev.Invoke(hazardType); }
    }

    public class FreezeTimeEventHandler {
        event FreezeTimeEvent ev;
        public void Subscribe(FreezeTimeEvent action) { ev += action; }
        public void Unsubscribe(FreezeTimeEvent action) { ev -= action; }
        public void Invoke(float duration, float timescale) { if (ev != null) ev.Invoke(duration, timescale); }
    }

    public class ShakeGamepadEventHandler {
        event ShakeGamepadEvent ev;
        public void Subscribe(ShakeGamepadEvent action) { ev += action; }
        public void Unsubscribe(ShakeGamepadEvent action) { ev -= action; }
        public void Invoke(float duration, float intensity) { if (ev != null) ev.Invoke(duration, intensity); }
    }

    [CreateAssetMenu(fileName = "EventChannel", menuName = "ScriptableObjects/EventChannel")]
    public class EventChannelSO : ScriptableObject {
        public GuidEventHandler OnRegionActivate = new GuidEventHandler();
        public GuidEventHandler OnRegionDeactivate = new GuidEventHandler();
        public StringEventHandler OnPlayMusic = new StringEventHandler();
        public VoidEventHandler OnStopMusic = new VoidEventHandler();

        public VoidEventHandler OnRespawnPlayer = new VoidEventHandler();
        public PlayerEventHandler OnPlayerSpawned = new PlayerEventHandler();
        public VoidEventHandler OnPlayerDeath = new VoidEventHandler();

        public EnemyEventHandler OnEnemyDeath = new EnemyEventHandler();
        public MonolithEventHandler OnMonolithDeath = new MonolithEventHandler();
        public VoidEventHandler OnAllMonolithsDestroyed = new VoidEventHandler();
        public UpgradeEventHandler OnApplyUpgrade = new UpgradeEventHandler();
        public UpgradeEventHandler OnAbilityUpgraded = new UpgradeEventHandler();

        public VoidEventHandler OnUseKeyPress = new VoidEventHandler();
        public VoidEventHandler OnUseKeyRelease = new VoidEventHandler();
        public VoidEventHandler OnGotoMainMenu = new VoidEventHandler();
        public VoidEventHandler OnPause = new VoidEventHandler();
        public VoidEventHandler OnUnpause = new VoidEventHandler();

        public FloatEventHandler OnGainHealth = new FloatEventHandler();

        public HazardEventHandler OnHazardEnter = new HazardEventHandler();
        public HazardEventHandler OnHazardExit = new HazardEventHandler();

        public FreezeTimeEventHandler OnFreezeTime = new FreezeTimeEventHandler();
        public ShakeGamepadEventHandler OnShakeGamepad = new ShakeGamepadEventHandler();
    }
}

