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
            // === GUERREIRO ===
            ["warrior"] = new List<ClassMilestone>
            {
                new ClassMilestone("Vigor Inicial", "Aumenta a vida máxima", 1)
                {
                    StatBonuses = { ["maxLife"] = 5f }
                },
                new ClassMilestone("Pele Grossa", "Aumenta a defesa", 5)
                {
                    StatBonuses = { ["defense"] = 4f }
                },
                new ClassMilestone("Brute Force", "Increases melee damage", 10)
                {
                    StatBonuses = { ["meleeDamage"] = 0.06f }
                },
                new ClassMilestone("Instinto de Sobrevivência", "Aumenta a regeneração de vida", 15)
                {
                    StatBonuses = { ["lifeRegen"] = 1f }
                },
                new ClassMilestone("Golpe Decisivo", "Aumenta a chance de crítico corpo a corpo", 20)
                {
                    StatBonuses = { ["meleeCrit"] = 8f }
                },
                new ClassMilestone("Armadura Improvisada", "Defesa extra ao receber dano crítico", 25)
                {
                    SpecialEffect = "defense_on_crit"
                },
                new ClassMilestone("Warrior's Fury", "Increases attack speed", 30)
                {
                    StatBonuses = { ["meleeSpeed"] = 0.10f }
                },
                new ClassMilestone("Espírito Indomável", "Chance de ignorar dano fatal", 35)
                {
                    SpecialEffect = "ignore_fatal_damage"
                },
                new ClassMilestone("Muralha de Ferro", "Aumenta a defesa permanentemente", 40)
                {
                    StatBonuses = { ["defense"] = 10f }
                },
                new ClassMilestone("Campeão de Batalha", "Aumenta vida e defesa", 45)
                {
                    StatBonuses = { ["maxLife"] = 10f, ["defense"] = 10f }
                },
                new ClassMilestone("Lenda Viva", "Aumenta dano e defesa", 50)
                {
                    StatBonuses = { ["meleeDamage"] = 0.15f, ["defense"] = 15f }
                }
            },

            // === ARQUEIRO ===
            ["archer"] = new List<ClassMilestone>
            {
                new ClassMilestone("Olho de Águia", "Aumenta a velocidade de projéteis", 1)
                {
                    StatBonuses = { ["projectileSpeed"] = 0.05f }
                },
                new ClassMilestone("Mira Precisa", "Aumenta a chance de crítico à distância", 5)
                {
                    StatBonuses = { ["rangedCrit"] = 5f }
                },
                new ClassMilestone("Tiro Certeiro", "Aumenta o dano à distância", 10)
                {
                    StatBonuses = { ["rangedDamage"] = 0.08f }
                },
                new ClassMilestone("Flecha Perfurante", "Flechas atravessam inimigos extras", 15)
                {
                    SpecialEffect = "piercing_arrows"
                },
                new ClassMilestone("Munição Eficiente", "Chance de não consumir munição", 20)
                {
                    SpecialEffect = "ammo_conservation"
                },
                new ClassMilestone("Recarga Rápida", "Aumenta a velocidade de recarga", 25)
                {
                    StatBonuses = { ["reloadSpeed"] = 0.10f }
                },
                new ClassMilestone("Disparo Duplo", "Chance de disparar flecha extra", 30)
                {
                    SpecialEffect = "double_shot"
                },
                new ClassMilestone("Camuflagem", "Aumenta a chance de esquiva", 35)
                {
                    StatBonuses = { ["dodgeChance"] = 8f }
                },
                new ClassMilestone("Mestre do Arco", "Aumenta o dano à distância", 40)
                {
                    StatBonuses = { ["rangedDamage"] = 0.12f }
                },
                new ClassMilestone("Flecha Fantasma", "Chance de disparar flecha fantasma", 45)
                {
                    SpecialEffect = "ghost_arrow"
                },
                new ClassMilestone("Tiro Perfeito", "Crítico garantido uma vez por combate", 50)
                {
                    SpecialEffect = "guaranteed_crit"
                }
            },

            // === MAGO ===
            ["mage"] = new List<ClassMilestone>
            {
                new ClassMilestone("Mente Brilhante", "Aumenta a mana máxima", 1)
                {
                    StatBonuses = { ["maxMana"] = 20f }
                },
                new ClassMilestone("Meditação", "Aumenta a regeneração de mana", 5)
                {
                    StatBonuses = { ["manaRegen"] = 0.05f }
                },
                new ClassMilestone("Poder Arcano", "Aumenta o dano mágico", 10)
                {
                    StatBonuses = { ["magicDamage"] = 0.08f }
                },
                new ClassMilestone("Conjuração Rápida", "Aumenta a velocidade de conjuração", 15)
                {
                    StatBonuses = { ["castSpeed"] = 0.08f }
                },
                new ClassMilestone("Economia de Mana", "Chance de não consumir mana", 20)
                {
                    SpecialEffect = "mana_conservation"
                },
                new ClassMilestone("Projétil Extra", "Chance de projétil extra", 25)
                {
                    SpecialEffect = "extra_projectile"
                },
                new ClassMilestone("Barreira Arcana", "Resistência a dano mágico", 30)
                {
                    StatBonuses = { ["magicResistance"] = 0.08f }
                },
                new ClassMilestone("Magia Persistente", "Magias aplicam debuff de lentidão", 35)
                {
                    SpecialEffect = "slow_debuff"
                },
                new ClassMilestone("Mestre dos Elementos", "Aumenta o dano mágico", 40)
                {
                    StatBonuses = { ["magicDamage"] = 0.12f }
                },
                new ClassMilestone("Mana Infinita", "Chance de mana infinita temporária", 45)
                {
                    SpecialEffect = "infinite_mana"
                },
                new ClassMilestone("Supremo Feiticeiro", "Magia grátis uma vez por combate", 50)
                {
                    SpecialEffect = "free_spell"
                }
            },

            // === ACROBATA ===
            ["acrobat"] = new List<ClassMilestone>
            {
                new ClassMilestone("Passos Ágeis", "Aumenta a velocidade de movimento", 1)
                {
                    StatBonuses = { ["moveSpeed"] = 0.05f }
                },
                new ClassMilestone("Pulo Duplo", "Adiciona um pulo extra", 5)
                {
                    StatBonuses = { ["extraJumps"] = 1f }
                },
                new ClassMilestone("Reflexos Rápidos", "Aumenta a chance de esquiva", 10)
                {
                    StatBonuses = { ["dodgeChance"] = 8f }
                },
                new ClassMilestone("Dash Inicial", "Adiciona um dash extra", 15)
                {
                    StatBonuses = { ["extraDashes"] = 1f }
                },
                new ClassMilestone("Mestre do Dash", "Dash extra a cada 20 níveis", 20)
                {
                    SpecialEffect = "dash_per_level"
                },
                new ClassMilestone("Acrobacia Avançada", "Aumenta a altura do pulo", 25)
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

            // === EXPLORADOR ===
            ["explorer"] = new List<ClassMilestone>
            {
                new ClassMilestone("Passos Ligeiros", "Aumenta a velocidade de movimento", 1)
                {
                    StatBonuses = { ["moveSpeed"] = 0.05f }
                },
                new ClassMilestone("Olhos de Águia", "Aumenta o alcance de visão", 5)
                {
                    StatBonuses = { ["lightRadius"] = 0.08f }
                },
                new ClassMilestone("Rastreador", "Aumenta a chance de loot raro", 10)
                {
                    StatBonuses = { ["rareLootChance"] = 0.05f }
                },
                new ClassMilestone("Caminho Seguro", "Resistência a armadilhas", 15)
                {
                    StatBonuses = { ["trapResistance"] = 0.10f }
                },
                new ClassMilestone("Mestre do Mapa", "Aumenta a velocidade de movimento", 20)
                {
                    StatBonuses = { ["moveSpeed"] = 0.10f }
                },
                new ClassMilestone("Bússola Avançada", "Mostra localização exata", 25)
                {
                    SpecialEffect = "exact_location"
                },
                new ClassMilestone("Mochila Grande", "Adiciona slots de inventário", 30)
                {
                    StatBonuses = { ["inventorySlots"] = 10f }
                },
                new ClassMilestone("Guia de Trilhas", "Velocidade em trilhas", 35)
                {
                    StatBonuses = { ["miningSpeed"] = 0.10f }
                },
                new ClassMilestone("Rastreador Supremo", "Aumenta a chance de loot raro", 40)
                {
                    StatBonuses = { ["rareLootChance"] = 0.10f }
                },
                new ClassMilestone("Mochila Suprema", "Adiciona slots de inventário", 45)
                {
                    StatBonuses = { ["inventorySlots"] = 10f }
                },
                new ClassMilestone("Explorador Supremo", "Bônus finais de movimento e loot", 50)
                {
                    StatBonuses = { ["moveSpeed"] = 0.15f, ["rareLootChance"] = 0.10f }
                }
            },

            // === ENGENHEIRO ===
            ["engineer"] = new List<ClassMilestone>
            {
                new ClassMilestone("Ferramentas Aprimoradas", "Aumenta a velocidade de mineração", 1)
                {
                    StatBonuses = { ["miningSpeed"] = 0.05f }
                },
                new ClassMilestone("Construção Rápida", "Aumenta a velocidade de construção", 5)
                {
                    StatBonuses = { ["buildSpeed"] = 0.05f }
                },
                new ClassMilestone("Armadilhas Eficientes", "Aumenta o dano de armadilhas", 10)
                {
                    StatBonuses = { ["trapDamage"] = 0.08f }
                },
                new ClassMilestone("Torreta Extra", "Adiciona uma torreta extra", 15)
                {
                    StatBonuses = { ["turretSlots"] = 1f }
                },
                new ClassMilestone("Reparos Rápidos", "Aumenta a velocidade de reparo", 20)
                {
                    StatBonuses = { ["repairSpeed"] = 0.10f }
                },
                new ClassMilestone("Engrenagem Avançada", "Aumenta a eficiência de máquinas", 25)
                {
                    StatBonuses = { ["machineEfficiency"] = 0.08f }
                },
                new ClassMilestone("Armadilha Explosiva", "Armadilhas causam explosão", 30)
                {
                    SpecialEffect = "explosive_traps"
                },
                new ClassMilestone("Torreta de Fogo", "Adiciona uma torreta extra", 35)
                {
                    StatBonuses = { ["turretSlots"] = 1f }
                },
                new ClassMilestone("Mestre das Máquinas", "Aumenta o dano de armadilhas", 40)
                {
                    StatBonuses = { ["trapDamage"] = 0.10f }
                },
                new ClassMilestone("Construção Suprema", "Aumenta a velocidade de construção", 45)
                {
                    StatBonuses = { ["buildSpeed"] = 0.10f }
                },
                new ClassMilestone("Engenheiro Supremo", "Bônus finais de eficiência e dano", 50)
                {
                    StatBonuses = { ["machineEfficiency"] = 0.10f, ["trapDamage"] = 0.10f }
                }
            },

            // === SOBREVIVENTE ===
            ["survivalist"] = new List<ClassMilestone>
            {
                new ClassMilestone("Instinto de Sobrevivente", "Aumenta a resistência a dano", 1)
                {
                    StatBonuses = { ["damageResistance"] = 0.05f }
                },
                new ClassMilestone("Forrageador", "Aumenta a chance de loot extra", 5)
                {
                    StatBonuses = { ["extraLootChance"] = 0.05f }
                },
                new ClassMilestone("Regeneração Natural", "Aumenta a regeneração de vida", 10)
                {
                    StatBonuses = { ["lifeRegen"] = 1f }
                },
                new ClassMilestone("Caçador de Recursos", "Aumenta a chance de recursos raros", 15)
                {
                    StatBonuses = { ["rareResourceChance"] = 0.05f }
                },
                new ClassMilestone("Mestre da Fome", "Fome diminui mais devagar", 20)
                {
                    StatBonuses = { ["hungerRate"] = -0.10f }
                },
                new ClassMilestone("Imunidade a Doenças", "Resistência a debuffs", 25)
                {
                    StatBonuses = { ["debuffResistance"] = 0.10f }
                },
                new ClassMilestone("Regeneração Suprema", "Aumenta a regeneração de vida", 30)
                {
                    StatBonuses = { ["lifeRegen"] = 2f }
                },
                new ClassMilestone("Mestre da Sanidade", "Sanidade diminui mais devagar", 35)
                {
                    StatBonuses = { ["sanityRate"] = -0.10f }
                },
                new ClassMilestone("Sobrevivente Nato", "Bônus de resistência e loot", 40)
                {
                    StatBonuses = { ["damageResistance"] = 0.10f, ["extraLootChance"] = 0.10f }
                },
                new ClassMilestone("Caçador de Tesouros", "Aumenta a chance de loot extra", 45)
                {
                    StatBonuses = { ["extraLootChance"] = 0.10f }
                },
                new ClassMilestone("Lenda da Sobrevivência", "Bônus finais de resistência e regeneração", 50)
                {
                    StatBonuses = { ["damageResistance"] = 0.15f, ["lifeRegen"] = 3f }
                }
            },

            // === FERREIRO ===
            ["blacksmith"] = new List<ClassMilestone>
            {
                new ClassMilestone("Forja Aprimorada", "Aumenta a chance de itens melhores", 1)
                {
                    StatBonuses = { ["forgeQuality"] = 0.05f }
                },
                new ClassMilestone("Reparos Rápidos", "Aumenta a velocidade de reparo", 5)
                {
                    StatBonuses = { ["repairSpeed"] = 0.05f }
                },
                new ClassMilestone("Dano de Ferramenta", "Aumenta o dano com ferramentas", 10)
                {
                    StatBonuses = { ["toolDamage"] = 0.08f }
                },
                new ClassMilestone("Forja de Armas", "Aumenta o dano com armas forjadas", 15)
                {
                    StatBonuses = { ["forgedWeaponDamage"] = 0.08f }
                },
                new ClassMilestone("Mestre da Forja", "Aumenta a chance de itens raros", 20)
                {
                    StatBonuses = { ["rareItemChance"] = 0.10f }
                },
                new ClassMilestone("Reparos Divinos", "Aumenta a velocidade de reparo", 25)
                {
                    StatBonuses = { ["repairSpeed"] = 0.10f }
                },
                new ClassMilestone("Forja Suprema", "Aumenta a chance de itens raros", 30)
                {
                    StatBonuses = { ["rareItemChance"] = 0.15f }
                },
                new ClassMilestone("Dano Supremo", "Aumenta o dano com ferramentas e armas", 35)
                {
                    StatBonuses = { ["toolDamage"] = 0.10f, ["forgedWeaponDamage"] = 0.10f }
                },
                new ClassMilestone("Lenda da Forja", "Aumenta a chance de itens raros", 40)
                {
                    StatBonuses = { ["rareItemChance"] = 0.20f }
                },
                new ClassMilestone("Reparos Supremos", "Chance de reparos grátis", 45)
                {
                    SpecialEffect = "free_repairs"
                },
                new ClassMilestone("Ferreiro Supremo", "Bônus finais de forja e dano", 50)
                {
                    StatBonuses = { ["rareItemChance"] = 0.25f, ["forgedWeaponDamage"] = 0.10f }
                }
            },

            // === ALQUIMISTA ===
            ["alchemist"] = new List<ClassMilestone>
            {
                new ClassMilestone("Mãos de Ouro", "Aumenta a cura de poções", 1)
                {
                    StatBonuses = { ["potionHealing"] = 0.05f }
                },
                new ClassMilestone("Mestre dos Buffs", "Aumenta a duração de buffs", 5)
                {
                    StatBonuses = { ["buffDuration"] = 0.05f }
                },
                new ClassMilestone("Economia de Ingredientes", "Chance de não consumir poção", 10)
                {
                    SpecialEffect = "potion_conservation"
                },
                new ClassMilestone("Poção Potente", "Chance de buff extra", 15)
                {
                    SpecialEffect = "extra_buff"
                },
                new ClassMilestone("Estoque Extra", "Adiciona slot de buff", 20)
                {
                    StatBonuses = { ["buffSlots"] = 1f }
                },
                new ClassMilestone("Sinergia Vital", "Poções de cura restauram mana", 25)
                {
                    SpecialEffect = "heal_restores_mana"
                },
                new ClassMilestone("Poção Compartilhada", "Buffs afetam aliados", 30)
                {
                    SpecialEffect = "shared_buffs"
                },
                new ClassMilestone("Mestre dos Elixires", "Aumenta a duração de buffs", 35)
                {
                    StatBonuses = { ["buffDuration"] = 0.10f }
                },
                new ClassMilestone("Poção Suprema", "Aumenta a cura de poções", 40)
                {
                    StatBonuses = { ["potionHealing"] = 0.10f }
                },
                new ClassMilestone("Alquimia Rápida", "Aumenta a velocidade de uso", 45)
                {
                    StatBonuses = { ["potionUseSpeed"] = 0.10f }
                },
                new ClassMilestone("Alquimista Supremo", "Bônus finais de duração e cura", 50)
                {
                    StatBonuses = { ["buffDuration"] = 0.15f, ["potionHealing"] = 0.15f }
                }
            },

            // === MÍSTICO ===
            ["mystic"] = new List<ClassMilestone>
            {
                new ClassMilestone("Aura Protetora", "Resistência a debuffs", 1)
                {
                    StatBonuses = { ["debuffResistance"] = 0.05f }
                },
                new ClassMilestone("Sorte do Destino", "Aumenta a sorte", 5)
                {
                    StatBonuses = { ["luck"] = 0.02f }
                },
                new ClassMilestone("Invocação Aprimorada", "Aumenta o dano de invocação", 10)
                {
                    StatBonuses = { ["summonDamage"] = 0.05f }
                },
                new ClassMilestone("Minion Extra", "Adiciona um minion", 15)
                {
                    StatBonuses = { ["minionSlots"] = 1f }
                },
                new ClassMilestone("Vida Espiritual", "Minions têm mais vida", 20)
                {
                    StatBonuses = { ["minionHealth"] = 0.05f }
                },
                new ClassMilestone("Sentinela Extra", "Adiciona uma sentinela", 25)
                {
                    StatBonuses = { ["sentinelSlots"] = 1f }
                },
                new ClassMilestone("Fraqueza Mística", "Minions aplicam debuff", 30)
                {
                    SpecialEffect = "minion_debuff"
                },
                new ClassMilestone("Aura de Cura", "Regeneração para aliados", 35)
                {
                    StatBonuses = { ["allyLifeRegen"] = 1f }
                },
                new ClassMilestone("Mestre dos Espíritos", "Bônus de invocação e minion", 40)
                {
                    StatBonuses = { ["summonDamage"] = 0.08f, ["minionSlots"] = 1f }
                },
                new ClassMilestone("Invocação Rápida", "Aumenta a velocidade de invocação", 45)
                {
                    StatBonuses = { ["summonSpeed"] = 0.08f }
                },
                new ClassMilestone("Místico Supremo", "Bônus finais de minions e sentinelas", 50)
                {
                    StatBonuses = { ["minionSlots"] = 2f, ["sentinelSlots"] = 2f }
                }
            }
        };

        /// <summary>
        /// Obtém todas as milestones desbloqueadas para uma classe específica
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
        /// Obtém a próxima milestone para uma classe específica
        /// </summary>
        public static ClassMilestone GetNextMilestone(string className, float classLevel)
        {
            if (!ClassMilestones.ContainsKey(className))
                return null;

            return ClassMilestones[className]
                .FirstOrDefault(m => m.Level > classLevel);
        }

        /// <summary>
        /// Calcula todos os bônus de stats das milestones desbloqueadas
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
        /// Obtém todos os efeitos especiais ativos das milestones desbloqueadas
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