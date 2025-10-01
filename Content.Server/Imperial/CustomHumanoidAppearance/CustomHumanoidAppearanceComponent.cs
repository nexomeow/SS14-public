using Robust.Shared.Prototypes;
using Robust.Shared.Enums;
using Content.Shared.IdentityManagement.Components;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Humanoid.Prototypes;

/// <summary>
/// Выдает сущности определенный прототип персонажа.
/// </summary>
namespace Content.Server.Imperial.CustomHumanoidAppearance.Components
{
    [RegisterComponent]
    [Access(typeof(CustomHumanoidAppearanceSystem))]
    public sealed partial class CustomHumanoidAppearanceComponent : Component
    {
        #region base fields
        //Базовые значения.

        /// <summary>
        /// Имя персонажа.
        /// </summary>
        [DataField("name")]
        public string CharacterName = "John Sanabi"; //220 санаби кактус кемерово 42

        /// <summary>
        /// Возраст.
        /// </summary>
        [DataField("age")]
        public int CharacterAge = 18; // Даже бл не думайте сделать ниже.

        /// <summary>
        /// Цвет кожи.
        /// </summary>
        [DataField("skinColor")]
        public Color CharacterSkinColor { get; set; } = Color.FromHex("#C0967F");

        /// <summary>
        /// Цвет глаз.
        /// </summary>
        [DataField("eyeColor")]
        public Color CharacterEyeColor = Color.Black;
        #endregion

        #region sex and gender
        /// <summary>
        /// Определяет, будет ли пол гуманоида женским или мужским (по умолчанию неопределенный).
        /// </summary>
        [DataField("sex")]
        public Sex CharacterSex = Sex.Unsexed;

        /// <summary>
        /// Определяет, будет ли пол гуманоида женским или мужским (по умолчанию неопределенный).
        /// </summary>
        [DataField("gender")]
        public Gender CharacterGender = Gender.Epicene;
        #endregion

        //    [DataField]
        // public ProtoId<HumanoidProfilePrototype>? Initial { get; private set; }
        // На будущее
        #region markings
        // Маркинги (волосы, хвосты, ушки etc)
        // Волосы
        [DataField("hair")]
        // public List<MarkingPrototype> CharacterHair = new List<MarkingPrototype>();
        // public string CharacterHair = HumanHairLongBedheadOld;
        public ProtoId<MarkingPrototype> CharacterHair = "HumanHairLongBedheadOld";

        [DataField("hairColor")]
        public List<Color> CharacterHairColor = new List<Color>();
        #endregion
    }
}
