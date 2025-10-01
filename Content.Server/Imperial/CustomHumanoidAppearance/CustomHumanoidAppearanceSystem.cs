using Robust.Shared.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects.Components.Localization;
using Content.Server.Explosion.EntitySystems;
using Content.Shared.Body.Systems;
using Content.Shared.Body.Components;
using Content.Shared.Preferences;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Humanoid.Prototypes;
using Content.Server.Humanoid;
using Content.Server.Imperial.CustomHumanoidAppearance.Components;

namespace Content.Server.Imperial.CustomHumanoidAppearance;

public sealed class CustomHumanoidAppearanceSystem : EntitySystem
{
    [Dependency] private readonly HumanoidAppearanceSystem _humanoidAppearance = default!;
    [Dependency] private readonly MetaDataSystem _metaData = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly GrammarSystem _grammarSystem = default!;
    [Dependency] private readonly SharedBodySystem _body = default!;
    [Dependency] private readonly ExplosionSystem _explosion = default!;
    [Dependency] private readonly MarkingManager _markingManager = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CustomHumanoidAppearanceComponent, MapInitEvent>(OnMapInit);
    }
    private void SetCharacterAppearance(EntityUid uid, CustomHumanoidAppearanceComponent component, HumanoidAppearanceComponent? humanoid = null)
    {
        if (TryComp<HumanoidAppearanceComponent>(uid, out var comp))
        {
            #region base
            // Имя
            _metaData.SetEntityName(uid, component.CharacterName);
            // Пол и гендер
            comp.Sex = component.CharacterSex;
            comp.Gender = component.CharacterGender;
            // Оторбажение гендера при осмотре
            if (TryComp<GrammarComponent>(uid, out var grammar))
                _grammarSystem.SetGender((uid, grammar), component.CharacterGender);
            // Возраст
            comp.Age = component.CharacterAge;
            // Цвет глаз
            comp.EyeColor = component.CharacterEyeColor;
            // Цвет кожи
            _humanoidAppearance.SetSkinColor(uid, component.CharacterSkinColor, true);
            #endregion

            #region markings (hairs, tails, ears etc)
            // Волосы
            //_humanoidAppearance.SetMarkingId(uid, MarkingCategories.Hair, 0, component.CharacterHair, humanoid: humanoid);
            //_humanoidAppearance.SetMarkingColor(uid, MarkingCategories.Hair, 0, component.CharacterHairColor);
            //markings.AddBack(MarkingCategories.Hair, component.CharacterHair);
            #endregion

        }
        if (HasComp<BodyComponent>(uid))
            if (component.CharacterAge < 18) // Никаких детей на станции.
            {
                _explosion.QueueExplosion(uid, ExplosionSystem.DefaultExplosionPrototypeId, 1, 1, 1);
                _body.GibBody(uid, splatModifier: 5f);
            }
    }
    private void OnMapInit(EntityUid uid, CustomHumanoidAppearanceComponent component, MapInitEvent args)
    {
        SetCharacterAppearance(uid, component);
    }
}
