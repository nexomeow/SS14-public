using Robust.Shared.GameObjects;
using Robust.Shared.Timing;
using Robust.Shared.Input.Binding;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Content.Shared.Imperial.ICCVar;
using Content.Shared.Audio;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Standing;
using Content.Shared.Damage;
using Content.Shared.Damage.Systems;
using Content.Shared.Alert;
using Content.Shared.Input;
using Content.Shared.Popups;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.DoAfter;
using Content.Shared.CombatMode.Pacification;
using Content.Shared.Popups;
using Content.Shared.Imperial.Intent.Events;
using Content.Shared.Imperial.Intent.Enums;
using Content.Shared.Imperial.Intent.Components;

namespace Content.Shared.Imperial.Intent;

public abstract partial class SharedIntentSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly AlertsSystem _alert = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedStaminaSystem _stamina = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly PullingSystem _pulling = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedInteractionSystem _interact = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;

    public static readonly ProtoId<AlertCategoryPrototype> IntentCategory = "Intent";
    public static readonly ProtoId<AlertPrototype> IntentAlert = "IntentAlert";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<IntentComponent, ComponentStartup>(IntentStart);
        SubscribeLocalEvent<IntentComponent, IntentDoAfterEvent>(OnDoAfter);
        SubscribeLocalEvent<IntentComponent, UserActivateInWorldEvent>(OnAfterInteract); //InteractionAttemptEvent

        CommandBinds.Builder
            .Bind(ContentKeyFunctions.ChangeIntent, InputCmdHandler.FromDelegate(ChooseNextIntent, handle: false))
            .Register<SharedIntentSystem>();
    }
    private void IntentStart(EntityUid uid, IntentComponent intent, ComponentStartup args)
    {
        if(_cfg.GetCVar(ICCVars.EnableIntents))
        {
            _alert.ShowAlert(uid, IntentAlert, 0);
        }
        else
        {
            RemComp<IntentComponent>(uid);
            RemComp<IntentableComponent>(uid);
        }
    }
    private void OnAfterInteract(EntityUid uid, IntentComponent intent, ref UserActivateInWorldEvent args)
    {
        if (args.Target == null) // Это уже не совсем актуально, но пусть будет. Взаимодействий в воздухом нам не надо
            return;

        if (intent.State == IntentState.White) // Если стоит стейт White, любые действия дальше прекращаются.
            return;

        var target = args.Target;

        if (!HasComp<IntentableComponent>(target)) // Сущность должна иметь компонент Intentable, для использования на ней интентов
            return;

        if (_timing.CurTime < intent.FinalTime) // Задержка в 1.5 секунды во избежании спама
        {
            _popup.PopupEntity(Loc.GetString("intent-not-so-fast"), args.User, PopupType.Small);
            return;
        }
        GetCooldownTime(uid, intent); // Обновление задержки

        if (uid == target) // С собой взаимодействовать нельзя
        {
            _popup.PopupClient(Loc.GetString("intent-cant-interact-with-myself"), args.User, PopupType.Small);
            return;
        }

        switch (intent.State) // Попап зависит от выбранного интента
        {
            case IntentState.White:
                _popup.PopupEntity(Loc.GetString("intent-start-interact-with", ("target", EntityManager.GetComponent<MetaDataComponent>(target).EntityName)), args.User, PopupType.SmallCaution);
                break;
            case IntentState.Green:
                _popup.PopupEntity(Loc.GetString("intent-start-interact-green", ("target", EntityManager.GetComponent<MetaDataComponent>(target).EntityName)), args.User, PopupType.Small);
                break;
            case IntentState.Blue:
                _popup.PopupEntity(Loc.GetString("intent-start-interact-blue", ("target", EntityManager.GetComponent<MetaDataComponent>(target).EntityName)), args.User, PopupType.SmallCaution);
                break;
            case IntentState.Yellow:
                _popup.PopupEntity(Loc.GetString("intent-start-interact-yellow", ("target", EntityManager.GetComponent<MetaDataComponent>(target).EntityName)), args.User, PopupType.SmallCaution);
                break;
            case IntentState.Red:
                _popup.PopupEntity(Loc.GetString("intent-start-interact-red", ("target", EntityManager.GetComponent<MetaDataComponent>(target).EntityName)), args.User, PopupType.MediumCaution);
                break;
        }
        intent.InteractionsCount = intent.InteractionsCount + 1; // Подсчет взаимодействий за раунд

        StartInteract(uid, target, intent);
    }
    private void StartInteract(EntityUid uid, EntityUid user, IntentComponent intent)
    {
        if (!intent.CanUse)
            return;

        switch (intent.State)
        {
            case IntentState.White:
                // Буквально ничего
                // Белый интент подразумевает взаимодействия таким образом, будто бы у куклы вообще нет этой системы
                break;

            case IntentState.Green: // Интент направлен на помощь
                intent.FinalLength = intent.GreenLength / intent.TimeCoefficient;
                ConfirmDoAfter(uid, user, intent, false, true, true, intent.FinalLength);
                break;

            case IntentState.Blue: // Интент направлен на обезоруживание и попытку свалить с ног
                intent.FinalLength = intent.BlueLength / intent.TimeCoefficient;
                ConfirmDoAfter(uid, user, intent, false, false, true, intent.FinalLength);
                break;

            case IntentState.Yellow: // Интент направлен на заламывание противника лицом в пол, либо к ограничению движения будто наручниками
                intent.FinalLength = intent.YellowLength / intent.TimeCoefficient;
                ConfirmDoAfter(uid, user, intent, true, true, true, intent.FinalLength);
                break;

            case IntentState.Red: // Этот интент направлен на убийство
                intent.FinalLength = intent.RedLength / intent.TimeCoefficient;
                ConfirmDoAfter(uid, user, intent, true, true, true, intent.FinalLength);
                break;
        }
    }
    private void ConfirmDoAfter(EntityUid uid, EntityUid user, IntentComponent intent, bool breakOnDamage, bool breakOnMove, bool needHand, float doAfterLength)
    {
        var doAfter = new DoAfterArgs(EntityManager, uid, TimeSpan.FromSeconds(doAfterLength), new IntentDoAfterEvent(), eventTarget: uid)
        {
            BreakOnDamage = breakOnDamage,
            BreakOnMove = breakOnMove,
            NeedHand = needHand,
            RequireCanInteract = true,
            DistanceThreshold = 1f,
        };

        if (!_doAfter.TryStartDoAfter(doAfter))
            return;

        _popup.PopupEntity(Loc.GetString("intent-get-interact-with", ("uid", EntityManager.GetComponent<MetaDataComponent>(uid).EntityName), ("target", EntityManager.GetComponent<MetaDataComponent>(user).EntityName)),
            user,
            user,
            PopupType.LargeCaution);
        intent.LastTarget = user;
    }
    private void OnDoAfter(EntityUid uid, IntentComponent intent, IntentDoAfterEvent args)
    {
        if (args.Cancelled)
            return;
        if (args.Handled)
            return;

        args.Handled = true;
        switch (intent.State)
        {
            case IntentState.White:
//               ░░░░░░▄█▀█▄░░░░░░░░░░░░░░░
//               ░▄█▀▀▀▀░░░░▀█▄▄▄▄▄▄▄░░░░░░
//               █▀░░░░░░░░░░░░░░░░░▀█░░░░░
//               ▀▄░▄░░░░░░░░░░░░░░░▄█░░░░░
//               ░█████▄▄▄▄▄██▄▄▄█▀▀█░░░░░░
//               ░█▀█░░░░▀░░░░░▀░░░░█▀▀▀▀▀█
//               ░█░███▄▄▄▄░░░▄▄▄▄▄██▀▀██░█
//               ░█░███░████▀████░███░░█░░█             .／＞　　フ
//               ░█▄███░████░████░███░░█░█▀   　　　 　　| 　　   l
//               ░░░███░████░████░███░░█░█░   　 　　 　／` ミ＿xノ
//               ░░░███░████░████░███▄▄█░█░   　　 　 /　　　 　 |
//               ░░░███░████░████░███░░░▄█░   　　　 /　 ヽ　　 ﾉ
//               ░░░███░████░████░███▀▀▀▀░░    　   │　　|　|　|
//               ░░▄███▄████░████▄███▄░░░░░  　 　   │　　|　|　|
//               ░░███▀███████████▀███░░░░░  　 |(￣ヽ＿ヽ)__)__)
//               ░░░░▀▀▀██▄▄▄▄▄██▀▀▀░░░░░░░   　＼二つ
//               stop suffering while shitcoding
//               take a beer
                break;

            case IntentState.Green:
                TryToHelp(uid, intent);
                break;

            case IntentState.Blue:
                TryToDisarm(uid, intent);
                break;

            case IntentState.Yellow:
                TryToGrab(uid, intent);
                break;

            case IntentState.Red:
                TryToHarm(uid, intent);
                break;
        }
    }
    private void ChooseNextIntent(ICommonSession? session)
    {
        if (session is not { } playerSession)
            return;

        if (playerSession.AttachedEntity is not { Valid: true } playerEnt || !Exists(playerEnt))
            return;

        if (TryComp<IntentComponent>(playerEnt, out var intcomp))
            NextIntent(playerEnt, intcomp);
    }
    private void NextIntent(EntityUid target, IntentComponent intent)
    {
        if (!intent.CanChange)
        {
            _popup.PopupEntity(Loc.GetString("intent-cant-change-intent", ("target", EntityManager.GetComponent<MetaDataComponent>(target).EntityName)), target, PopupType.Small);
            return;
        }

        switch (intent.State)
        {
            case IntentState.White:
                intent.State = IntentState.Green;
                break;

            case IntentState.Green:
                intent.State = IntentState.Blue;
                break;

            case IntentState.Blue:
                intent.State = IntentState.Yellow;
                break;

            case IntentState.Yellow:
                intent.State = IntentState.Red;
                break;

            case IntentState.Red:
                intent.State = IntentState.White;
                break;
        }
        UpdateAlert(target, intent);
    }
    private void UpdateAlert(EntityUid target, IntentComponent? intent = null)
    {
        if (!Resolve(target, ref intent))
            return;

        _alert.ShowAlert(target, IntentAlert, (short)intent.State);
    }
    private void GetCooldownTime(EntityUid uid, IntentComponent intent) => intent.FinalTime = _timing.CurTime + intent.CooldownTime;
    #region Intent Actions
    // Логика интентов в данном плане довольно простая
    // Если сущность не в захвате то проивзодятся определенные действия (за исключением синего), присущие только этому интенту
    // Например зеленым интентом вы проводите реанимацию. Если сущность в захвате, производится уже другое действие,
    // которое будет происходит в другой системе и вызываться по локальному ивенту
    // Hope on private repo developers, что приватичайны добавят интересный функционал завязаный на интентах в приватном репозиторее
    private void TryToHelp(EntityUid uid, IntentComponent intent) // Зеленый интент
    {
        var target = intent.LastTarget;

        if (!_interact.InRangeUnobstructed(uid, target))
        {
            _popup.PopupEntity(Loc.GetString("intent-too-far", ("target", EntityManager.GetComponent<MetaDataComponent>(target).EntityName)), uid, PopupType.SmallCaution);
            return;
        }

        if (!HasComp<ForceGrabedComponent>(target))
        {
            // Должно работать только в крите
            if (_mobState.IsAlive(target))
            {
                _popup.PopupEntity(Loc.GetString("intent-cant-help-alive", ("target", EntityManager.GetComponent<MetaDataComponent>(target).EntityName)), uid, PopupType.Small);
                return;
            }
            if (_mobState.IsDead(target))
            {
                _popup.PopupEntity(Loc.GetString("intent-cant-help-dead", ("target", EntityManager.GetComponent<MetaDataComponent>(target).EntityName)), uid, PopupType.Small);
                return;
            }

            _damageable.TryChangeDamage(target, intent.DamageRecovery);
            _audio.PlayPvs(intent.HelpSound, target);
            if (intent.State != IntentState.Green)
            return;
            StartInteract(uid, target, intent); //Повторяем
        }
        else
        {
            var intented = new GreenIntentedEvent(uid, target);
            RaiseLocalEvent(target, ref intented);
        }
    }
    private void TryToDisarm(EntityUid uid, IntentComponent intent) // Синий интент
    {
        if (intent.State != IntentState.Blue)
            return;

        var target = intent.LastTarget;

        if (!_interact.InRangeUnobstructed(uid, target))
        {
            _popup.PopupEntity(Loc.GetString("intent-too-far", ("target", EntityManager.GetComponent<MetaDataComponent>(target).EntityName)), uid, PopupType.SmallCaution);
            return;
        }

        if (_random.Prob(intent.PushChance)) // Шанс 75% нанести урон по стамине
        {
            intent.StaminaDamage = intent.StaminaDamage * intent.EffectivenessCoefficient;
            _stamina.TakeStaminaDamage(target, intent.StaminaDamage);
            _audio.PlayPvs(intent.DisarmSound, target);
        }
        else return;

        if (_random.Prob(intent.DisarmChance)) // Шанс 25% выбить предметы из рук
        {
            var ev = new DropHandItemsEvent();
            RaiseLocalEvent(target, ref ev);
        }

        var intented = new BlueIntentedEvent(uid, target); // Синий не имеет своего взаимодействия. Ивент райсится всегда
        RaiseLocalEvent(target, ref intented);
    }
    private void TryToGrab(EntityUid uid, IntentComponent intent) // Жёлтый интент
    {
        var target = intent.LastTarget;

        if (!_interact.InRangeUnobstructed(uid, target))
        {
            _popup.PopupEntity(Loc.GetString("intent-too-far", ("target", EntityManager.GetComponent<MetaDataComponent>(target).EntityName)), uid, PopupType.SmallCaution);
            return;
        }

        // Зачем захватывать мёртвого?
        if (_mobState.IsDead(target))
        {
            _popup.PopupEntity(Loc.GetString("intent-cant-grab-dead", ("target", EntityManager.GetComponent<MetaDataComponent>(target).EntityName)), uid, PopupType.Small);
            return;
        }

        if (!HasComp<ForceGrabedComponent>(target))
        {
            _audio.PlayPvs(intent.GrabSound, target);
            EnsureComp<ForceGrabedComponent>(target);
            _pulling.ToggleForcedPull(target, uid); // Если отпустить, захват пропадет

            var forceGrabedSystem = EntityManager.System<SharedForceGrabedSystem>();
            forceGrabedSystem.SetGraberEntityUid(target, uid);
        }
        else
        {
            var intented = new YellowIntentedEvent(uid, target);
            RaiseLocalEvent(target, ref intented);
        }
    }
    private void TryToHarm(EntityUid uid, IntentComponent intent) // Красный интент
    {
        var target = intent.LastTarget;

        if (!_interact.InRangeUnobstructed(uid, target))
        {
            _popup.PopupEntity(Loc.GetString("intent-too-far", ("target", EntityManager.GetComponent<MetaDataComponent>(target).EntityName)), uid, PopupType.SmallCaution);
            return;
        }

        // Зачем вредить мёртвому?
        if (_mobState.IsDead(target))
        {
            _popup.PopupEntity(Loc.GetString("intent-cant-grab-dead", ("target", EntityManager.GetComponent<MetaDataComponent>(target).EntityName)), uid, PopupType.Small);
            return;
        }
        if (!HasComp<ForceGrabedComponent>(target))
        {
            // Ничего
        }
        else
        {
            var intented = new RedIntentedEvent(uid, target); // В отличии от остальных красный интент работает пока только в синергии с желтым
            RaiseLocalEvent(target, ref intented);
        }
    }
    #endregion
    #region Public API

    /// <summary>
    /// Следующие два интента запрещают или разрешают использовать и менять интенты
    /// </summary>
    public void DisallowUseIntents(EntityUid target, IntentComponent? intent = null)
    {
        if (!Resolve(target, ref intent))
            return;
        intent.CanUse = false;
        intent.CanChange = false;
    }
    public void AllowUseIntents(EntityUid target, IntentComponent? intent = null)
    {
        if (!Resolve(target, ref intent))
            return;
        intent.CanUse = true;
        intent.CanChange = true;
    }

    /// <summary>
    /// Устанавливает интент для энтити
    /// </summary>
    /// <param name="target"></param>
    /// <param name="state"></param>
    /// <param name="intent"></param>
    public void SetIntent(EntityUid target, IntentState state, IntentComponent? intent = null)
    {
        if (!Resolve(target, ref intent))
            return;

        intent.State = state;
        UpdateAlert(target, intent);
    }

    /// <summary>
    /// Устанавливает белый интент и не дает возможность сменить его
    /// </summary>
    /// <param name="target"></param>
    /// <param name="intent"></param>
    public void PacifiedIntentUser(EntityUid target, IntentComponent? intent = null)
    {
        if (!Resolve(target, ref intent))
            return;

        intent.CanChange = false;
        intent.State = IntentState.White;
        UpdateAlert(target, intent);
    }
    #endregion
}
