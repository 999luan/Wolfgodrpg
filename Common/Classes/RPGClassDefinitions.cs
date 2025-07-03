using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Wolfgodrpg.Common.Data;

namespace Wolfgodrpg.Common.Classes
{
    public enum ClassAbility
    {
        // Habilidades do Acrobata
        BasicDash = 1,
        DoubleJump = 25,
        EnhancedDash = 50,
        EvasiveDash = 75,
        AirDash = 100,

        // Habilidades do Guerreiro
        BasicCombat = 1,
        ImprovedDefense = 25,
        WeaponMastery = 50,
        BattleCry = 75,
        BerserkerRage = 100,

        // Habilidades do Arqueiro
        BasicArchery = 1,
        PreciseShot = 25,
        RapidFire = 50,
        PowerShot = 75,
        MultiShot = 100,

        // Habilidades do Mago
        BasicMagic = 1,
        SpellMastery = 25,
        ElementalPower = 50,
        ArcaneShield = 75,
        SpellCascade = 100,

        // Habilidades do Invocador
        BasicSummon = 1,
        ImprovedMinions = 25,
        MinionMastery = 50,
        SoulBond = 75,
        LegionCommand = 100,

        // Habilidades do Explorador
        BasicExploration = 1,
        ImprovedMovement = 25,
        TreasureHunter = 50,
        PathFinder = 75,
        MasterExplorer = 100,

        // Habilidades do Engenheiro
        BasicEngineering = 1,
        ImprovedCrafting = 25,
        AdvancedMechanics = 50,
        TechMastery = 75,
        MasterInventor = 100,

        // Habilidades do Sobrevivente
        BasicSurvival = 1,
        ImprovedVitality = 25,
        NaturalHealing = 50,
        AdaptiveBody = 75,
        UltimateEndurance = 100,

        // Habilidades do Ferreiro
        BasicSmithing = 1,
        ImprovedForging = 25,
        MasterBlacksmith = 50,
        LegendaryWeapons = 75,
        DivineCrafting = 100,

        // Habilidades do Alquimista
        BasicAlchemy = 1,
        ImprovedPotions = 25,
        MasterBrewer = 50,
        ElementalMixing = 75,
        UltimatePotions = 100,

        // Habilidades do Místico
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
            // === CLASSES DE COMBATE ===
            
            // Guerreiro
            {"warrior", new ClassInfo(
                "Guerreiro",
                "Mestre do combate corpo a corpo")
            {
                Milestones = new Dictionary<ClassAbility, string>
                {
                    {ClassAbility.BasicCombat, "Técnicas básicas de combate"},
                    {ClassAbility.ImprovedDefense, "Defesa aprimorada"},
                    {ClassAbility.WeaponMastery, "Maestria com armas"},
                    {ClassAbility.BattleCry, "Grito de guerra"},
                    {ClassAbility.BerserkerRage, "Fúria do berserker"}
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

            // Arqueiro
            {"archer", new ClassInfo(
                "Arqueiro",
                "Especialista em combate à distância")
            {
                Milestones = new Dictionary<ClassAbility, string>
                {
                    {ClassAbility.BasicArchery, "Técnicas básicas de arco"},
                    {ClassAbility.PreciseShot, "Tiro preciso"},
                    {ClassAbility.RapidFire, "Tiro rápido"},
                    {ClassAbility.PowerShot, "Tiro poderoso"},
                    {ClassAbility.MultiShot, "Tiro múltiplo"}
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

            // Mago
            {"mage", new ClassInfo(
                "Mago",
                "Mestre das artes arcanas")
            {
                Milestones = new Dictionary<ClassAbility, string>
                {
                    {ClassAbility.BasicMagic, "Magias básicas"},
                    {ClassAbility.SpellMastery, "Maestria em feitiços"},
                    {ClassAbility.ElementalPower, "Poder elemental"},
                    {ClassAbility.ArcaneShield, "Escudo arcano"},
                    {ClassAbility.SpellCascade, "Cascata de feitiços"}
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

            // Invocador
            {"summoner", new ClassInfo(
                "Invocador",
                "Comandante de criaturas místicas")
            {
                Milestones = new Dictionary<ClassAbility, string>
                {
                    {ClassAbility.BasicSummon, "Invocação básica"},
                    {ClassAbility.ImprovedMinions, "Servos aprimorados"},
                    {ClassAbility.MinionMastery, "Maestria em invocação"},
                    {ClassAbility.SoulBond, "Vínculo espiritual"},
                    {ClassAbility.LegionCommand, "Comando da legião"}
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

            // === CLASSES DE UTILIDADE ===

            // Acrobata
            {"acrobat", new ClassInfo(
                "Acrobata",
                "Mestre da mobilidade e agilidade")
            {
                Milestones = new Dictionary<ClassAbility, string>
                {
                    {ClassAbility.BasicDash, "Dash básico"},
                    {ClassAbility.DoubleJump, "Pulo duplo"},
                    {ClassAbility.EnhancedDash, "Dash aprimorado"},
                    {ClassAbility.EvasiveDash, "Dash evasivo"},
                    {ClassAbility.AirDash, "Dash aéreo"}
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

            // Explorador
            {"explorer", new ClassInfo(
                "Explorador",
                "Especialista em exploração e descoberta")
            {
                Milestones = new Dictionary<ClassAbility, string>
                {
                    {ClassAbility.BasicExploration, "Exploração básica"},
                    {ClassAbility.ImprovedMovement, "Movimento aprimorado"},
                    {ClassAbility.TreasureHunter, "Caçador de tesouros"},
                    {ClassAbility.PathFinder, "Desbravador"},
                    {ClassAbility.MasterExplorer, "Mestre explorador"}
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

            // Engenheiro
            {"engineer", new ClassInfo(
                "Engenheiro",
                "Mestre da tecnologia e construção")
            {
                Milestones = new Dictionary<ClassAbility, string>
                {
                    {ClassAbility.BasicEngineering, "Engenharia básica"},
                    {ClassAbility.ImprovedCrafting, "Criação aprimorada"},
                    {ClassAbility.AdvancedMechanics, "Mecânica avançada"},
                    {ClassAbility.TechMastery, "Maestria tecnológica"},
                    {ClassAbility.MasterInventor, "Inventor supremo"}
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

            // Sobrevivente
            {"survivalist", new ClassInfo(
                "Sobrevivente",
                "Especialista em sobrevivência e adaptação")
            {
                Milestones = new Dictionary<ClassAbility, string>
                {
                    {ClassAbility.BasicSurvival, "Sobrevivência básica"},
                    {ClassAbility.ImprovedVitality, "Vitalidade aprimorada"},
                    {ClassAbility.NaturalHealing, "Cura natural"},
                    {ClassAbility.AdaptiveBody, "Corpo adaptativo"},
                    {ClassAbility.UltimateEndurance, "Resistência suprema"}
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

            // Ferreiro
            {"blacksmith", new ClassInfo(
                "Ferreiro",
                "Mestre da forja e metalurgia")
            {
                Milestones = new Dictionary<ClassAbility, string>
                {
                    {ClassAbility.BasicSmithing, "Forja básica"},
                    {ClassAbility.ImprovedForging, "Forja aprimorada"},
                    {ClassAbility.MasterBlacksmith, "Mestre ferreiro"},
                    {ClassAbility.LegendaryWeapons, "Armas lendárias"},
                    {ClassAbility.DivineCrafting, "Forja divina"}
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

            // Alquimista
            {"alchemist", new ClassInfo(
                "Alquimista",
                "Mestre das poções e elixires")
            {
                Milestones = new Dictionary<ClassAbility, string>
                {
                    {ClassAbility.BasicAlchemy, "Alquimia básica"},
                    {ClassAbility.ImprovedPotions, "Poções aprimoradas"},
                    {ClassAbility.MasterBrewer, "Mestre alquimista"},
                    {ClassAbility.ElementalMixing, "Misturas elementais"},
                    {ClassAbility.UltimatePotions, "Poções supremas"}
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

            // Místico
            {"mystic", new ClassInfo(
                "Místico",
                "Mestre do equilíbrio vital e harmonia")
            {
                Milestones = new Dictionary<ClassAbility, string>
                {
                    {ClassAbility.BasicMysticism, "Misticismo básico"},
                    {ClassAbility.ImprovedRegeneration, "Regeneração aprimorada"},
                    {ClassAbility.VitalityMastery, "Maestria vital"},
                    {ClassAbility.SpiritualHarmony, "Harmonia espiritual"},
                    {ClassAbility.TranscendentBeing, "Ser transcendente"}
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

        // Informações de stats aleatórios para itens
        public static readonly Dictionary<string, StatInfo> RandomStats = new Dictionary<string, StatInfo>
        {
            {"damage", new StatInfo("Dano", 1f, 5f)},
            {"critChance", new StatInfo("Chance Crítica", 1f, 3f)},
            {"attackSpeed", new StatInfo("Velocidade de Ataque", 1f, 3f)},
            {"defense", new StatInfo("Defesa", 1f, 4f)},
            {"moveSpeed", new StatInfo("Velocidade de Movimento", 1f, 3f)},
            {"lifeRegen", new StatInfo("Regeneração de Vida", 1f, 2f)},
            {"manaRegen", new StatInfo("Regeneração de Mana", 1f, 2f)},
            {"staminaRegen", new StatInfo("Regeneração de Stamina", 1f, 2f)},
            {"sanityRegen", new StatInfo("Regeneração de Sanidade", 1f, 2f)},
            {"hungerRegen", new StatInfo("Regeneração de Fome", 1f, 2f)}
        };

        // Quantidade de stats por raridade
        public static readonly Dictionary<ItemRarity, int> StatsPerRarity = new Dictionary<ItemRarity, int>
        {
            {ItemRarity.Common, 1},
            {ItemRarity.Uncommon, 2},
            {ItemRarity.Rare, 3},
            {ItemRarity.Epic, 4},
            {ItemRarity.Legendary, 5}
        };

        // Multiplicador de stats por raridade
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