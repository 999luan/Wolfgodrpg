using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Wolfgodrpg.Common.Systems
{
    public class ConfigSystem : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        // === HUNGER SETTINGS ===
        [LabelKey("$Mods.Wolfgodrpg.Config.EnableHunger.Label")]
        [TooltipKey("$Mods.Wolfgodrpg.Config.EnableHunger.Tooltip")]
        [DefaultValue(true)]
        public bool EnableHunger;

        [LabelKey("$Mods.Wolfgodrpg.Config.HungerRate.Label")]
        [TooltipKey("$Mods.Wolfgodrpg.Config.HungerRate.Tooltip")]
        [DefaultValue(1.0f)]
        [Range(0.1f, 5.0f)]
        public float HungerRate;

        // === SANITY SETTINGS ===
        [LabelKey("$Mods.Wolfgodrpg.Config.EnableSanity.Label")]
        [TooltipKey("$Mods.Wolfgodrpg.Config.EnableSanity.Tooltip")]
        [DefaultValue(true)]
        public bool EnableSanity;

        [LabelKey("$Mods.Wolfgodrpg.Config.SanityRate.Label")]
        [TooltipKey("$Mods.Wolfgodrpg.Config.SanityRate.Tooltip")]
        [DefaultValue(1.0f)]
        [Range(0.1f, 5.0f)]
        public float SanityRate;

        // === ITEM SETTINGS ===
        [LabelKey("$Mods.Wolfgodrpg.Config.RandomStats.Label")]
        [TooltipKey("$Mods.Wolfgodrpg.Config.RandomStats.Tooltip")]
        [DefaultValue(true)]
        public bool RandomStats;

        [LabelKey("$Mods.Wolfgodrpg.Config.ItemStatMultiplier.Label")]
        [TooltipKey("$Mods.Wolfgodrpg.Config.ItemStatMultiplier.Tooltip")]
        [DefaultValue(1.0f)]
        [Range(0.1f, 5.0f)]
        public float ItemStatMultiplier;

        // === INTERFACE SETTINGS ===
        [LabelKey("$Mods.Wolfgodrpg.Config.UIScale.Label")]
        [TooltipKey("$Mods.Wolfgodrpg.Config.UIScale.Tooltip")]
        [DefaultValue(1.0f)]
        [Range(0.5f, 2.0f)]
        public float UIScale;

        [LabelKey("$Mods.Wolfgodrpg.Config.ShowNotifications.Label")]
        [TooltipKey("$Mods.Wolfgodrpg.Config.ShowNotifications.Tooltip")]
        [DefaultValue(true)]
        public bool ShowNotifications;

        // === DIFFICULTY SETTINGS ===
        [LabelKey("$Mods.Wolfgodrpg.Config.MonsterHealthMultiplier.Label")]
        [TooltipKey("$Mods.Wolfgodrpg.Config.MonsterHealthMultiplier.Tooltip")]
        [DefaultValue(1.0f)]
        [Range(0.5f, 3.0f)]
        public float MonsterHealthMultiplier;

        [LabelKey("$Mods.Wolfgodrpg.Config.MonsterDamageMultiplier.Label")]
        [TooltipKey("$Mods.Wolfgodrpg.Config.MonsterDamageMultiplier.Tooltip")]
        [DefaultValue(1.0f)]
        [Range(0.5f, 3.0f)]
        public float MonsterDamageMultiplier;

        // === LEVEL SETTINGS ===
        [LabelKey("$Mods.Wolfgodrpg.Config.MaxLevel.Label")]
        [TooltipKey("$Mods.Wolfgodrpg.Config.MaxLevel.Tooltip")]
        [DefaultValue(100)]
        [Range(10, 999)]
        public int MaxLevel;

        [LabelKey("$Mods.Wolfgodrpg.Config.StartingLevel.Label")]
        [TooltipKey("$Mods.Wolfgodrpg.Config.StartingLevel.Tooltip")]
        [DefaultValue(1)]
        [Range(1, 10)]
        public int StartingLevel;
    }

    public enum UIPosition
    {
        TopLeft,
        TopRight,
        Center,
        BottomLeft,
        BottomRight
    }
}