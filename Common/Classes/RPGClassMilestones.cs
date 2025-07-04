using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Wolfgodrpg.Common.Classes
{
    /// <summary>
    /// Representa uma milestone/habilidade passiva de uma classe
    /// </summary>
    public class ClassMilestone
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public Dictionary<string, float> StatBonuses { get; set; }
        public string SpecialEffect { get; set; }
        public bool IsUnlocked { get; set; }

        public ClassMilestone(string name, string description, int level)
        {
            Name = name;
            Description = description;
            Level = level;
            StatBonuses = new Dictionary<string, float>();
            SpecialEffect = "";
            IsUnlocked = false;
        }
    }

    /// <summary>
    /// Sistema centralizado para gerenciar milestones e habilidades passivas
    /// </summary>
    public static class RPGClassMilestones
    {
        /// <summary>
        /// Dicionário com todas as milestones organizadas por classe
        /// </summary>
        public static readonly Dictionary<string, List<ClassMilestone>> ClassMilestones = new Dictionary<string, List<ClassMilestone>>
        {
            // === WARRIOR ===
            ["warrior"] = new List<ClassMilestone>
            {
                new ClassMilestone("Initial Vigor", "Increases max health", 1)
                {
                    StatBonuses = { ["maxLife"] = 5f }
                },
                new ClassMilestone("Thick Skin", "Increases defense", 5)
                {
                    StatBonuses = { ["defense"] = 4f }
                },
                new ClassMilestone("Brute Force", "Increases melee damage", 10)
                {
                    StatBonuses = { ["meleeDamage"] = 0.06f }
                },
                new ClassMilestone("Survival Instinct", "Increases health regeneration", 15)
                {
                    StatBonuses = { ["lifeRegen"] = 1f }
                },
                new ClassMilestone("Decisive Strike", "Increases melee critical chance", 20)
                {
                    StatBonuses = { ["meleeCrit"] = 8f }
                },
                new ClassMilestone("Improvised Armor", "Extra defense when taking critical damage", 25)
                {
                    SpecialEffect = "defense_on_crit"
                },
                new ClassMilestone("Warrior's Fury", "Increases attack speed", 30)
                {
                    StatBonuses = { ["meleeSpeed"] = 0.10f }
                },
                new ClassMilestone("Indomitable Spirit", "Chance to ignore fatal damage", 35)
                {
                    SpecialEffect = "ignore_fatal_damage"
                },
                new ClassMilestone("Iron Wall", "Permanently increases defense", 40)
                {
                    StatBonuses = { ["defense"] = 10f }
                },
                new ClassMilestone("Battle Champion", "Increases health and defense", 45)
                {
                    StatBonuses = { ["maxLife"] = 10f, ["defense"] = 10f }
                },
                new ClassMilestone("Living Legend", "Increases damage and defense", 50)
                {
                    StatBonuses = { ["meleeDamage"] = 0.15f, ["defense"] = 15f }
                }
            },

            // === ARCHER ===
            ["archer"] = new List<ClassMilestone>
            {
                new ClassMilestone("Eagle Eye", "Increases projectile speed", 1)
                {
                    StatBonuses = { ["projectileSpeed"] = 0.05f }
                },
                new ClassMilestone("Precise Aim", "Increases ranged critical chance", 5)
                {
                    StatBonuses = { ["rangedCrit"] = 5f }
                },
                new ClassMilestone("Sure Shot", "Increases ranged damage", 10)
                {
                    StatBonuses = { ["rangedDamage"] = 0.08f }
                },
                new ClassMilestone("Piercing Arrow", "Arrows pierce extra enemies", 15)
                {
                    SpecialEffect = "piercing_arrows"
                },
                new ClassMilestone("Efficient Ammunition", "Chance to not consume ammo", 20)
                {
                    SpecialEffect = "ammo_conservation"
                },
                new ClassMilestone("Quick Reload", "Increases reload speed", 25)
                {
                    StatBonuses = { ["reloadSpeed"] = 0.10f }
                },
                new ClassMilestone("Double Shot", "Chance to fire extra arrow", 30)
                {
                    SpecialEffect = "double_shot"
                },
                new ClassMilestone("Camouflage", "Increases dodge chance", 35)
                {
                    StatBonuses = { ["dodgeChance"] = 8f }
                },
                new ClassMilestone("Master of the Bow", "Increases ranged damage", 40)
                {
                    StatBonuses = { ["rangedDamage"] = 0.12f }
                },
                new ClassMilestone("Ghost Arrow", "Chance to fire ghost arrow", 45)
                {
                    SpecialEffect = "ghost_arrow"
                },
                new ClassMilestone("Perfect Shot", "Guaranteed critical once per combat", 50)
                {
                    SpecialEffect = "guaranteed_crit"
                }
            },

            // === MAGE ===
            ["mage"] = new List<ClassMilestone>
            {
                new ClassMilestone("Brilliant Mind", "Increases max mana", 1)
                {
                    StatBonuses = { ["maxMana"] = 20f }
                },
                new ClassMilestone("Meditation", "Increases mana regeneration", 5)
                {
                    StatBonuses = { ["manaRegen"] = 0.05f }
                },
                new ClassMilestone("Arcane Power", "Increases magic damage", 10)
                {
                    StatBonuses = { ["magicDamage"] = 0.08f }
                },
                new ClassMilestone("Rapid Casting", "Increases casting speed", 15)
                {
                    StatBonuses = { ["castSpeed"] = 0.08f }
                },
                new ClassMilestone("Mana Economy", "Chance to not consume mana", 20)
                {
                    SpecialEffect = "mana_conservation"
                },
                new ClassMilestone("Extra Projectile", "Chance of extra projectile", 25)
                {
                    SpecialEffect = "extra_projectile"
                },
                new ClassMilestone("Arcane Barrier", "Resistance to magic damage", 30)
                {
                    StatBonuses = { ["magicResistance"] = 0.08f }
                },
                new ClassMilestone("Persistent Magic", "Spells apply slow debuff", 35)
                {
                    SpecialEffect = "slow_debuff"
                },
                new ClassMilestone("Master of Elements", "Increases magic damage", 40)
                {
                    StatBonuses = { ["magicDamage"] = 0.12f }
                },
                new ClassMilestone("Infinite Mana", "Chance of temporary infinite mana", 45)
                {
                    SpecialEffect = "infinite_mana"
                },
                new ClassMilestone("Supreme Sorcerer", "Free spell once per combat", 50)
                {
                    SpecialEffect = "free_spell"
                }
            },

            // === ACROBAT ===
            ["acrobat"] = new List<ClassMilestone>
            {
                new ClassMilestone("Agile Steps", "Increases movement speed", 1)
                {
                    StatBonuses = { ["moveSpeed"] = 0.05f }
                },
                new ClassMilestone("Double Jump", "Adds an extra jump", 5)
                {
                    StatBonuses = { ["extraJumps"] = 1f }
                },
                new ClassMilestone("Quick Reflexes", "Increases dodge chance", 10)
                {
                    StatBonuses = { ["dodgeChance"] = 8f }
                },
                new ClassMilestone("Initial Dash", "Adds an extra dash", 15)
                {
                    StatBonuses = { ["extraDashes"] = 1f }
                },
                new ClassMilestone("Master of Dash", "Extra dash every 20 levels", 20)
                {
                    SpecialEffect = "dash_per_level"
                },
                new ClassMilestone("Advanced Acrobatics", "Increases jump height", 25)
                {
                    StatBonuses = { ["jumpHeight"] = 0.10f }
                },
                new ClassMilestone("Imunidade a Queda", "Imunidade a dano de queda", 30)
                {
                    SpecialEffect = "fall_immunity"
                },
                new ClassMilestone("Extreme Speed", "Increases movement speed", 35)
                {
                    StatBonuses = { ["moveSpeed"] = 0.10f }
                },
                new ClassMilestone("Ghost Dash", "Chance of ghost dash", 40)
                {
                    SpecialEffect = "ghost_dash"
                },
                new ClassMilestone("Triple Jump", "Adds an extra jump", 45)
                {
                    StatBonuses = { ["extraJumps"] = 1f }
                },
                new ClassMilestone("Supreme Acrobat", "Extra dash and jump", 50)
                {
                    StatBonuses = { ["extraDashes"] = 1f, ["extraJumps"] = 1f }
                }
            },

            // === SUMMONER ===
            ["summoner"] = new List<ClassMilestone>
            {
                new ClassMilestone("Basic Summoning", "Adds a minion", 1)
                {
                    StatBonuses = { ["minionSlots"] = 1f }
                },
                new ClassMilestone("Summon Damage", "Increases summon damage", 5)
                {
                    StatBonuses = { ["summonDamage"] = 0.08f }
                },
                new ClassMilestone("Extra Minion", "Adds a minion", 10)
                {
                    StatBonuses = { ["minionSlots"] = 1f }
                },
                new ClassMilestone("Summon Speed", "Increases summon speed", 15)
                {
                    StatBonuses = { ["summonSpeed"] = 0.08f }
                },
                new ClassMilestone("Master of Minions", "Adds a minion", 20)
                {
                    StatBonuses = { ["minionSlots"] = 1f }
                },
                new ClassMilestone("Supreme Damage", "Increases summon damage", 25)
                {
                    StatBonuses = { ["summonDamage"] = 0.10f }
                },
                new ClassMilestone("Extra Minion II", "Adds a minion", 30)
                {
                    StatBonuses = { ["minionSlots"] = 1f }
                },
                new ClassMilestone("Supreme Speed", "Increases summon speed", 35)
                {
                    StatBonuses = { ["summonSpeed"] = 0.10f }
                },
                new ClassMilestone("Extra Minion III", "Adds a minion", 40)
                {
                    StatBonuses = { ["minionSlots"] = 1f }
                },
                new ClassMilestone("Legendary Damage", "Increases summon damage", 45)
                {
                    StatBonuses = { ["summonDamage"] = 0.12f }
                },
                new ClassMilestone("Supreme Summoner", "Adds two final minions", 50)
                {
                    StatBonuses = { ["minionSlots"] = 2f }
                }
            },

            // === EXPLORER ===
            ["explorer"] = new List<ClassMilestone>
            {
                new ClassMilestone("Light Steps", "Increases movement speed", 1)
                {
                    StatBonuses = { ["moveSpeed"] = 0.05f }
                },
                new ClassMilestone("Eagle Eyes", "Increases vision range", 5)
                {
                    StatBonuses = { ["lightRadius"] = 0.08f }
                },
                new ClassMilestone("Tracker", "Increases rare loot chance", 10)
                {
                    StatBonuses = { ["rareLootChance"] = 0.05f }
                },
                new ClassMilestone("Safe Path", "Trap resistance", 15)
                {
                    StatBonuses = { ["trapResistance"] = 0.10f }
                },
                new ClassMilestone("Map Master", "Increases movement speed", 20)
                {
                    StatBonuses = { ["moveSpeed"] = 0.10f }
                },
                new ClassMilestone("Advanced Compass", "Shows exact location", 25)
                {
                    SpecialEffect = "exact_location"
                },
                new ClassMilestone("Large Backpack", "Adds inventory slots", 30)
                {
                    StatBonuses = { ["inventorySlots"] = 10f }
                },
                new ClassMilestone("Trail Guide", "Speed on trails", 35)
                {
                    StatBonuses = { ["miningSpeed"] = 0.10f }
                },
                new ClassMilestone("Supreme Tracker", "Increases rare loot chance", 40)
                {
                    StatBonuses = { ["rareLootChance"] = 0.10f }
                },
                new ClassMilestone("Supreme Backpack", "Adds inventory slots", 45)
                {
                    StatBonuses = { ["inventorySlots"] = 10f }
                },
                new ClassMilestone("Supreme Explorer", "Final movement and loot bonuses", 50)
                {
                    StatBonuses = { ["moveSpeed"] = 0.15f, ["rareLootChance"] = 0.10f }
                }
            },

            // === ENGINEER ===
            ["engineer"] = new List<ClassMilestone>
            {
                new ClassMilestone("Enhanced Tools", "Increases mining speed", 1)
                {
                    StatBonuses = { ["miningSpeed"] = 0.05f }
                },
                new ClassMilestone("Rapid Construction", "Increases build speed", 5)
                {
                    StatBonuses = { ["buildSpeed"] = 0.05f }
                },
                new ClassMilestone("Efficient Traps", "Increases trap damage", 10)
                {
                    StatBonuses = { ["trapDamage"] = 0.08f }
                },
                new ClassMilestone("Extra Turret", "Adds an extra turret", 15)
                {
                    StatBonuses = { ["turretSlots"] = 1f }
                },
                new ClassMilestone("Quick Repairs", "Increases repair speed", 20)
                {
                    StatBonuses = { ["repairSpeed"] = 0.10f }
                },
                new ClassMilestone("Advanced Gears", "Increases machine efficiency", 25)
                {
                    StatBonuses = { ["machineEfficiency"] = 0.08f }
                },
                new ClassMilestone("Explosive Trap", "Traps cause explosion", 30)
                {
                    SpecialEffect = "explosive_traps"
                },
                new ClassMilestone("Fire Turret", "Adds an extra turret", 35)
                {
                    StatBonuses = { ["turretSlots"] = 1f }
                },
                new ClassMilestone("Master of Machines", "Increases trap damage", 40)
                {
                    StatBonuses = { ["trapDamage"] = 0.10f }
                },
                new ClassMilestone("Supreme Construction", "Increases build speed", 45)
                {
                    StatBonuses = { ["buildSpeed"] = 0.10f }
                },
                new ClassMilestone("Supreme Engineer", "Final efficiency and damage bonuses", 50)
                {
                    StatBonuses = { ["machineEfficiency"] = 0.10f, ["trapDamage"] = 0.10f }
                }
            },

            // === SURVIVALIST ===
            ["survivalist"] = new List<ClassMilestone>
            {
                new ClassMilestone("Survival Instinct", "Increases damage resistance", 1)
                {
                    StatBonuses = { ["damageResistance"] = 0.05f }
                },
                new ClassMilestone("Forager", "Increases extra loot chance", 5)
                {
                    StatBonuses = { ["extraLootChance"] = 0.05f }
                },
                new ClassMilestone("Natural Regeneration", "Increases health regeneration", 10)
                {
                    StatBonuses = { ["lifeRegen"] = 1f }
                },
                new ClassMilestone("Resource Hunter", "Increases rare resource chance", 15)
                {
                    StatBonuses = { ["rareResourceChance"] = 0.05f }
                },
                new ClassMilestone("Hunger Master", "Hunger decreases slower", 20)
                {
                    StatBonuses = { ["hungerRate"] = -0.10f }
                },
                new ClassMilestone("Disease Immunity", "Debuff resistance", 25)
                {
                    StatBonuses = { ["debuffResistance"] = 0.10f }
                },
                new ClassMilestone("Supreme Regeneration", "Increases health regeneration", 30)
                {
                    StatBonuses = { ["lifeRegen"] = 2f }
                },
                new ClassMilestone("Sanity Master", "Sanity decreases slower", 35)
                {
                    StatBonuses = { ["sanityRate"] = -0.10f }
                },
                new ClassMilestone("Natural Survivor", "Resistance and loot bonuses", 40)
                {
                    StatBonuses = { ["damageResistance"] = 0.10f, ["extraLootChance"] = 0.10f }
                },
                new ClassMilestone("Treasure Hunter", "Increases extra loot chance", 45)
                {
                    StatBonuses = { ["extraLootChance"] = 0.10f }
                },
                new ClassMilestone("Legend of Survival", "Final resistance and regeneration bonuses", 50)
                {
                    StatBonuses = { ["damageResistance"] = 0.15f, ["lifeRegen"] = 3f }
                }
            },

            // === BLACKSMITH ===
            ["blacksmith"] = new List<ClassMilestone>
            {
                new ClassMilestone("Enhanced Forge", "Increases chance of better items", 1)
                {
                    StatBonuses = { ["forgeQuality"] = 0.05f }
                },
                new ClassMilestone("Quick Repairs", "Increases repair speed", 5)
                {
                    StatBonuses = { ["repairSpeed"] = 0.05f }
                },
                new ClassMilestone("Tool Damage", "Increases damage with tools", 10)
                {
                    StatBonuses = { ["toolDamage"] = 0.08f }
                },
                new ClassMilestone("Weapon Forge", "Increases damage with forged weapons", 15)
                {
                    StatBonuses = { ["forgedWeaponDamage"] = 0.08f }
                },
                new ClassMilestone("Master Forge", "Increases chance of rare items", 20)
                {
                    StatBonuses = { ["rareItemChance"] = 0.10f }
                },
                new ClassMilestone("Divine Repairs", "Increases repair speed", 25)
                {
                    StatBonuses = { ["repairSpeed"] = 0.10f }
                },
                new ClassMilestone("Supreme Forge", "Increases chance of rare items", 30)
                {
                    StatBonuses = { ["rareItemChance"] = 0.15f }
                },
                new ClassMilestone("Supreme Damage", "Increases damage with tools and weapons", 35)
                {
                    StatBonuses = { ["toolDamage"] = 0.10f, ["forgedWeaponDamage"] = 0.10f }
                },
                new ClassMilestone("Legend of the Forge", "Increases chance of rare items", 40)
                {
                    StatBonuses = { ["rareItemChance"] = 0.20f }
                },
                new ClassMilestone("Supreme Repairs", "Chance of free repairs", 45)
                {
                    SpecialEffect = "free_repairs"
                },
                new ClassMilestone("Supreme Blacksmith", "Final forging and damage bonuses", 50)
                {
                    StatBonuses = { ["rareItemChance"] = 0.25f, ["forgedWeaponDamage"] = 0.10f }
                }
            },

            // === ALCHEMIST ===
            ["alchemist"] = new List<ClassMilestone>
            {
                new ClassMilestone("Golden Hands", "Increases potion healing", 1)
                {
                    StatBonuses = { ["potionHealing"] = 0.05f }
                },
                new ClassMilestone("Master of Buffs", "Increases buff duration", 5)
                {
                    StatBonuses = { ["buffDuration"] = 0.05f }
                },
                new ClassMilestone("Ingredient Economy", "Chance to not consume potion", 10)
                {
                    SpecialEffect = "potion_conservation"
                },
                new ClassMilestone("Potent Potion", "Chance of extra buff", 15)
                {
                    SpecialEffect = "extra_buff"
                },
                new ClassMilestone("Extra Stock", "Adds buff slot", 20)
                {
                    StatBonuses = { ["buffSlots"] = 1f }
                },
                new ClassMilestone("Vital Synergy", "Healing potions restore mana", 25)
                {
                    SpecialEffect = "heal_restores_mana"
                },
                new ClassMilestone("Shared Potion", "Buffs affect allies", 30)
                {
                    SpecialEffect = "shared_buffs"
                },
                new ClassMilestone("Master of Elixirs", "Increases buff duration", 35)
                {
                    StatBonuses = { ["buffDuration"] = 0.10f }
                },
                new ClassMilestone("Supreme Potion", "Increases potion healing", 40)
                {
                    StatBonuses = { ["potionHealing"] = 0.10f }
                },
                new ClassMilestone("Rapid Alchemy", "Increases use speed", 45)
                {
                    StatBonuses = { ["potionUseSpeed"] = 0.10f }
                },
                new ClassMilestone("Supreme Alchemist", "Final duration and healing bonuses", 50)
                {
                    StatBonuses = { ["buffDuration"] = 0.15f, ["potionHealing"] = 0.15f }
                }
            },

            // === MYSTIC ===
            ["mystic"] = new List<ClassMilestone>
            {
                new ClassMilestone("Protective Aura", "Debuff resistance", 1)
                {
                    StatBonuses = { ["debuffResistance"] = 0.05f }
                },
                new ClassMilestone("Destiny's Luck", "Increases luck", 5)
                {
                    StatBonuses = { ["luck"] = 0.02f }
                },
                new ClassMilestone("Enhanced Summoning", "Increases summon damage", 10)
                {
                    StatBonuses = { ["summonDamage"] = 0.05f }
                },
                new ClassMilestone("Extra Minion", "Adds a minion", 15)
                {
                    StatBonuses = { ["minionSlots"] = 1f }
                },
                new ClassMilestone("Spiritual Life", "Minions have more health", 20)
                {
                    StatBonuses = { ["minionHealth"] = 0.05f }
                },
                new ClassMilestone("Extra Sentinel", "Adds a sentinel", 25)
                {
                    StatBonuses = { ["sentinelSlots"] = 1f }
                },
                new ClassMilestone("Mystic Weakness", "Minions apply debuff", 30)
                {
                    SpecialEffect = "minion_debuff"
                },
                new ClassMilestone("Healing Aura", "Regeneration for allies", 35)
                {
                    StatBonuses = { ["allyLifeRegen"] = 1f }
                },
                new ClassMilestone("Master of Spirits", "Summon and minion bonuses", 40)
                {
                    StatBonuses = { ["summonDamage"] = 0.08f, ["minionSlots"] = 1f }
                },
                new ClassMilestone("Rapid Summoning", "Increases summon speed", 45)
                {
                    StatBonuses = { ["summonSpeed"] = 0.08f }
                },
                new ClassMilestone("Supreme Mystic", "Final minions and sentinels bonuses", 50)
                {
                    StatBonuses = { ["minionSlots"] = 2f, ["sentinelSlots"] = 2f }
                }
            }
        };

        /// <summary>
        /// Gets all unlocked milestones for a specific class
        /// </summary>
        public static List<ClassMilestone> GetUnlockedMilestones(string className, float classLevel)
        {
            if (!ClassMilestones.ContainsKey(className))
                return new List<ClassMilestone>();

            return ClassMilestones[className]
                .Where(m => m.Level <= classLevel)
                .ToList();
        }

        /// <summary>
        /// Gets the next milestone for a specific class
        /// </summary>
        public static ClassMilestone GetNextMilestone(string className, float classLevel)
        {
            if (!ClassMilestones.ContainsKey(className))
                return null;

            return ClassMilestones[className]
                .FirstOrDefault(m => m.Level > classLevel);
        }

        /// <summary>
        /// Calculates all stat bonuses from unlocked milestones
        /// </summary>
        public static Dictionary<string, float> CalculateMilestoneBonuses(string className, float classLevel)
        {
            var bonuses = new Dictionary<string, float>();
            var unlockedMilestones = GetUnlockedMilestones(className, classLevel);

            foreach (var milestone in unlockedMilestones)
            {
                foreach (var bonus in milestone.StatBonuses)
                {
                    if (!bonuses.ContainsKey(bonus.Key))
                        bonuses[bonus.Key] = 0f;
                    bonuses[bonus.Key] += bonus.Value;
                }
            }

            return bonuses;
        }

        /// <summary>
        /// Gets all active special effects from unlocked milestones
        /// </summary>
        public static List<string> GetActiveSpecialEffects(string className, float classLevel)
        {
            var unlockedMilestones = GetUnlockedMilestones(className, classLevel);
            return unlockedMilestones
                .Where(m => !string.IsNullOrEmpty(m.SpecialEffect))
                .Select(m => m.SpecialEffect)
                .ToList();
        }
    }
} 