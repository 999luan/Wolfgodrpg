using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Wolfgodrpg.Common.Data;

namespace Wolfgodrpg.Common.Classes
{
    public enum ClassAbility
    {
        // Acrobat Abilities
        BasicDash = 1,
        DoubleJump = 25,
        EnhancedDash = 50,
        EvasiveDash = 75,
        AirDash = 100,

        // Warrior Abilities
        BasicCombat = 1,
        ImprovedDefense = 25,
        WeaponMastery = 50,
        BattleCry = 75,
        BerserkerRage = 100,

        // Archer Abilities
        BasicArchery = 1,
        PreciseShot = 25,
        RapidFire = 50,
        PowerShot = 75,
        MultiShot = 100,

        // Mage Abilities
        BasicMagic = 1,
        SpellMastery = 25,
        ElementalPower = 50,
        ArcaneShield = 75,
        SpellCascade = 100,

        // Summoner Abilities
        BasicSummon = 1,
        ImprovedMinions = 25,
        MinionMastery = 50,
        SoulBond = 75,
        LegionCommand = 100,

        // Explorer Abilities
        BasicExploration = 1,
        ImprovedMovement = 25,
        TreasureHunter = 50,
        PathFinder = 75,
        MasterExplorer = 100,

        // Engineer Abilities
        BasicEngineering = 1,
        ImprovedCrafting = 25,
        AdvancedMechanics = 50,
        TechMastery = 75,
        MasterInventor = 100,

        // Survivalist Abilities
        BasicSurvival = 1,
        ImprovedVitality = 25,
        NaturalHealing = 50,
        AdaptiveBody = 75,
        UltimateEndurance = 100,

        // Blacksmith Abilities
        BasicSmithing = 1,
        ImprovedForging = 25,
        MasterBlacksmith = 50,
        LegendaryWeapons = 75,
        DivineCrafting = 100,

        // Alchemist Abilities
        BasicAlchemy = 1,
        ImprovedPotions = 25,
        MasterBrewer = 50,
        ElementalMixing = 75,
        UltimatePotions = 100,

        // Mystic Abilities
        BasicMysticism = 1,
        ImprovedRegeneration = 25,
        VitalityMastery = 50,
        SpiritualHarmony = 75,
        TranscendentBeing = 100
    }

    public class ClassInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<ClassAbility, string> Milestones { get; set; }
        public Dictionary<string, float> BaseStats { get; set; }
        public Dictionary<string, float> StatsPerLevel { get; set; }

        public Dictionary<string, float> StatBonuses
        {
            get
            {
                var bonuses = new Dictionary<string, float>();
                foreach (var stat in StatsPerLevel)
                {
                    bonuses[stat.Key] = stat.Value;
                }
                return bonuses;
            }
        }

        public ClassInfo(string name, string description)
        {
            Name = name;
            Description = description;
            Milestones = new Dictionary<ClassAbility, string>();
            BaseStats = new Dictionary<string, float>();
            StatsPerLevel = new Dictionary<string, float>();
        }
    }

    public static class RPGClassDefinitions
    {
        public static readonly Dictionary<string, ClassInfo> ActionClasses = new Dictionary<string, ClassInfo>
        {
            // === COMBAT CLASSES ===
            
            // Warrior
            {"warrior", new ClassInfo(
                "Warrior",
                "Master of melee combat")
            {
                Milestones = new Dictionary<ClassAbility, string>
                {
                    {ClassAbility.BasicCombat, "Basic combat techniques"},
                    {ClassAbility.ImprovedDefense, "Enhanced defense"},
                    {ClassAbility.WeaponMastery, "Weapon mastery"},
                    {ClassAbility.BattleCry, "Battle cry"},
                    {ClassAbility.BerserkerRage, "Berserker rage"}
                },
                BaseStats = new Dictionary<string, float>
                {
                    {"health", 100},
                    {"defense", 5},
                    {"meleeDamage", 1.1f},
                    {"staminaRegen", 0.2f}
                },
                StatsPerLevel = new Dictionary<string, float>
                {
                    {"health", 10},
                    {"defense", 0.5f},
                    {"meleeDamage", 0.02f},
                    {"staminaRegen", 0.01f}
                }
            }},

            // Archer
            {"archer", new ClassInfo(
                "Archer",
                "Specialist in ranged combat")
            {
                Milestones = new Dictionary<ClassAbility, string>
                {
                    {ClassAbility.BasicArchery, "Basic archery techniques"},
                    {ClassAbility.PreciseShot, "Precise shot"},
                    {ClassAbility.RapidFire, "Rapid fire"},
                    {ClassAbility.PowerShot, "Power shot"},
                    {ClassAbility.MultiShot, "Multi shot"}
                },
                BaseStats = new Dictionary<string, float>
                {
                    {"health", 80},
                    {"defense", 3},
                    {"rangedDamage", 1.1f},
                    {"movementSpeed", 1.1f}
                },
                StatsPerLevel = new Dictionary<string, float>
                {
                    {"health", 8},
                    {"defense", 0.3f},
                    {"rangedDamage", 0.02f},
                    {"movementSpeed", 0.01f}
                }
            }},

            // Mage
            {"mage", new ClassInfo(
                "Mage",
                "Master of arcane arts")
            {
                Milestones = new Dictionary<ClassAbility, string>
                {
                    {ClassAbility.BasicMagic, "Basic magic"},
                    {ClassAbility.SpellMastery, "Spell mastery"},
                    {ClassAbility.ElementalPower, "Elemental power"},
                    {ClassAbility.ArcaneShield, "Arcane shield"},
                    {ClassAbility.SpellCascade, "Spell cascade"}
                },
                BaseStats = new Dictionary<string, float>
                {
                    {"health", 70},
                    {"mana", 120},
                    {"magicDamage", 1.15f},
                    {"manaRegen", 0.3f}
                },
                StatsPerLevel = new Dictionary<string, float>
                {
                    {"health", 7},
                    {"mana", 12},
                    {"magicDamage", 0.025f},
                    {"manaRegen", 0.02f}
                }
            }},

            // Summoner
            {"summoner", new ClassInfo(
                "Summoner",
                "Commander of mystical creatures")
            {
                Milestones = new Dictionary<ClassAbility, string>
                {
                    {ClassAbility.BasicSummon, "Basic summoning"},
                    {ClassAbility.ImprovedMinions, "Enhanced servants"},
                    {ClassAbility.MinionMastery, "Summoning mastery"},
                    {ClassAbility.SoulBond, "Spiritual bond"},
                    {ClassAbility.LegionCommand, "Legion command"}
                },
                BaseStats = new Dictionary<string, float>
                {
                    {"health", 75},
                    {"mana", 100},
                    {"summonDamage", 1.15f},
                    {"maxMinions", 1}
                },
                StatsPerLevel = new Dictionary<string, float>
                {
                    {"health", 7.5f},
                    {"mana", 10},
                    {"summonDamage", 0.025f},
                    {"maxMinions", 0.1f}
                }
            }},

            // === UTILITY CLASSES ===

            // Acrobat
            {"acrobat", new ClassInfo(
                "Acrobat",
                "Master of mobility and agility")
            {
                Milestones = new Dictionary<ClassAbility, string>
                {
                    {ClassAbility.BasicDash, "Basic dash"},
                    {ClassAbility.DoubleJump, "Double jump"},
                    {ClassAbility.EnhancedDash, "Enhanced dash"},
                    {ClassAbility.EvasiveDash, "Evasive dash"},
                    {ClassAbility.AirDash, "Air dash"}
                },
                BaseStats = new Dictionary<string, float>
                {
                    {"movementSpeed", 1.2f},
                    {"jumpSpeed", 1.1f},
                    {"staminaRegen", 0.3f},
                    {"maxStamina", 100}
                },
                StatsPerLevel = new Dictionary<string, float>
                {
                    {"movementSpeed", 0.01f},
                    {"jumpSpeed", 0.01f},
                    {"staminaRegen", 0.02f},
                    {"maxStamina", 5}
                }
            }},

            // Explorer
            {"explorer", new ClassInfo(
                "Explorer",
                "Specialist in exploration and discovery")
            {
                Milestones = new Dictionary<ClassAbility, string>
                {
                    {ClassAbility.BasicExploration, "Basic exploration"},
                    {ClassAbility.ImprovedMovement, "Enhanced movement"},
                    {ClassAbility.TreasureHunter, "Treasure hunter"},
                    {ClassAbility.PathFinder, "Pathfinder"},
                    {ClassAbility.MasterExplorer, "Master Explorer"}
                },
                BaseStats = new Dictionary<string, float>
                {
                    {"movementSpeed", 1.15f},
                    {"lightRadius", 1.2f},
                    {"staminaRegen", 0.25f},
                    {"treasureFindChance", 1.1f}
                },
                StatsPerLevel = new Dictionary<string, float>
                {
                    {"movementSpeed", 0.01f},
                    {"lightRadius", 0.02f},
                    {"staminaRegen", 0.015f},
                    {"treasureFindChance", 0.02f}
                }
            }},

            // Engineer
            {"engineer", new ClassInfo(
                "Engineer",
                "Master of technology and construction")
            {
                Milestones = new Dictionary<ClassAbility, string>
                {
                    {ClassAbility.BasicEngineering, "Basic engineering"},
                    {ClassAbility.ImprovedCrafting, "Enhanced crafting"},
                    {ClassAbility.AdvancedMechanics, "Advanced mechanics"},
                    {ClassAbility.TechMastery, "Technology mastery"},
                    {ClassAbility.MasterInventor, "Supreme inventor"}
                },
                BaseStats = new Dictionary<string, float>
                {
                    {"craftingSpeed", 1.2f},
                    {"craftingQuality", 1.1f},
                    {"toolDurability", 1.2f},
                    {"inventionChance", 1.1f}
                },
                StatsPerLevel = new Dictionary<string, float>
                {
                    {"craftingSpeed", 0.02f},
                    {"craftingQuality", 0.01f},
                    {"toolDurability", 0.02f},
                    {"inventionChance", 0.01f}
                }
            }},

            // Survivalist
            {"survivalist", new ClassInfo(
                "Survivalist",
                "Specialist in survival and adaptation")
            {
                Milestones = new Dictionary<ClassAbility, string>
                {
                    {ClassAbility.BasicSurvival, "Basic survival"},
                    {ClassAbility.ImprovedVitality, "Enhanced vitality"},
                    {ClassAbility.NaturalHealing, "Natural healing"},
                    {ClassAbility.AdaptiveBody, "Adaptive body"},
                    {ClassAbility.UltimateEndurance, "Supreme endurance"}
                },
                BaseStats = new Dictionary<string, float>
                {
                    {"health", 90},
                    {"healthRegen", 0.2f},
                    {"hungerRate", 0.8f},
                    {"environmentalResistance", 1.2f}
                },
                StatsPerLevel = new Dictionary<string, float>
                {
                    {"health", 9},
                    {"healthRegen", 0.02f},
                    {"hungerRate", -0.01f},
                    {"environmentalResistance", 0.02f}
                }
            }},

            // Blacksmith
            {"blacksmith", new ClassInfo(
                "Blacksmith",
                "Master of forging and metallurgy")
            {
                Milestones = new Dictionary<ClassAbility, string>
                {
                    {ClassAbility.BasicSmithing, "Basic smithing"},
                    {ClassAbility.ImprovedForging, "Enhanced forging"},
                    {ClassAbility.MasterBlacksmith, "Master Blacksmith"},
                    {ClassAbility.LegendaryWeapons, "Legendary weapons"},
                    {ClassAbility.DivineCrafting, "Divine crafting"}
                },
                BaseStats = new Dictionary<string, float>
                {
                    {"forgingSpeed", 1.2f},
                    {"forgingQuality", 1.1f},
                    {"weaponDurability", 1.2f},
                    {"masterworkChance", 1.1f}
                },
                StatsPerLevel = new Dictionary<string, float>
                {
                    {"forgingSpeed", 0.02f},
                    {"forgingQuality", 0.01f},
                    {"weaponDurability", 0.02f},
                    {"masterworkChance", 0.01f}
                }
            }},

            // Alchemist
            {"alchemist", new ClassInfo(
                "Alchemist",
                "Master of potions and elixirs")
            {
                Milestones = new Dictionary<ClassAbility, string>
                {
                    {ClassAbility.BasicAlchemy, "Basic alchemy"},
                    {ClassAbility.ImprovedPotions, "Enhanced potions"},
                    {ClassAbility.MasterBrewer, "Master Alchemist"},
                    {ClassAbility.ElementalMixing, "Elemental mixing"},
                    {ClassAbility.UltimatePotions, "Supreme potions"}
                },
                BaseStats = new Dictionary<string, float>
                {
                    {"brewingSpeed", 1.2f},
                    {"potionDuration", 1.1f},
                    {"potionEffectiveness", 1.2f},
                    {"discoveryChance", 1.1f}
                },
                StatsPerLevel = new Dictionary<string, float>
                {
                    {"brewingSpeed", 0.02f},
                    {"potionDuration", 0.01f},
                    {"potionEffectiveness", 0.02f},
                    {"discoveryChance", 0.01f}
                }
            }},

            // Mystic
            {"mystic", new ClassInfo(
                "Mystic",
                "Master of vital balance and harmony")
            {
                Milestones = new Dictionary<ClassAbility, string>
                {
                    {ClassAbility.BasicMysticism, "Basic mysticism"},
                    {ClassAbility.ImprovedRegeneration, "Enhanced regeneration"},
                    {ClassAbility.VitalityMastery, "Vitality mastery"},
                    {ClassAbility.SpiritualHarmony, "Spiritual harmony"},
                    {ClassAbility.TranscendentBeing, "Transcendent being"}
                },
                BaseStats = new Dictionary<string, float>
                {
                    {"healthRegen", 0.3f},
                    {"manaRegen", 0.3f},
                    {"staminaRegen", 0.3f},
                    {"sanityRegen", 0.3f},
                    {"hungerRegen", 0.3f}
                },
                StatsPerLevel = new Dictionary<string, float>
                {
                    {"healthRegen", 0.02f},
                    {"manaRegen", 0.02f},
                    {"staminaRegen", 0.02f},
                    {"sanityRegen", 0.02f},
                    {"hungerRegen", 0.02f}
                }
            }}
        };

        // Random stats information for items
        public static readonly Dictionary<string, StatInfo> RandomStats = new Dictionary<string, StatInfo>
        {
            {"damage", new StatInfo("Damage", 1f, 5f)},
            {"critChance", new StatInfo("Critical Chance", 1f, 3f)},
            {"attackSpeed", new StatInfo("Attack Speed", 1f, 3f)},
            {"defense", new StatInfo("Defense", 1f, 4f)},
            {"moveSpeed", new StatInfo("Movement Speed", 1f, 3f)},
            {"lifeRegen", new StatInfo("Health Regeneration", 1f, 2f)},
            {"manaRegen", new StatInfo("Mana Regeneration", 1f, 2f)},
            {"staminaRegen", new StatInfo("Stamina Regeneration", 1f, 2f)},
            {"sanityRegen", new StatInfo("Sanity Regeneration", 1f, 2f)},
            {"hungerRegen", new StatInfo("Hunger Regeneration", 1f, 2f)}
        };

        // Number of stats per rarity
        public static readonly Dictionary<ItemRarity, int> StatsPerRarity = new Dictionary<ItemRarity, int>
        {
            {ItemRarity.Common, 1},
            {ItemRarity.Uncommon, 2},
            {ItemRarity.Rare, 3},
            {ItemRarity.Epic, 4},
            {ItemRarity.Legendary, 5}
        };

        // Stat multiplier per rarity
        public static readonly Dictionary<ItemRarity, float> StatMultiplierPerRarity = new Dictionary<ItemRarity, float>
        {
            {ItemRarity.Common, 1.0f},
            {ItemRarity.Uncommon, 1.2f},
            {ItemRarity.Rare, 1.5f},
            {ItemRarity.Epic, 2.0f},
            {ItemRarity.Legendary, 2.5f}
        };
    }

    public class StatInfo
    {
        public string Name { get; private set; }
        public float MinValue { get; private set; }
        public float MaxValue { get; private set; }

        public StatInfo(string name, float minValue, float maxValue)
        {
            Name = name;
            MinValue = minValue;
            MaxValue = maxValue;
        }
    }
} 