using Robust.Shared.Timing;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Content.Shared.Inventory;
using Content.Shared.Mobs;
using Content.Shared.Tag;
using Content.Shared.Mobs.Systems;
using Content.Shared.ActionBlocker;
using Content.Shared.Emoting;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Speech;
using Content.Shared.Throwing;
using Content.Shared.Damage;
using Content.Shared.Damage.Systems;
using Content.Shared.Popups;
using Content.Shared.Alert;
using Content.Shared.Imperial.Intent.Events;
using Content.Shared.Imperial.Intent.Components;

namespace Content.Shared.Imperial.Intent;

public partial class SharedForceGrabedSystem : EntitySystem
{
    [Dependency] private readonly ActionBlockerSystem _blocker = default!;
    [Dependency] private readonly PullingSystem _pulling = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly AlertsSystem _alert = default!;
    [Dependency] private readonly SharedStaminaSystem _stamina = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly InventorySystem _inventorySystem = default!;
    [Dependency] private readonly TagSystem _tag = default!;
    public static readonly ProtoId<AlertPrototype> ReinforcedAlert = "IntentReinforced";
    public static readonly ProtoId<AlertPrototype> KillAlert = "IntentKill";
    public static readonly ProtoId<TagPrototype> HardsuitTag = "Hardsuit";
    public override void Initialize()
    {
        SubscribeLocalEvent<ForceGrabedComponent, UseAttemptEvent>(OnAttempt);
        SubscribeLocalEvent<ForceGrabedComponent, PickupAttemptEvent>(OnAttempt);
        SubscribeLocalEvent<ForceGrabedComponent, ThrowAttemptEvent>(OnAttempt);
        SubscribeLocalEvent<ForceGrabedComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<ForceGrabedComponent, ComponentShutdown>(UpdateCanMove);
        SubscribeLocalEvent<ForceGrabedComponent, UpdateCanMoveEvent>(OnUpdateCanMove);
        SubscribeLocalEvent<ForceGrabedComponent, AttackAttemptEvent>(OnAttempt);
        SubscribeLocalEvent<ForceGrabedComponent, ChangeDirectionAttemptEvent>(OnAttempt);
        SubscribeLocalEvent<ForceGrabedComponent, SpeakAttemptEvent>(OnSpeakAttempt);
        SubscribeLocalEvent<ForceGrabedComponent, PullStoppedMessage>(AfterPullStop);
        SubscribeLocalEvent<ForceGrabedComponent, RedIntentedEvent>(OnRedIntented);
        SubscribeLocalEvent<ForceGrabedComponent, ForceGrabedAlertEvent>(OnGetFreeTry);
    }
    private void OnGetFreeTry(EntityUid uid, ForceGrabedComponent grabed, ForceGrabedAlertEvent args)
    {
        var graber = grabed.GraberUid;

        if (_random.Prob(grabed.PushChance)) // Шанс 8% нанести урон по стамине
        {
            _stamina.TakeStaminaDamage(graber, grabed.StaminaDamage);
            _popup.PopupEntity(Loc.GetString("intent-push-graber", ("graber", EntityManager.GetComponent<MetaDataComponent>(graber).EntityName)), uid, PopupType.SmallCaution);
        }
    }
    private void OnRedIntented(EntityUid uid, ForceGrabedComponent grabed, ref RedIntentedEvent args)
    {
        var graber = args.Uid;

        if (_inventorySystem.TryGetSlotEntity(uid, "outerClothing", out var clothingTarget) &&
            _tag.HasTag((EntityUid) clothingTarget, HardsuitTag))
        {
            _popup.PopupEntity(Loc.GetString("intent-cant-strangle-with-hardsuit"), graber, PopupType.SmallCaution);
            return;
        }

        _alert.ClearAlert(uid, ReinforcedAlert);
        _alert.ShowAlert(uid, KillAlert);
        ReCheckCooldown(uid, grabed);
        grabed.Muted = true; // Трудно говорить, когда тебя душат
        grabed.Strangling = true;
        _popup.PopupEntity(Loc.GetString("intent-start-strangling-target", ("graber", EntityManager.GetComponent<MetaDataComponent>(graber).EntityName), ("target", EntityManager.GetComponent<MetaDataComponent>(uid).EntityName)), graber, PopupType.LargeCaution);
    }
    private void AfterPullStop(EntityUid uid, ForceGrabedComponent grabed, PullStoppedMessage args) => EndGrab(uid, grabed);
    private void EndGrab(EntityUid uid, ForceGrabedComponent grabed)
    {
        _alert.ClearAlert(uid, ReinforcedAlert);
        _alert.ClearAlert(uid, KillAlert);
        grabed.Muted = false; // Если был замьючен
        grabed.Strangling = false;
        var intentSystem = EntityManager.System<SharedIntentSystem>();
        intentSystem.AllowUseIntents(uid);
        RemComp<ForceGrabedComponent>(uid);
    }
    private void OnSpeakAttempt(EntityUid uid, ForceGrabedComponent grabed, SpeakAttemptEvent args)
    {
        if (!grabed.Muted)
            return;

        args.Cancel();
    }
    private void OnAttempt(EntityUid uid, ForceGrabedComponent grabed, CancellableEntityEventArgs args)
    {
        args.Cancel();
    }
    private void OnStartup(EntityUid uid, ForceGrabedComponent grabed, ComponentStartup args)
    {
        _alert.ShowAlert(uid, ReinforcedAlert);

        var intentSystem = EntityManager.System<SharedIntentSystem>();
        intentSystem.DisallowUseIntents(uid);

        UpdateCanMove(uid, grabed, args);
    }
    private void OnUpdateCanMove(EntityUid uid, ForceGrabedComponent grabed, UpdateCanMoveEvent args)
    {
        if (grabed.LifeStage > ComponentLifeStage.Running)
            return;

        args.Cancel();
    }
    private void UpdateCanMove(EntityUid uid, ForceGrabedComponent grabed, EntityEventArgs args)
    {
        _blocker.UpdateCanMove(uid);
    }
    private void ProvideStrangling(EntityUid uid, ForceGrabedComponent grabed) => _damageable.TryChangeDamage(uid, grabed.SuffocatingDamage);
    private void ReCheckCooldown(EntityUid uid, ForceGrabedComponent grabed) => grabed.StranglingCooldown = _timing.CurTime + grabed.Cooldown;
    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<ForceGrabedComponent>();

        while (query.MoveNext(out var uid, out var grab))
        {
            if (_mobState.IsDead(uid))
            {
                _popup.PopupEntity(Loc.GetString("intent-cant-strangle-dead"), uid, PopupType.SmallCaution);
                EndGrab(uid, grab);
                return;
            }

            if(grab.Strangling && _timing.CurTime > grab.StranglingCooldown)
            {
                ProvideStrangling(uid, grab);
                ReCheckCooldown(uid, grab);
            }
        }
    }
    public void SetGraberEntityUid(EntityUid uid, EntityUid graber, ForceGrabedComponent? grabed = null)
    {
        if (!Resolve(uid, ref grabed))
            return;

        grabed.GraberUid = graber;
    }
}
